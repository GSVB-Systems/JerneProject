import { useCallback, useEffect, useMemo, useState } from "react";
import { userClient } from "../api-clients.ts";
import type { UserDto } from "../models/ServerAPI";


const DEFAULT_PAGE_SIZE = 25;
const DEFAULT_SORT_FIELD: SortField = "firstname";
const DEFAULT_SORT_DIRECTION: SortDirection = "asc";

export type StatusFilter = "all" | "active" | "inactive";
export type RoleFilter = "all" | "admin" | "user";
export type FirstLoginFilter = "all" | "yes" | "no";
export type SortField = "firstname" | "email" | "role" | "firstlogin" | "isActive" | "balance";
export type SortDirection = "asc" | "desc";

type UseUsersResult = {
  users: UserDto[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  visibleStart: number;
  visibleEnd: number;
  loading: boolean;
  error: string | null;
  searchTerm: string;
  statusFilter: StatusFilter;
  roleFilter: RoleFilter;
  firstLoginFilter: FirstLoginFilter;
  sortField: SortField;
  sortDirection: SortDirection;
  setSearchTerm: (value: string) => void;
  setStatusFilter: (value: StatusFilter) => void;
  setRoleFilter: (value: RoleFilter) => void;
  setFirstLoginFilter: (value: FirstLoginFilter) => void;
  setSortField: (value: SortField) => void;
  toggleSortDirection: () => void;
  toggleSort: (field: SortField) => void;
  handlePageChange: (direction: "prev" | "next") => void;
  resetFilters: () => void;
  refresh: () => Promise<void>;
};

const coerceNumber = (value: unknown, fallback = 0): number => {
  if (typeof value === "number" && !Number.isNaN(value)) return value;
  if (typeof value === "string") {
    const parsed = Number(value);
    if (!Number.isNaN(parsed)) return parsed;
  }
  return fallback;
};

const extractUsersPayload = (payload: unknown, fallbackLength = 0): { list: UserDto[]; total: number } => {
  if (Array.isArray(payload)) {
    return { list: payload as UserDto[], total: payload.length };
  }

  if (payload && typeof payload === "object") {
    const container = payload as Record<string, unknown>;
    const candidate = container.items ?? container.data ?? container.results ?? container.users;
    const list = Array.isArray(candidate) ? (candidate as UserDto[]) : [];
    const total = coerceNumber(
      container.total ?? container.totalCount ?? container.count ?? container.totalItems ?? (list.length || fallbackLength),
      list.length || fallbackLength,
    );
    return { list, total };
  }

  return { list: [], total: fallbackLength };
};

const buildSieveFilters = (
  search: string,
  status: StatusFilter,
  role: RoleFilter,
  firstLogin: FirstLoginFilter,
): string | null => {
  const parts: string[] = [];
  const sanitized = search.trim().replace(/[|,]/g, " ");
  if (sanitized) {
    parts.push(`firstname@=*${sanitized}*|lastname@=*${sanitized}*|email@=*${sanitized}*`);
  }

  if (status === "active") {
    parts.push("isActive==true");
  } else if (status === "inactive") {
    parts.push("isActive==false");
  }

  if (role === "admin") {
    parts.push("role==1");
  } else if (role === "user") {
    parts.push("role==0");
  }

  if (firstLogin === "yes") {
    parts.push("firstlogin==true");
  } else if (firstLogin === "no") {
    parts.push("firstlogin==false");
  }

  return parts.length ? parts.join(",") : null;
};

export const useUsers = (): UseUsersResult => {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [page, setPage] = useState(1);
  const pageSize = DEFAULT_PAGE_SIZE;
  const [sortField, setSortFieldState] = useState<SortField>(DEFAULT_SORT_FIELD);
  const [sortDirection, setSortDirectionState] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);
  const [searchTerm, setSearchTermState] = useState("");
  const [statusFilter, setStatusFilterState] = useState<StatusFilter>("all");
  const [roleFilter, setRoleFilterState] = useState<RoleFilter>("all");
  const [firstLoginFilter, setFirstLoginFilterState] = useState<FirstLoginFilter>("all");

  const filters = useMemo(
    () => buildSieveFilters(searchTerm, statusFilter, roleFilter, firstLoginFilter),
    [firstLoginFilter, roleFilter, searchTerm, statusFilter],
  );

  const sorts = useMemo(
    () => `${sortDirection === "desc" ? "-" : ""}${sortField}`,
    [sortDirection, sortField],
  );

  const fetchUsers = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await userClient.getAll(filters, sorts, page, pageSize);
      const textData = await response.data?.text();
      const payload = textData ? JSON.parse(textData) : null;
      const { list, total: totalCount } = extractUsersPayload(payload, pageSize);
      setUsers(list);
      setTotal(totalCount);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load users.");
      setUsers([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [filters, page, pageSize, sorts]);

  useEffect(() => {
    void fetchUsers();
  }, [fetchUsers]);

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

  const setStatusFilter = useCallback((value: StatusFilter) => {
    setStatusFilterState(value);
    setPage(1);
  }, []);

  const setRoleFilter = useCallback((value: RoleFilter) => {
    setRoleFilterState(value);
    setPage(1);
  }, []);

  const setFirstLoginFilter = useCallback((value: FirstLoginFilter) => {
    setFirstLoginFilterState(value);
    setPage(1);
  }, []);

  const setSortField = useCallback((value: SortField) => {
    setSortFieldState(value);
    setSortDirectionState(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const toggleSortDirection = useCallback(() => {
    setSortDirectionState((prev) => (prev === "asc" ? "desc" : "asc"));
    setPage(1);
  }, []);

  const toggleSort = useCallback((field: SortField) => {
    if (field === sortField) {
      setSortDirectionState((prev) => (prev === "asc" ? "desc" : "asc"));
    } else {
      setSortFieldState(field);
      setSortDirectionState("asc");
    }
    setPage(1);
  }, [sortField]);

  const handlePageChange = useCallback((direction: "prev" | "next") => {
    setPage((current) => {
      if (direction === "prev") {
        return Math.max(1, current - 1);
      }
      return Math.min(totalPages, current + 1);
    });
  }, [totalPages]);

  const resetFilters = useCallback(() => {
    setSearchTermState("");
    setStatusFilterState("all");
    setRoleFilterState("all");
    setFirstLoginFilterState("all");
    setSortFieldState(DEFAULT_SORT_FIELD);
    setSortDirectionState(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

  return {
    users,
    total,
    page,
    pageSize,
    totalPages,
    visibleStart,
    visibleEnd,
    loading,
    error,
    searchTerm,
    statusFilter,
    roleFilter,
    firstLoginFilter,
    sortField,
    sortDirection,
    setSearchTerm,
    setStatusFilter,
    setRoleFilter,
    setFirstLoginFilter,
    setSortField,
    toggleSortDirection,
    toggleSort,
    handlePageChange,
    resetFilters,
    refresh: fetchUsers,
  };
};
