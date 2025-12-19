import { useCallback, useEffect, useMemo, useState } from "react";
import { boardClient, userClient } from "../api-clients.ts";
import type { BoardDto, FileResponse, PagedResultOfBoardDto, UserDto } from "../models/ServerAPI";
import { useParseValidationMessage } from "./useParseValidationMessage.ts";

type SortDirection = "asc" | "desc";

const DEFAULT_PAGE_SIZE = 10;
const PRIMARY_SORT_FIELD = "year";
const SECONDARY_SORT_FIELD = "week";
const DEFAULT_SORT_DIRECTION: SortDirection = "desc";
const USERS_PAGE_SIZE = 25;
const USER_FETCH_SORT = "firstname";

const coerceNumber = (value: unknown, fallback = 0): number => {
  if (typeof value === "number" && !Number.isNaN(value)) return value;
  if (typeof value === "string") {
    const parsed = Number(value);
    if (!Number.isNaN(parsed)) return parsed;
  }
  return fallback;
};

const sanitizeSearchTerm = (value: string): string | null => {
  const normalized = value.trim().replace(/[|,"]+/g, " ").replace(/\s+/g, " ").trim();
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

const parsePagedBoards = async (
  response: FileResponse | null | undefined,
): Promise<PagedResultOfBoardDto> => {
  if (!response) {
    throw new Error("Tomt svar fra serveren.");
  }

  const payload = await response.data.text();
  if (!payload) {
    return { items: [], totalCount: 0, page: 1, pageSize: DEFAULT_PAGE_SIZE };
  }

  return JSON.parse(payload) as PagedResultOfBoardDto;
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
  return ids.map((id) => `userID==${JSON.stringify(id)}`).join("|");
};

export const useAdminBoardHistory = () => {
  const [boards, setBoards] = useState<BoardDto[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [hasLoadedOnce, setHasLoadedOnce] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);
  const [searchTerm, setSearchTermState] = useState("");
  const [sortDirection, setSortDirection] = useState<SortDirection>(DEFAULT_SORT_DIRECTION);
  const [userInfoMap, setUserInfoMap] = useState<Map<string, { fullName: string; email: string }>>(new Map());

  const filters = useMemo(() => buildSieveFilters(searchTerm), [searchTerm]);
  const sorts = useMemo(() => {
    const prefix = sortDirection === "desc" ? "-" : "";
    return `${prefix}${PRIMARY_SORT_FIELD},${prefix}${SECONDARY_SORT_FIELD}`;
  }, [sortDirection]);

  const parseValidationMessage = useParseValidationMessage("Kunne ikke hente boards.");

  const fetchBoards = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await boardClient.getAll(filters ?? undefined, sorts ?? undefined, page, pageSize);
      const parsed = await parsePagedBoards(response);
      const list = Array.isArray(parsed?.items) ? parsed.items : [];
      const totalCount = coerceNumber(parsed?.totalCount, list.length);
      const serverPageSize = coerceNumber(parsed?.pageSize, DEFAULT_PAGE_SIZE) || DEFAULT_PAGE_SIZE;
      const serverPage = coerceNumber(parsed?.page, page) || 1;

      setBoards(list);
      setTotal(totalCount);
      setPageSize(serverPageSize);
      setPage(serverPage);
    } catch (err) {
      setError(parseValidationMessage(err));
      setBoards([]);
      setTotal(0);
    } finally {
      setLoading(false);
      setHasLoadedOnce(true);
    }
  }, [filters, page, pageSize, sorts, parseValidationMessage]);

  const fetchUsersByIds = useCallback(
    async (userIds: Array<string | undefined>) => {
      const uniqueIds = Array.from(new Set(userIds.filter((id): id is string => Boolean(id))));
      const missingIds = uniqueIds.filter((id) => !userInfoMap.has(id));
      if (!missingIds.length) return;

      const nextMap = new Map(userInfoMap);
      try {
        const chunks = chunkArray(missingIds, USERS_PAGE_SIZE);
        for (const chunk of chunks) {
          const filter = buildUserIdFilter(chunk);
          if (!filter) continue;

          const response = await userClient.getAll(filter, USER_FETCH_SORT, 1, chunk.length);
          const items = Array.isArray(response?.items) ? response.items : [];
          items.forEach((user) => {
            if (user?.userID) {
              nextMap.set(user.userID, {
                fullName: buildUserFullName(user),
                email: user.email ?? "Ukendt email",
              });
            }
          });
        }
        setUserInfoMap(nextMap);
      } catch (err) {
        console.error("Kunne ikke hente brugere til opslag.", err);
      }
    },
    [userInfoMap],
  );

  useEffect(() => {
    if (!boards.length) return;
    void fetchUsersByIds(boards.map((board) => board.userID));
  }, [boards, fetchUsersByIds]);

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

  const toggleSortDirection = useCallback(() => {
    setSortDirection((prev) => (prev === "asc" ? "desc" : "asc"));
    setPage(1);
  }, []);

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
    setSortDirection(DEFAULT_SORT_DIRECTION);
    setPage(1);
  }, []);

  const visibleStart = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const visibleEnd = total === 0 ? 0 : Math.min(total, page * pageSize);

  const boardsWithUsers = useMemo(
    () =>
      boards.map((board) => {
        const info = board.userID ? userInfoMap.get(board.userID) : null;
        return {
          ...board,
          userFullName: info?.fullName ?? "Ukendt bruger",
          userEmail: info?.email ?? "Ukendt email",
        };
      }),
    [boards, userInfoMap],
  );

  return {
    boards,
    boardsWithUsers,
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
    sortDirection,
    setSearchTerm,
    toggleSortDirection,
    handlePageChange,
    resetFilters,
    refresh: fetchBoards,
  };
};

