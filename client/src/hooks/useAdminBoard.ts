import { useCallback, useMemo, useState } from "react";
import { winningBoardClient } from "../api-clients.ts";
import { useJWT } from "./useJWT.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

const BOARD_SIZE = 16;
const MAX_SELECTION = 5;
const MIN_SELECTION = 3;

interface UseAdminBoardResult {
    BOARD_SIZE: number;
    MAX_SELECTION: number;
    MIN_SELECTION: number;
    selected: number[];
    toggle: (num: number) => void;
    isValid: boolean;
    isSubmitting: boolean;
    error: string | null;
    success: boolean;
    createWinningBoard: () => Promise<boolean>;
}

export function useAdminBoard(): UseAdminBoardResult {
    const [selected, setSelected] = useState<number[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [success, setSuccess] = useState(false);

    const jwt = useJWT();
    const isAuthenticated = useMemo(() => Boolean(jwt), [jwt]);
    const parseValidationMessage = useParseValidationMessage("Kunne ikke oprette vindende numre.");

    const toggle = useCallback((num: number) => {
        setSelected((prev) => {
            if (prev.includes(num)) {
                return prev.filter((n) => n !== num);
            }
            if (prev.length >= MAX_SELECTION) {
                return prev;
            }
            return [...prev, num];
        });
    }, []);

    const createWinningBoard = useCallback(async (): Promise<boolean> => {
        setError(null);
        setSuccess(false);

        if (!isAuthenticated) {
            setError("Du er ikke logget ind.");
            return false;
        }
        if (selected.length < MIN_SELECTION || selected.length > MAX_SELECTION) {
            setError("Vælg mellem 3 og 5 numre.");
            return false;
        }

        setIsSubmitting(true);
        try {
            await winningBoardClient.create({ winningNumbers: selected });
            setSuccess(true);
            setSelected([]);
            return true;
        } catch (err) {
            setError(parseValidationMessage(err));
            return false;
        } finally {
            setIsSubmitting(false);
        }
    }, [isAuthenticated, selected, parseValidationMessage]);

    const isValid = selected.length >= MIN_SELECTION && selected.length <= MAX_SELECTION;

    return {
        BOARD_SIZE,
        MAX_SELECTION,
        MIN_SELECTION,
        selected,
        toggle,
        isValid,
        isSubmitting,
        error,
        success,
        createWinningBoard,
    };
}
