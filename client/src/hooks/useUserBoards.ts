import { useCallback, useMemo, useState } from "react";
import {useJWT} from "./useJWT.ts";
import type {CreateBoardDto, CreateTransactionDto} from "../models/ServerAPI.ts";
import {boardClient, transactionClient} from "../api-clients.ts";
import {useBalance} from "./useNavbar.ts";

const PRICE_CONFIG: Record<number, number> = {
    5: 20,
    6: 40,
    7: 80,
    8: 160,
};

const MAX_SELECTION = 8;
const MIN_SELECTION = 5;
const BOARD_SIZE = 16;

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

interface UseUserBoardsResult {
    selected: number[];
    toggle: (num: number) => void;
    value: string;
    setValue: (val: string) => void;
    getPrice: () => number;
    isValid: boolean;
    MAX_SELECTION: number;
    MIN_SELECTION: number;
    BOARD_SIZE: number;
    error: string | null;
    createBoardTransaction: () => Promise<boolean>;
    createBoard: () => Promise<boolean>;
}

export function useUserBoards(): UseUserBoardsResult {
    const [selected, setSelected] = useState<number[]>([]);
    const [value, setValue] = useState("");
    const [error, setError] = useState<string | null>(null);

    const jwt = useJWT();
    const userId = useMemo(() => getUserIdFromJwt(jwt), [jwt]) ?? "";



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



    const getPrice = useCallback((): number => {


        const basePrice = PRICE_CONFIG[selected.length];


        const weeks = Number.parseInt(value || "1", 10);
        return (basePrice * weeks);
    }, [selected.length, value]);

    const { loadUserBalance } = useBalance();


    const createBoardTransaction = useCallback(async (): Promise<boolean> => {
        const price = getPrice();
        const currentBalance = await loadUserBalance();

        if (currentBalance === undefined) {
            setError("Kunne ikke hente saldo.");
            return false;
        }

        if (price > currentBalance) {
            setError("Du har ikke tilstrækkelig penge til at købe dette bræt.");
            return false;
        }

        const dto: CreateTransactionDto = {
            transactionString: "GUID",
            amount: -Math.abs(price),
            userID: userId,
            pending: false,
        };

        try {
            await transactionClient.create(dto);
            return await createBoard();

        } catch (err) {
            setError("Fejl ved oprettelse af transaktion.");
            return false;
        }
    }, [getPrice, loadUserBalance, userId]);


    const createBoard = useCallback(async (): Promise<boolean> => {

        const selectedNumbers = selected;
        const repeatingWeeks = Number.parseInt(value || "1", 10);
        const boardSize = selectedNumbers.length;

        if(boardSize < MIN_SELECTION){
            setError("Du skal vælge mindst 5 numre");
            return false;
        }

        const dto: CreateBoardDto = {
            boardSize,
            week: repeatingWeeks,
            userID: userId,
            numbers: selectedNumbers,

        };

        try{
            await boardClient.create(dto);
            return true;
        } catch (err) {
            setError("Fejl ved oprettelse af spillebræt.");
            return false;
        }

    }, [selected, value, userId]);

    const isValid = selected.length >= MIN_SELECTION && selected.length <= MAX_SELECTION;

    return {
        selected,
        toggle,
        value,
        setValue,
        getPrice,
        isValid,
        MAX_SELECTION,
        MIN_SELECTION,
        BOARD_SIZE,
        error,
        createBoardTransaction,
        createBoard,
    };
}

