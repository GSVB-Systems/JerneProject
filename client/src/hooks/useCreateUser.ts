import { useState } from "react";
import type { RegisterUserDto } from "../models/ServerAPI.ts";
import { userClient } from "../api-clients.ts";

export function useCreateUser(onSuccess?: () => void) {
  const [error, setError] = useState<string | null>(null);

  const createUser = async (dto: RegisterUserDto) => {
    setError(null);
    try {
      await userClient.create(dto);
      onSuccess?.();
    } catch {
      setError("Network error"); //
    }
  };

  return { error, createUser };
}