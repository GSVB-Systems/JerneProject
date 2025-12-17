import { useCallback, useEffect, useMemo, useState } from "react";
import { boardClient } from "../api-clients.ts";
import type { BoardDto } from "../models/ServerAPI";
import { useJWT } from "./useJWT";

export type BoardSortField = "createdAt" | "boardSize";
type SortDirection = "asc" | "desc";

const DEFAULT_PAGE_SIZE = 10;
const DEFAULT_SORT_FIELD: BoardSortField = "createdAt";
const DEFAULT_SORT_DIRECTION: SortDirection = "desc";

const coerceNumber = (value: unknown, fallback = 0): number => {
  if (typeof value === "number" && !Number.isNaN(value)) return value;
  if (typeof value === "string") {
    const parsed = Number(value);
    if (!Number.isNaN(parsed)) return parsed;
  }
  return fallback;
};

const sanitizeSearchTerm = (value: string): string | null => {
  const normalized = value.trim().replace(/[|,"]/g, " ").replace(/\s+/g, " ").trim();
  if (!normalized) return null;
  return normalized.includes(" ") ? `"${normalized}"` : normalized;
};

const buildSieveFilters = (searchTerm: string): string | null => {
  const filters: string[] = [];
  const sanitizedSearch = sanitizeSearchTerm(searchTerm);
  if (sanitizedSearch) {
    const numericCandidate = sanitizedSearch.replace(/"/g, "");
    if (!Number.isNaN(Number(numericCandidate))) {
      filters.push(`numbers.number==${numericCandidate}`);
    } else {
      filters.push(`boardID@=${sanitizedSearch}`);
    }
  }
  return filters.length ? filters.join(",") : null;
};

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

export const useUserBoardHistory = () => {
  const [boards, setBoards] = useState<BoardDto[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [searchTerm, setSearchTermState] = useState("");
  const [sortField, setSortFieldState] = useState<BoardSortField>(DEFAULT_SORT_FIELD);
  const [sortDirection, setSortDirection] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);

  const jwt = useJWT();
  const userId = useMemo(() => getUserIdFromJwt(jwt), [jwt]);

  const filters = useMemo(() => buildSieveFilters(searchTerm), [searchTerm]);
  const sorts = useMemo(() => `${sortDirection === "desc" ? "-" : ""}${sortField}`, [sortField, sortDirection]);

  const fetchBoards = useCallback(async () => {
    if (!userId) {
      setError("Ingen gyldig bruger-session.");
      setBoards([]);
      setLoading(false);
      setHasLoadedOnce(true);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const response = await boardClient.getAllBoardsByUserId(userId, filters ?? undefined, sorts ?? undefined, page, pageSize);
      const list = Array.isArray(response?.items) ? response.items : [];
      const totalCount = coerceNumber(response?.totalCount, list.length);
      const serverPageSize = coerceNumber(response?.pageSize, DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
      const serverPage = coerceNumber(response?.page, page) || 1;

      setBoards(list);
      setTotal(totalCount);
      setPageSize(serverPageSize);
      setPage(serverPage);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Kunne ikke hente boards.");
      setBoards([]);
      setTotal(0);
    } finally {
      setLoading(false);
      setHasLoadedOnce(true);
    }
  }, [filters, page, pageSize, sorts, userId]);

  useEffect(() => {
    void fetchBoards();
  }, [fetchBoards]);

  const totalPages = useMemo(() => Math.max(1, Math.ceil(total / pageSize) || 1), [pageSize, total]);

  useEffect(() => {
    if (page > totalPages) {
      setPage(totalPages);
    }
  }, [page, totalPages]);

  const setSearchTerm = useCallback((value: string) => {
    setSearchTermState(value);
    setPage(1);
  }, []);

  const setSortField = useCallback((value: BoardSortField) => {
    setSortFieldState(value);
    setSortDirection(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const toggleSortDirection = useCallback(() => {
    setSortDirection(prev => prev === "asc" ? "desc" : "asc");
    setPage(1);
  }, []);

  const handlePageChange = useCallback((direction: "prev" | "next") => {
    setPage(current => {
      if (direction === "prev") {
        return Math.max(1, current - 1);
      }
      return Math.min(totalPages, current + 1);
    });
  }, [totalPages]);

  const resetFilters = useCallback(() => {
    setSearchTermState("");
    setSortFieldState(DEFAULT_SORT_FIELD);
    setSortDirection(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

  return {
    boards,
    total,
    page,
    pageSize,
    totalPages,
    visibleStart,
    visibleEnd,
    loading,
    hasLoadedOnce,
    error,
    searchTerm,
    sortField,
    sortDirection,
    setSearchTerm,
    setSortField,
    toggleSortDirection,
    handlePageChange,
    resetFilters,
    refresh: fetchBoards,
  };
};
