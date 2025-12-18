import { useCallback, useEffect, useMemo, useState } from "react";
import { transactionClient } from "../api-clients.ts";
import type { TransactionDto } from "../models/ServerAPI";
import { useJWT } from "./useJWT.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

const DEFAULT_PAGE_SIZE = 25;
const DEFAULT_SORT_FIELD: TransactionSortField = "transactionDate";
const DEFAULT_SORT_DIRECTION: SortDirection = "desc";

type UseUserTransactionsResult = {
  transactions: TransactionDto[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  visibleStart: number;
  visibleEnd: number;
  loading: boolean;
  hasLoadedOnce: boolean;
  error: string | null;
  searchTerm: string;
  transactionTypeFilter: TransactionTypeFilter;
  sortField: TransactionSortField;
  sortDirection: SortDirection;
  setSearchTerm: (value: string) => void;
  setTransactionTypeFilter: (value: TransactionTypeFilter) => void;
  setSortField: (value: TransactionSortField) => void;
  toggleSortDirection: () => void;
  toggleSort: (field: TransactionSortField) => void;
  handlePageChange: (direction: "prev" | "next") => void;
  resetFilters: () => void;
  refresh: () => Promise<void>;
};

export type TransactionSortField = "transactionDate" | "amount" | "pending" | "transactionString";
export type TransactionTypeFilter = "all" | "credit" | "debit";
export type SortDirection = "asc" | "desc";

const sanitizeSearchTerm = (value: string): string | null => {
  const normalized = value.trim().replace(/[|,"]/g, " ").replace(/\s+/g, " ").trim();
  if (!normalized) return null;
  return normalized.includes(" ") ? `"${normalized}"` : normalized;
};

const coerceNumber = (value: unknown, fallback = 0): number => {
  if (typeof value === "number" && !Number.isNaN(value)) return value;
  if (typeof value === "string") {
    const parsed = Number(value);
    if (!Number.isNaN(parsed)) return parsed;
  }
  return fallback;
};

const buildSieveFilters = (search: string, type: TransactionTypeFilter): string | null => {
  const parts: string[] = [];
  const sanitized = sanitizeSearchTerm(search);
  if (sanitized) {
    parts.push(`transactionString@=${sanitized}`);
  }

  if (type === "credit") {
    parts.push("amount>0");
  } else if (type === "debit") {
    parts.push("amount<0");
  }

  return parts.length ? parts.join(",") : null;
};

function getUserIdFromJwt(jwt: string | null | undefined): string | null {
  if (!jwt) return null;
  try {
    const payloadBase64 = jwt.split(".")[1];
    const payloadJson = atob(payloadBase64);
    const payload = JSON.parse(payloadJson);
    return payload["sub"] ?? null;
  } catch {
    return null;
  }
}

export const useUserTransactions = (): UseUserTransactionsResult => {
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortField, setSortFieldState] = useState<TransactionSortField>(DEFAULT_SORT_FIELD);
  const [sortDirection, setSortDirectionState] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);
  const [searchTerm, setSearchTermState] = useState("");
  const [transactionTypeFilter, setTransactionTypeFilterState] = useState<TransactionTypeFilter>("all");

  const jwt = useJWT();
  const userId = useMemo(() => getUserIdFromJwt(jwt), [jwt]);
  const parseValidationMessage = useParseValidationMessage("Kunne ikke hente transaktioner.");

  const filters = useMemo(
    () => buildSieveFilters(searchTerm, transactionTypeFilter),
    [searchTerm, transactionTypeFilter]
  );

  const sorts = useMemo(
    () => `${sortDirection === "desc" ? "-" : ""}${sortField}`,
    [sortDirection, sortField]
  );

  const fetchUserTransactions = useCallback(async () => {
    if (!userId) {
      setError("Ingen gyldig bruger-session.");
      setTransactions([]);
      setLoading(false);
      setHasLoadedOnce(true);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const response = await transactionClient.getAllTransactionsByUserId(userId, filters, sorts, page, pageSize);
      const list = Array.isArray(response?.items) ? response.items : [];
      const totalCount = coerceNumber(response?.totalCount, list.length);
      const serverPageSize = coerceNumber(response?.pageSize, DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
      const serverPage = coerceNumber(response?.page, page) || 1;

      setTransactions(list);
      setTotal(totalCount);
      setPageSize(serverPageSize);
      setPage(serverPage);
    } catch (err) {
      setError(parseValidationMessage(err));
      setTransactions([]);
      setTotal(0);
    } finally {
      setLoading(false);
      setHasLoadedOnce(true);
    }
  }, [filters, page, pageSize, sorts, userId, parseValidationMessage]);

  useEffect(() => {
    void fetchUserTransactions();
  }, [fetchUserTransactions]);

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

  const setTransactionTypeFilter = useCallback((value: TransactionTypeFilter) => {
    setTransactionTypeFilterState(value);
    setPage(1);
  }, []);

  const setSortField = useCallback((value: TransactionSortField) => {
    setSortFieldState(value);
    setSortDirectionState(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const toggleSortDirection = useCallback(() => {
    setSortDirectionState((prev) => (prev === "asc" ? "desc" : "asc"));
    setPage(1);
  }, []);

  const toggleSort = useCallback(
    (field: TransactionSortField) => {
      if (field === sortField) {
        setSortDirectionState((prev) => (prev === "asc" ? "desc" : "asc"));
      } else {
        setSortFieldState(field);
        setSortDirectionState("asc");
      }
      setPage(1);
    },
    [sortField]
  );

  const handlePageChange = useCallback(
    (direction: "prev" | "next") => {
      setPage((current) => {
        if (direction === "prev") {
          return Math.max(1, current - 1);
        }
        return Math.min(totalPages, current + 1);
      });
    },
    [totalPages]
  );

  const resetFilters = useCallback(() => {
    setSearchTermState("");
    setTransactionTypeFilterState("all");
    setSortFieldState(DEFAULT_SORT_FIELD);
    setSortDirectionState(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

  return {
    transactions,
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
    transactionTypeFilter,
    sortField,
    sortDirection,
    setSearchTerm,
    setTransactionTypeFilter,
    setSortField,
    toggleSortDirection,
    toggleSort,
    handlePageChange,
    resetFilters,
    refresh: fetchUserTransactions,
  };
};
