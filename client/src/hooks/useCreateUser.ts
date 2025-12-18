import { useState } from "react";
import type { RegisterUserDto } from "../models/ServerAPI.ts";
import { userClient } from "../api-clients.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

const createInitialFormState = (): RegisterUserDto => ({
  firstname: "",
  lastname: "",
  email: "",
  password: "",
  role: "",
});

export function useCreateUser(onSuccess?: () => void) {
  const [error, setError] = useState<string | null>(null);
  const [formValues, setFormValues] = useState<RegisterUserDto>(createInitialFormState());
  const parseValidationMessage = useParseValidationMessage();

  const updateField = (field: keyof RegisterUserDto, value: string) => {
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
    try {
      await userClient.create(dto ?? formValues);
      resetForm();
      onSuccess?.();
    } catch (cause) {
      setError(parseValidationMessage(cause));
    }
  };

  return { error, formValues, updateField, resetForm, createUser };
}