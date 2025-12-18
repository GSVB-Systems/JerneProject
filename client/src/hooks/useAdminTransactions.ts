import { useCallback, useEffect, useMemo, useState } from "react";
import { transactionClient, userClient } from "../api-clients.ts";
import type {
  FileResponse,
  PagedResultOfTransactionDto,
  TransactionDto,
  UserDto,
} from "../models/ServerAPI";
import type {
  SortDirection,
  TransactionSortField,
  TransactionTypeFilter,
} from "./useUserTransactions.ts";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

const DEFAULT_PAGE_SIZE = 25;
const DEFAULT_SORT_FIELD: TransactionSortField = "transactionDate";
const DEFAULT_SORT_DIRECTION: SortDirection = "desc";
const USERS_PAGE_SIZE = 25;
const USER_FETCH_SORT = "firstname";

export type AdminTransactionRow = TransactionDto & { userFullName: string };
export type TransactionPendingTypeFilter = "all" | "bogført" | "afventer";


type UseAdminTransactionsResult = {
  transactions: AdminTransactionRow[];
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
  acceptTransaction: (transaction: TransactionDto) => Promise<void>;
  deleteTransaction: (transaction: TransactionDto) => Promise<void>;
  userNamesLoaded: boolean;
  pendingTypeFilter: TransactionPendingTypeFilter;
  setPendingTypeFilter: (value: TransactionPendingTypeFilter) => void;
};

