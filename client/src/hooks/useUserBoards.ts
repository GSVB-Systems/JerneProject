import { useCallback, useMemo, useState } from "react";
import {useJWT} from "./useJWT.ts";

const PRICE_CONFIG: Record<number, number> = {
    5: 20,
    6: 40,
    7: 80,
    8: 160,
};

const MAX_SELECTION = 8;
const MIN_SELECTION = 5;
const BOARD_SIZE = 16;

interface UseUserBoardsResult {
    selected: number[];
    toggle: (num: number) => void;
    value: string;
    setValue: (val: string) => void;
    options: string[];
    getPrice: () => string;
    isValid: boolean;
    MAX_SELECTION: number;
    MIN_SELECTION: number;
    BOARD_SIZE: number;
    error: string | null;
}

export function useUserBoards(): UseUserBoardsResult {
    const [selected, setSelected] = useState<number[]>([]);
    const [value, setValue] = useState("");
    const [error, setError] = useState<string | null>(null);
    const options = useMemo(() => ["1", "2", "3", "4", "5"], []);

    const jwt = useJWT();
    const userId = useMemo(() => getUserIdFromJwt(jwt), [jwt]);


    const toggle = useCallback((num: number) => {
        setSelected((prev) => {
            if (prev.includes(num)) {
                return prev.filter((n) => n !== num);
            }
            if (prev.length < MAX_SELECTION) {
                return [...prev, num];
            }
            return prev;
        });
    }, []);

    const getUserIdFromJwt = (jwt: string | null | undefined): string | null => {
        if (!jwt) return null;
        try {
            const payloadBase64 = jwt.split(".")[1];
            const payloadJson = atob(payloadBase64);
            const payload = JSON.parse(payloadJson);
            return payload["sub"] ?? null;
        } catch {
            return null;
        }
    };

    const getPrice = useCallback((): string => {
        if (selected.length < MIN_SELECTION) return "—";
        return PRICE_CONFIG[selected.length]?.toString() || "—";
    }, [selected.length]);

    const createBoard = useCallback(async (): Promise<boolean> => {

        const selectedNumbers = selected;
        const repeatingWeeks = options;
        const boardsize = selectedNumbers.length;

        if(boardsize < MIN_SELECTION){
            setError("Du skal vælge mindst 5 numre");
            return false;
        }

    })

    const isValid = selected.length >= MIN_SELECTION && selected.length <= MAX_SELECTION;

    return {
        selected,
        toggle,
        value,
        setValue,
        options,
        getPrice,
        isValid,
        MAX_SELECTION,
        MIN_SELECTION,
        BOARD_SIZE,
        error,
    };
}

