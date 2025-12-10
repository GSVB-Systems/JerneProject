import { useCallback, useState } from "react";
import type { CreateTransactionDto } from "../models/ServerAPI.ts";
import { transactionClient } from "../api-clients.ts";

export type UseCreateTransactionResult = {
  amount: string;
  transactionString: string;
  setAmount: (value: string) => void;
  setTransactionString: (value: string) => void;
  createTransaction: () => Promise<boolean>;
  resetForm: () => void;
  error: string | null;
  isSubmitting: boolean;
};

const parseAmount = (value: string): number | null => {
  const normalized = value.replace(",", ".");
  const parsed = Number(normalized);
  return Number.isNaN(parsed) ? null : parsed;
};

export const useCreateTransaction = (): UseCreateTransactionResult => {
  const [amount, setAmount] = useState("");
  const [transactionString, setTransactionString] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const resetForm = useCallback(() => {
    setAmount("");
    setTransactionString("");
  }, []);

  const createTransaction = useCallback(async (): Promise<boolean> => {
    const parsedAmount = parseAmount(amount);
    if (parsedAmount === null) {
      setError("Ugyldigt beløb.");
      return false;
    }

    setIsSubmitting(true);
    setError(null);
    const dto: CreateTransactionDto = {
      amount: parsedAmount,
      transactionString: transactionString.trim() || undefined,
    };

    try {
      await transactionClient.create(dto);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : "Kunne ikke oprette transaktion.");
      return false;
    } finally {
      setIsSubmitting(false);
    }
  }, [amount, transactionString]);

  return {
    amount,
    transactionString,
    setAmount,
    setTransactionString,
    createTransaction,
    resetForm,
    error,
    isSubmitting,
  };
};

