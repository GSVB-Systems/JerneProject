import Navbar from "../Navbar.tsx";
import ThinBoard from "./ThinBoard.tsx";
import { useUserBoardHistory } from "../../hooks/useUserBoardHistory.ts";
import type { BoardSortField } from "../../hooks/useUserBoardHistory.ts";
import type { JSX } from "react";
import { useMemo } from "react";

const SORT_OPTIONS: Array<{ label: string; field: BoardSortField }> = [
    { label: "Oprettelsesdato", field: "createdAt" },
    { label: "Størrelse", field: "boardSize" },
];

export default function BoardHistory(): JSX.Element {
    const {
        boards,
        total,
        page,
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
        refresh,
    } = useUserBoardHistory();

    const debouncedSearch = useMemo(() => {
        let timeout: ReturnType<typeof setTimeout> | null = null;
        return (value: string) => {
            if (timeout) clearTimeout(timeout);
            timeout = setTimeout(() => {
                setSearchTerm(value);
            }, 300);
        };
    }, [setSearchTerm]);

    const handleSearchChange = (value: string) => {
        debouncedSearch(value);
    };

    const showEmptyState = !loading && boards.length === 0 && !error;

    return (
        <div className="flex flex-col min-h-screen w-full bg-base-100">
            <Navbar />
            <main className="p-6 max-w-5xl mx-auto w-full space-y-6 flex-1">
                <header className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
                    <div>
                        <h1 className="text-2xl font-semibold">Board historik</h1>
                        <p className="text-sm text-gray-500">Hold styr på dine tidligere boards.</p>
                    </div>
                    <div className="text-sm text-gray-500">{`Antal: ${total}`}</div>
                </header>

                <section className="bg-base-200 rounded-lg p-4 space-y-4">
                    <div className="grid gap-4 md:grid-cols-2">
                        <label className="form-control">
                            <span className="label-text text-sm font-medium">Søg</span>
                            <input
                                className="input input-bordered w-full"
                                placeholder="Søg efter tal"
                                type="text"
                                defaultValue={searchTerm}
                                onChange={(event) => {
                                    handleSearchChange(event.target.value);
                                }}
                            />
                        </label>
                        <div className="flex gap-2 items-end">
                            <label className="form-control flex-1">
                                <span className="label-text text-sm font-medium">Sorter</span>
                                <select
                                    className="select select-bordered w-full"
                                    value={sortField}
                                    onChange={(event) => {
                                        setSortField(event.target.value as BoardSortField);
                                    }}
                                >
                                    {SORT_OPTIONS.map((option) => (
                                        <option key={option.field} value={option.field}>
                                            {option.label}
                                        </option>
                                    ))}
                                </select>
                            </label>
                            <button className="btn" onClick={toggleSortDirection} type="button">
                                {sortDirection === "asc" ? "Stigende" : "Faldende"}
                            </button>
                        </div>
                    </div>

                    <div className="flex flex-wrap items-center gap-3 text-sm">
                        <button className="btn btn-sm" onClick={resetFilters} type="button">
                            Nulstil filtre
                        </button>
                        <button className="btn btn-sm" onClick={refresh} type="button">
                            Opdater
                        </button>
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
                        {boards.map((board) => (
                            <ThinBoard
                                key={board.boardID}
                                selectedNumbers={(board.numbers ?? [])
                                    .map((entry) => Number(entry?.number ?? 0))
                                    .filter((num) => !Number.isNaN(num))
                                    .sort((a, b) => a - b)}
                                creationTimestamp={board.createdAt ? Date.parse(board.createdAt) / 1000 : undefined}
                            />
                        ))}
                     </section>
                )}

                {!showEmptyState && total > 0 && (
                    <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between pt-4">
                        <p className="text-sm text-gray-600">{`Viser ${visibleStart}-${visibleEnd} af ${total}`}</p>
                        <div className="flex items-center gap-2">
                            <button className="btn btn-sm" disabled={page === 1} onClick={() => handlePageChange("prev")}
                                    type="button">
                                Tidligere
                            </button>
                            <span className="text-sm font-medium">{`Side ${page} af ${totalPages}`}</span>
                            <button className="btn btn-sm" disabled={page >= totalPages} onClick={() => handlePageChange("next")}
                                    type="button">
                                Næste
                            </button>
                        </div>
                    </div>
                )}
            </main>
        </div>
    );
}
