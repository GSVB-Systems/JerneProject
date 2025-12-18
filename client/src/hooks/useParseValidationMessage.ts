import { useCallback } from "react";
import { ApiException } from "../models/ServerAPI.ts";

type ValidationPayload = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

export const useParseValidationMessage = (fallbackMessage = "Network error") => {
  return useCallback((cause: unknown): string => {
    if (cause instanceof ApiException) {
      try {
        const payload = JSON.parse(cause.response ?? "null") as ValidationPayload | null;
        const fieldError = Object.values(payload?.errors ?? {})
          .flat()
          .find((message) => message?.trim());
        return fieldError ?? payload?.detail ?? payload?.title ?? cause.message;
      } catch {
        return cause.message;
      }
    }

    if (cause instanceof Error && cause.message.trim()) return cause.message;
    if (typeof cause === "string" && cause.trim()) return cause;
    return fallbackMessage;
  }, [fallbackMessage]);
};
