import { useMemo } from "react";
import type { JSX } from "react";
import Navbar from "../Navbar";
import { useUserTransactions } from "../../hooks/useUserTransactions";
import type { TransactionSortField, TransactionTypeFilter } from "../../hooks/useUserTransactions";

export default function UserTransactions(): JSX.Element {
  const {
    transactions,
    total,
    page,
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
  } = useUserTransactions();

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

  const showEmptyState = !loading && transactions.length === 0 && !error;

  if (loading && transactions.length === 0) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="loader mb-4" />
            <p className="text-sm text-gray-500">Henter transaktioner…</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="flex flex-col min-h-screen w-full bg-base-100">
      <Navbar />
      <main className="p-6 max-w-6xl mx-auto w-full space-y-6">
        <header className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
          <h1 className="text-2xl font-semibold">Mine transaktioner</h1>
          <p className="text-sm text-gray-500">{`Antal: ${total}`}</p>
        </header>

        <section className="bg-base-200 rounded-lg p-4 space-y-4">
          <div className="grid gap-4 md:grid-cols-3">
            <label className="form-control">
              <span className="label-text text-sm font-medium">Søg</span>
              <input
                className="input input-bordered w-full"
                placeholder="Tekst"
                type="text"
                defaultValue={searchTerm}
                onChange={(event) => {
                  handleSearchChange(event.target.value);
                }}
              />
            </label>

            <label className="form-control">
              <span className="label-text text-sm font-medium">Type</span>
              <select
                className="select select-bordered w-full"
                value={transactionTypeFilter}
                onChange={(event) => {
                  setTransactionTypeFilter(event.target.value as TransactionTypeFilter);
                }}
              >
                <option value="all">Alle</option>
                <option value="credit">Indbetaling</option>
                <option value="debit">Udbetaling</option>
              </select>
            </label>
          </div>

          <div className="flex flex-wrap items-center gap-3 text-sm">
            <button className="btn btn-sm" onClick={resetFilters} type="button">
              Nulstil filtre
            </button>
            <div className="flex items-center gap-2">
              <span className="font-medium">Sorter:</span>
              <select
                className="select select-bordered select-sm"
                value={sortField}
                onChange={(event) => {
                  setSortField(event.target.value as TransactionSortField);
                }}
              >
                {SORTABLE_COLUMNS.map((column) => (
                  <option key={column.field} value={column.field}>
                    {column.label}
                  </option>
                ))}
              </select>
              <button className="btn btn-sm" onClick={toggleSortDirection} type="button">
                {sortDirection === "asc" ? "Stigende" : "Faldende"}
              </button>
            </div>
          </div>
        </section>

        {error && (
          <div className="alert alert-error">
            <span>{error}</span>
          </div>
        )}

        {showEmptyState ? (
          <div className="flex flex-1 items-center justify-center py-10">
            <p className="text-gray-600">
              {hasLoadedOnce ? "Ingen transaktioner matcher filteret." : "Indtast for at søge."}
            </p>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto hidden md:block shadow rounded-lg">
              <table className="table w-full">
                <thead>
                  <tr>
                    {TABLE_COLUMNS.map((column) => (
                      <th key={column.field} className={column.align ?? "text-left"}>
                        <button className="flex items-center gap-2" onClick={() => toggleSort(column.field)} type="button">
                          {column.label}
                          {sortField === column.field && <span className="text-xs text-gray-500"></span>}
                        </button>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {transactions.map((tx) => (
                    <tr key={tx.transactionID}>
                      <td>
                        <div className="font-medium whitespace-nowrap">
                          {new Date(tx.transactionDate ?? "").toLocaleString() || "-"}
                        </div>
                      </td>
                      <td className="whitespace-nowrap">
                        <span className={tx.amount && tx.amount > 0 ? "text-green-600" : "text-red-600"}>
                          {tx.amount?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "0,00"}
                        </span>
                      </td>
                      <td className="opacity-90">{tx.transactionString ?? "-"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="md:hidden space-y-3">
              {transactions.map((tx) => (
                <div key={tx.transactionID} className="card card-compact bg-base-200 p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <div className="text-lg font-semibold">
                        {tx.amount?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "0,00"}
                      </div>
                      <div className="text-sm text-gray-500">{tx.transactionString ?? "-"}</div>
                    </div>
                    <div className="text-right text-sm">
                      {new Date(tx.transactionDate ?? "").toLocaleString() || "-"}
                    </div>
                  </div>
                </div>
              ))}
            </div>

            <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between pt-4">
              <p className="text-sm text-gray-600">{`Viser ${visibleStart}-${visibleEnd} af ${total}`}</p>
              <div className="flex items-center gap-2">
                <button className="btn btn-sm" disabled={page === 1} onClick={() => handlePageChange("prev")} type="button">
                  Tidligere
                </button>
                <span className="text-sm font-medium">{`Side ${page} af ${totalPages}`}</span>
                <button className="btn btn-sm" disabled={page >= totalPages} onClick={() => handlePageChange("next")} type="button">
                  Næste
                </button>
              </div>
            </div>
          </>
        )}
      </main>
    </div>
  );
}

const TABLE_COLUMNS: ColumnDefinition[] = [
  { field: "transactionDate", label: "Dato" },
  { field: "amount", label: "Beløb", align: "text-right" },
  { field: "transactionString", label: "Beskrivelse" },
];

const SORTABLE_COLUMNS = TABLE_COLUMNS;

type ColumnDefinition = {
  field: TransactionSortField;
  label: string;
  align?: string;
};
