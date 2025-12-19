import Navbar from "../Navbar.tsx";
import ThinBoard from "../BoardPages/ThinBoard.tsx";
import { useAdminBoardHistory } from "../../hooks/useAdminBoardHistory.ts";
import type { JSX } from "react";

export default function AdminBoardHistory(): JSX.Element {
  const {
    boards,
    boardsWithUsers,
    total,
    page,
    totalPages,
    visibleStart,
    visibleEnd,
    loading,
    hasLoadedOnce,
    error,
    sortDirection,
    toggleSortDirection,
    handlePageChange,
    resetFilters,
    refresh,
  } = useAdminBoardHistory();

  const showEmptyState = !loading && boards.length === 0 && !error;
  const sortLabel = sortDirection === "asc" ? "Stigende" : "Faldende";
  const getInitials = (name: string | undefined): string => {
    if (!name) return "??";
    const [first = "", second = ""] = name.split(" ");
    return `${first.charAt(0)}${second.charAt(0)}`.trim().toUpperCase() || first.charAt(0).toUpperCase() || "?";
  };

  return (
    <div className="flex flex-col min-h-screen w-full bg-base-100">
      <Navbar />
      <main className="p-6 max-w-5xl mx-auto w-full space-y-6 flex-1">
        <header className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
          <div>
            <h1 className="text-2xl font-semibold">Board historik</h1>
            <p className="text-sm text-gray-500">Hold styr på alle tidligere boards.</p>
          </div>
          <div className="text-sm text-gray-500">{`Antal: ${total}`}</div>
        </header>

        <section className="bg-base-200 rounded-lg p-4 space-y-6">
          <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div className="flex flex-col gap-2">
              <span className="label-text text-sm font-medium">Sortering</span>
              <div className="flex flex-wrap items-center gap-3 text-sm">
                <span className="text-sm text-gray-600">Spilleuge</span>
                <button
                  className="btn btn-sm btn-outline flex items-center gap-2 border-base-300"
                  onClick={toggleSortDirection}
                  type="button"
                  disabled={loading}
                >
                  <span className="font-semibold">{sortLabel}</span>
                  <span className="text-xs text-gray-500">
                    {sortDirection === "asc" ? "Ældste først" : "Nyeste først"}
                  </span>
                </button>
              </div>
            </div>

            <div className="flex flex-wrap items-center gap-3 text-sm">
              <button className="btn btn-sm btn-ghost border border-base-300" disabled={loading} onClick={resetFilters} type="button">
                Nulstil filtre
              </button>
              <button className="btn btn-sm border border-base-300 bg-base-100" onClick={refresh} type="button">
                {loading ? "Opdaterer…" : "Opdater"}
              </button>
            </div>
          </div>
        </section>

        {error && (
          <div className="alert alert-error">
            <span>{error}</span>
          </div>
        )}

        {loading && boards.length === 0 ? (
          <div className="flex flex-1 items-center justify-center py-12">
            <div className="text-center">
              <div className="loader mb-4" />
              <p className="text-sm text-gray-500">Henter boards…</p>
            </div>
          </div>
        ) : showEmptyState ? (
          <div className="flex flex-1 items-center justify-center py-12">
            <p className="text-gray-600">
              {hasLoadedOnce ? "Ingen boards matcher filteret." : "Indtast for at søge."}
            </p>
          </div>
        ) : (
          <section className="space-y-4">
            {boardsWithUsers.map((board) => (
              <article key={board.boardID} className="space-y-3 rounded-2xl border border-base-300 bg-white/80 p-4 shadow-sm">
                <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex items-center gap-3">
                    <div className="flex h-10 w-10 items-center justify-center rounded-full bg-red-100 text-sm font-semibold text-red-700">
                      {getInitials(board.userFullName)}
                    </div>
                    <div className="flex flex-col">
                      <span className="text-base font-semibold text-gray-900">{board.userFullName}</span>
                      <span className="text-sm text-gray-500">{board.userEmail}</span>
                    </div>
                  </div>
                  <div className="flex flex-wrap gap-3 text-xs sm:text-sm">
                    <span className="rounded-full bg-gray-100 px-3 py-1 font-medium text-gray-700">
                      Uge {board.week ?? "?"}, {board.year ?? "?"}
                    </span>
                    <span className={`rounded-full px-3 py-1 font-medium ${board.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"}`}>
                      {board.isActive ? "Aktiv" : "Inaktiv"}
                    </span>
                    {board.win && (
                      <span className="rounded-full bg-yellow-100 px-3 py-1 font-medium text-yellow-700">Vinder</span>
                    )}
                  </div>
                </div>

                <ThinBoard
                  selectedNumbers={(board.numbers ?? [])
                    .map((entry) => Number(entry?.number ?? 0))
                    .filter((num) => !Number.isNaN(num))
                    .sort((a, b) => a - b)}
                  playingWeek={board.week}
                  playingYear={board.year}
                  isActive={board.isActive}
                  hasWon={board.win}
                />
              </article>
            ))}
          </section>
        )}

        {!showEmptyState && total > 0 && (
          <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between pt-4">
            <p className="text-sm text-gray-600">{`Viser ${visibleStart}-${visibleEnd} af ${total}`}</p>
            <div className="flex items-center gap-2">
              <button className="btn btn-sm border border-base-300" disabled={page === 1} onClick={() => handlePageChange("prev")} type="button">
                Tidligere
              </button>
              <span className="text-sm font-medium">{`Side ${page} af ${totalPages}`}</span>
              <button className="btn btn-sm border border-base-300" disabled={page >= totalPages} onClick={() => handlePageChange("next")} type="button">
                Næste
              </button>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}

