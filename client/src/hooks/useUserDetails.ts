import { useCallback, useEffect, useRef, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { userClient } from "../api-clients.ts";
import type { UserDto, UserRole } from "../models/ServerAPI";

export type UserFormState = {
  firstname: string;
  lastname: string;
  email: string;
  role: UserRole | undefined;
  firstlogin: boolean;
  isActive: boolean;
  balance: number;
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

  const mapUserToFormState = useCallback((payload: UserDto): UserFormState => ({
    firstname: payload.firstname ?? "",
    lastname: payload.lastname ?? "",
    email: payload.email ?? "",
    role: payload.role,
    firstlogin: payload.firstlogin ?? false,
    isActive: payload.isActive ?? false,
    balance: payload.balance ?? 0,
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
      setError(err instanceof Error ? err.message : "Ukendt fejl ved hentning af bruger.");
      setUser(null);
      setFormState(null);
      setEditing(false);
      setDeleteError(null);
    } finally {
      if (isMounted.current) {
        setLoading(false);
      }
    }
  }, [mapUserToFormState, userId]);

  const beginEdit = useCallback(() => {
    if (!formState) return;
    setEditing(true);
    setSaveMessage(null);
    setSaveError(null);
  }, [formState]);

  const cancelEdit = useCallback(() => {
    setEditing(false);
    setFormState(user ? mapUserToFormState(user) : null);
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

    setSaving(true);
    setSaveMessage(null);
    setSaveError(null);
    try {
      const payload: UserDto = {
        userID: user?.userID,
        ...formState,
      };
      await userClient.update(userId, payload);
      await fetchUser();
      setSaveMessage("Brugeroplysninger opdateret.");
      setEditing(false);
    } catch (err) {
      setSaveError("Der opstod en fejl under opdatering af brugeroplysninger.");
    } finally {
      setSaving(false);
    }
  }, [fetchUser, formState, user?.userID, userId]);

  const deleteUser = useCallback(async (): Promise<boolean> => {
    if (!userId) return false;
    setDeleting(true);
    setDeleteError(null);
    try {
      await userClient.delete(userId);
      return true;
    } catch (err) {
      setDeleteError("Kunne ikke slette brugeren. Prøv igen.");
      return false;
    } finally {
      setDeleting(false);
    }
  }, [userId]);

  const resetDeleteState = useCallback(() => {
    setDeleteError(null);
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
