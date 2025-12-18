import { useCallback, useEffect, useMemo, useState } from "react";
import { boardMatcherClient, winningBoardClient } from "../api-clients.ts";
import type { WinnerResultDto, WinningBoardDto } from "../models/ServerAPI.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

const DEFAULT_SORT = "-createdAt";
const DEFAULT_PAGE_SIZE = 10;

interface WinningBoardWithMatches extends WinningBoardDto {
    matchingBoards: WinnerResultDto[];
    matchesLoaded: boolean;
}

export const useWinningBoards = () => {
    const [winningBoards, setWinningBoards] = useState<WinningBoardWithMatches[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [activeBoardId, setActiveBoardId] = useState<string | null>(null);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(DEFAULT_PAGE_SIZE);
    const [total, setTotal] = useState(0);

    const parseValidationMessage = useParseValidationMessage("Kunne ikke hente vindende boards.");

    const fetchWinningBoards = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const payload = await winningBoardClient.getAll(undefined, DEFAULT_SORT, page, pageSize);
            const boards = Array.isArray(payload?.items) ? payload.items : [];
            setWinningBoards(boards.map((board) => ({ ...board, matchingBoards: [], matchesLoaded: false })));
            setTotal(payload?.totalCount ?? boards.length);
        } catch (err) {
            setError(parseValidationMessage(err));
        } finally {
            setLoading(false);
        }
    }, [page, pageSize, parseValidationMessage]);

    useEffect(() => {
        void fetchWinningBoards();
    }, [fetchWinningBoards]);

    const fetchMatchingBoards = useCallback(async (winningBoardId: string) => {
        try {
            const result = await boardMatcherClient.getBoardsContainingNumbers(winningBoardId);
            setWinningBoards((prev) => prev.map((board) => board.winningBoardID === winningBoardId
                ? { ...board, matchingBoards: result ?? [], matchesLoaded: true }
                : board));
        } catch (cause) {
            setError(parseValidationMessage(cause));
        }
    }, [parseValidationMessage]);

    const toggleBoard = useCallback((winningBoardId: string) => {
        setActiveBoardId((prev) => {
            if (prev === winningBoardId) return null;
            const target = winningBoards.find((board) => board.winningBoardID === winningBoardId);
            if (target && !target.matchesLoaded) {
                void fetchMatchingBoards(winningBoardId);
            }
            return winningBoardId;
        });
    }, [fetchMatchingBoards, winningBoards]);

    useEffect(() => {
        if (!activeBoardId) return;
        if (!winningBoards.some((board) => board.winningBoardID === activeBoardId)) {
            setActiveBoardId(null);
        }
    }, [activeBoardId, winningBoards]);

    const totalPages = useMemo(() => Math.max(1, Math.ceil(total / pageSize) || 1), [pageSize, total]);
    const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
    const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

    const handlePageChange = useCallback((direction: "prev" | "next") => {
        setPage((current) => {
            if (direction === "prev") {
                return Math.max(1, current - 1);
            }
            return Math.min(totalPages, current + 1);
        });
    }, [totalPages]);

    const goToPage = useCallback((nextPage: number) => {
        setPage((current) => {
            const normalized = Math.min(Math.max(1, Math.floor(nextPage)), totalPages);
            if (normalized === current) return current;
            return normalized;
        });
    }, [totalPages]);

    const activeBoardMatches = useMemo(() => winningBoards.find((board) => board.winningBoardID === activeBoardId)?.matchingBoards ?? [], [activeBoardId, winningBoards]);

    return {
        winningBoards,
        loading,
        error,
        activeBoardId,
        activeBoardMatches,
        toggleBoard,
        refresh: fetchWinningBoards,
        page,
        pageSize,
        total,
        totalPages,
        visibleStart,
        visibleEnd,
        handlePageChange,
        goToPage,
    };
};