const sanitizeSearchTerm = (value: string): string | null => {
  const normalized = value.trim().replace(/[|,"]+/g, " ").replace(/\s+/g, " ").trim();
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

const buildSieveFilters = (
    search: string,
    type: TransactionTypeFilter,
    pending: TransactionPendingTypeFilter,
): string | null => {
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

  // ➕ ADD PENDING TYPE FILTER
  if (pending === "bogført") {
    parts.push("pending==false");
  } else if (pending === "afventer") {
    parts.push("pending==true");
  }

  return parts.length ? parts.join(",") : null;
};


const parsePagedTransactions = async (
  response: FileResponse | null | undefined,
): Promise<PagedResultOfTransactionDto> => {
  if (!response) {
    throw new Error("Tomt svar fra serveren.");
  }

  const payload = await response.data.text();
  if (!payload) {
    return { items: [], totalCount: 0, page: 1, pageSize: DEFAULT_PAGE_SIZE };
  }

  return JSON.parse(payload) as PagedResultOfTransactionDto;
};

const buildUserFullName = (user: UserDto): string => {
  const fullName = `${user.firstname ?? ""} ${user.lastname ?? ""}`.trim();
  return fullName || user.email || "Ukendt bruger";
};

const chunkArray = <T,>(items: T[], size: number): T[][] => {
  const result: T[][] = [];
  for (let i = 0; i < items.length; i += size) {
    result.push(items.slice(i, i + size));
  }
  return result;
};

const buildUserIdFilter = (ids: string[]): string | null => {
  if (!ids.length) return null;
  return ids
    .map((id) => `userID==${JSON.stringify(id)}`)
    .join("|");
};

export const useAdminTransactions = (): UseAdminTransactionsResult => {
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [userNameMap, setUserNameMap] = useState<Map<string, string>>(new Map());
  const [userNamesLoaded, setUserNamesLoaded] = useState(false);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [sortField, setSortFieldState] = useState<TransactionSortField>(DEFAULT_SORT_FIELD);
  const [sortDirection, setSortDirectionState] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);
  const [searchTerm, setSearchTermState] = useState("");
  const [transactionTypeFilter, setTransactionTypeFilterState] = useState<TransactionTypeFilter>("all");
  const [pendingTypeFilter, setPendingTypeFilter] = useState<TransactionPendingTypeFilter>("all");

  const parseValidationMessage = useParseValidationMessage("Kunne ikke hente transaktioner.");


  const filters = useMemo(
      () => buildSieveFilters(searchTerm, transactionTypeFilter, pendingTypeFilter),
      [searchTerm, transactionTypeFilter, pendingTypeFilter],
  );


  const sorts = useMemo(
    () => `${sortDirection === "desc" ? "-" : ""}${sortField}`,
    [sortDirection, sortField],
  );

  const fetchAdminTransactions = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await transactionClient.getAll(filters, sorts, page, pageSize);
      const parsed = await parsePagedTransactions(response);
      const list = Array.isArray(parsed?.items) ? parsed.items : [];
      const totalCount = coerceNumber(parsed?.totalCount, list.length);
      const serverPageSize = coerceNumber(parsed?.pageSize, DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
      const serverPage = coerceNumber(parsed?.page, page) || 1;

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
  }, [filters, page, pageSize, sorts, parseValidationMessage]);

  const fetchUsersByIds = useCallback(
    async (userIds: Array<string | undefined>) => {
      const uniqueIds = Array.from(new Set(userIds.filter((id): id is string => Boolean(id))));
      const missingIds = uniqueIds.filter((id) => !userNameMap.has(id));

      if (!missingIds.length) {
        setUserNamesLoaded(true);
        return;
      }

      setUserNamesLoaded(false);
      const nextMap = new Map(userNameMap);

      try {
        const chunks = chunkArray(missingIds, USERS_PAGE_SIZE);
        for (const chunk of chunks) {
          const filter = buildUserIdFilter(chunk);
          if (!filter) continue;

          const response = await userClient.getAll(filter, USER_FETCH_SORT, 1, chunk.length);
          const items = Array.isArray(response?.items) ? response.items : [];
          items.forEach((user) => {
            if (user?.userID) {
              nextMap.set(user.userID, buildUserFullName(user));
            }
          });
        }

        setUserNameMap(nextMap);
      } catch (err) {
        console.error("Kunne ikke hente brugere til opslag.", err);
      } finally {
        setUserNamesLoaded(true);
      }
    },
    [userNameMap],
  );

  const acceptTransaction = useCallback(async (transactionDTO: TransactionDto): Promise<void> => {
    try {
      if (!transactionDTO.transactionID) {
        throw new Error("Transaktion mangler ID");
      }
      await transactionClient.update(transactionDTO.transactionID,{
        ...transactionDTO,
        pending: false,
      });
      await fetchAdminTransactions();
    } catch (err) {
      console.error("Kunne ikke acceptere transaktionen", err);
      throw err;
    }
  }, [fetchAdminTransactions]);


  const deleteTransaction = useCallback(async (transactionDTO: TransactionDto): Promise<void> => {
    try {
      await transactionClient.delete(transactionDTO.transactionID ?? "");
      await fetchAdminTransactions();
    } catch (err) {
      console.error("Kunne ikke slette transaktionen", err);
      throw err;
    }
  }, [fetchAdminTransactions]);

  useEffect(() => {
    void fetchAdminTransactions();
  }, [fetchAdminTransactions]);

  useEffect(() => {
    if (!transactions.length) {
      setUserNamesLoaded(true);
      return;
    }

    void fetchUsersByIds(transactions.map((tx) => tx.userID));
  }, [transactions, fetchUsersByIds]);

  const totalPages = useMemo(
    () => Math.max(1, Math.ceil(total / pageSize) || 1),
    [pageSize, total],
  );

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
    [sortField],
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
    [totalPages],
  );

  const resetFilters = useCallback(() => {
    setSearchTermState("");
    setPendingTypeFilter("all");
    setTransactionTypeFilterState("all");
    setSortFieldState(DEFAULT_SORT_FIELD);
    setSortDirectionState(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

  const transactionsWithNames = useMemo<AdminTransactionRow[]>(
    () =>
      transactions.map((tx) => ({
        ...tx,
        userFullName: tx.userID ? userNameMap.get(tx.userID) ?? "Ukendt bruger" : "Ukendt bruger",
      })),
    [transactions, userNameMap],
  );

  return {
    transactions: transactionsWithNames,
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
    pendingTypeFilter,
    sortField,
    sortDirection,
    setSearchTerm,
    setTransactionTypeFilter,
    setPendingTypeFilter,
    setSortField,
    toggleSortDirection,
    toggleSort,
    handlePageChange,
    resetFilters,
    refresh: fetchAdminTransactions,
    acceptTransaction,
    deleteTransaction,
    userNamesLoaded,
  };
};

export type { TransactionSortField, TransactionTypeFilter } from "./useUserTransactions.ts";
