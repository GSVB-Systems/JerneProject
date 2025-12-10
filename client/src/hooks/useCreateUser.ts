import { useState } from "react";
import type { RegisterUserDto } from "../models/ServerAPI.ts";
import { userClient } from "../api-clients.ts";

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
    } catch {
      setError("Network error"); //
    }
  };

  return { error, formValues, updateField, resetForm, createUser };
}