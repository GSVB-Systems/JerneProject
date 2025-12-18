import { useState } from "react";
import type { RegisterUserDto } from "../models/ServerAPI.ts";
import { userClient } from "../api-clients.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";


const createInitialFormState = (): CreateUserFormValues => ({
  firstname: "",
  lastname: "",
  email: "",
  password: "",
  confirmPassword: "",
  role: "",
});

export function useCreateUser(onSuccess?: () => void) {
  const [error, setError] = useState<string | null>(null);
  const [formValues, setFormValues] = useState<CreateUserFormValues>(createInitialFormState());
  const parseValidationMessage = useParseValidationMessage();

  const updateField = (field: keyof CreateUserFormValues, value: string) => {
    setFormValues((previous) => ({
      ...previous,
      [field]: value,
    }));
  };

  const resetForm = () => {
    setFormValues(createInitialFormState());
  };

  const createUser = async (dto?: RegisterUserDto) => {
    setError(null);

    // Front-end only password confirmation check
    if (formValues.password !== formValues.confirmPassword) {
      setError("Adgangskoderne matcher ikke.");
      return;
    }

    try {
      const { ...payload } = dto ?? formValues;
      const normalizedPayload = { ...payload, email: payload.email.toLowerCase() };
      await userClient.create(normalizedPayload);
      resetForm();
      onSuccess?.();
    } catch (cause) {
      setError(parseValidationMessage(cause));
    }
  };

  return { error, formValues, updateField, resetForm, createUser };
}

export type CreateUserFormValues = RegisterUserDto & { confirmPassword: string };
