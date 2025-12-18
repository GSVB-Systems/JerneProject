import { useCallback, useEffect, useRef, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { userClient, authClient } from "../api-clients.ts";
import type { UserDto, UserRole, UpdateUserDto } from "../models/ServerAPI";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

export type UserFormState = {
  firstname: string;
  lastname: string;
  email: string;
  role: UserRole | undefined;
  firstlogin: boolean;
  isActive: boolean;
  balance: number;
  // add ephemeral password fields used during edit only
  newPassword?: string;
  confirmPassword?: string;
};

interface UseUserDetailsResult {
  user: UserDto | null;
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
  formState: UserFormState | null;
  saving: boolean;
  saveMessage: string | null;
  saveError: string | null;
  handleFormChange: (event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => void;
  handleSubmit: (event: FormEvent<HTMLFormElement>) => Promise<void>;
  editing: boolean;
  beginEdit: () => void;
  cancelEdit: () => void;
  deleting: boolean;
  deleteError: string | null;
  resetDeleteState: () => void;
  deleteUser: () => Promise<boolean>;
}

export const useUserDetails = (userId?: string): UseUserDetailsResult => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [formState, setFormState] = useState<UserFormState | null>(null);
  const [saving, setSaving] = useState(false);
  const [saveMessage, setSaveMessage] = useState<string | null>(null);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [editing, setEditing] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const isMounted = useRef(false);
  const parseValidationMessage = useParseValidationMessage("Ukendt fejl ved hentning af bruger.");

  const mapUserToFormState = useCallback((payload: UserDto): UserFormState => ({
    firstname: payload.firstname ?? "",
    lastname: payload.lastname ?? "",
    email: payload.email ?? "",
    role: payload.role,
    firstlogin: payload.firstlogin ?? false,
    isActive: payload.isActive ?? false,
    balance: payload.balance ?? 0,
    // do not prefill password fields
  }), []);

  const fetchUser = useCallback(async () => {
    if (!userId) {
      if (isMounted.current) {
        setError("Bruger-ID mangler.");
        setLoading(false);
      }
      return;
    }

    if (isMounted.current) {
      setLoading(true);
      setError(null);
    }
    try {
      const response = await userClient.getById(userId);
      const text = await response.data?.text();
      const parsed = text ? (JSON.parse(text) as UserDto) : null;
      if (!isMounted.current) return;
      setUser(parsed);
      setFormState(parsed ? mapUserToFormState(parsed) : null);
      setEditing(false);
      if (!parsed) {
        setError("Kunne ikke finde brugeren.");
      }
    } catch (err) {
      if (!isMounted.current) return;
      setError(parseValidationMessage(err));
      setUser(null);
      setFormState(null);
      setEditing(false);
      setDeleteError(null);
    } finally {
      if (isMounted.current) {
        setLoading(false);
      }
    }
  }, [mapUserToFormState, userId, parseValidationMessage]);

  const beginEdit = useCallback(() => {
    if (!formState) return;
    setEditing(true);
    setSaveMessage(null);
    setSaveError(null);
  }, [formState]);

  const cancelEdit = useCallback(() => {
    setEditing(false);
    // reset ephemeral password fields when cancelling
    const base = user ? mapUserToFormState(user) : null;
    setFormState(base);
    setSaveMessage(null);
    setSaveError(null);
  }, [mapUserToFormState, user]);

  const handleFormChange = useCallback((event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = event.target;
    setFormState((prev) => {
      if (!prev) return prev;
      let nextValue: string | number | boolean = value;
      if (type === "checkbox") {
        nextValue = (event.target as HTMLInputElement).checked;
      } else if (name === "balance") {
        nextValue = Number(value);
      } else if (name === "role") {
        nextValue = Number(value) as UserRole;
      }
      return {
        ...prev,
        [name]: nextValue,
      };
    });
    setSaveMessage(null);
    setSaveError(null);
  }, []);

  const handleSubmit = useCallback(async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!formState || !userId) return;

    // validate optional password fields if provided (UI-only; backend update does not accept password)
    if (formState.newPassword || formState.confirmPassword) {
      if ((formState.newPassword ?? "") !== (formState.confirmPassword ?? "")) {
        setSaveError("Adgangskoderne matcher ikke.");
        return;
      }
      if ((formState.newPassword ?? "").length < 8) {
        setSaveError("Adgangskoden skal være mindst 8 tegn.");
        return;
      }
    }

    setSaving(true);
    setSaveMessage(null);
    setSaveError(null);

    let updateSucceeded = false;
    let passwordSucceeded = false;
    const passwordAttempted = Boolean(formState.newPassword);

    try {
      const payload: UpdateUserDto = {
        firstname: formState.firstname,
        lastname: formState.lastname,
        email: formState.email,
        role: formState.role,
        isActive: formState.isActive,
        balance: formState.balance,
      };


      await userClient.update(userId, payload);
      updateSucceeded = true;


      if (passwordAttempted && formState.newPassword) {
        await authClient.adminResetPassword(userId, { newPassword: formState.newPassword });
        passwordSucceeded = true;
      }


      await fetchUser();

      if (passwordAttempted) {
        if (updateSucceeded && passwordSucceeded) {
          setSaveMessage("Brugeroplysninger opdateret og adgangskode nulstillet.");
          setEditing(false);
        } else if (updateSucceeded && !passwordSucceeded) {
          setSaveMessage("Brugeroplysninger opdateret, men nulstilling af adgangskode mislykkedes.");
          // keep editing to allow fixing password fields
        }
      } else {
        setSaveMessage("Brugeroplysninger opdateret.");
        setEditing(false);
      }
    } catch (error) {
      const message = parseValidationMessage(error);
      if (!updateSucceeded) {
        setSaveError(message);
      } else if (passwordAttempted && !passwordSucceeded) {
        setSaveError(message);
      } else {
        setSaveError(message);
      }
    } finally {
      setSaving(false);
    }
  }, [fetchUser, formState, userId, parseValidationMessage]);

  const deleteUser = useCallback(async (): Promise<boolean> => {
    if (!userId) return false;
    setDeleting(true);
    setDeleteError(null);
    try {
      await userClient.delete(userId);
      return true;
    } catch (cause) {
      setDeleteError(parseValidationMessage(cause));
      return false;
    } finally {
      setDeleting(false);
    }
  }, [userId, parseValidationMessage]);

  const resetDeleteState = useCallback(() => {
    setDeleteError(null);
    setDeleting(false);
  }, []);

  useEffect(() => {
    isMounted.current = true;
    fetchUser();
    return () => {
      isMounted.current = false;
    };
  }, [fetchUser]);

  return {
    user,
    loading,
    error,
    refresh: fetchUser,
    formState,
    saving,
    saveMessage,
    saveError,
    handleFormChange,
    handleSubmit,
    editing,
    beginEdit,
    cancelEdit,
    deleting,
    deleteError,
    resetDeleteState,
    deleteUser,
  };
};
