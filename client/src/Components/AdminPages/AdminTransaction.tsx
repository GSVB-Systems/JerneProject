import { useMemo } from "react";
import type { JSX } from "react";
import Navbar from "../Navbar";
import { useAdminTransactions } from "../../hooks/useAdminTransactions";
import type { TransactionSortField, TransactionTypeFilter } from "../../hooks/useUserTransactions";

const formatAmount = (amount?: number) =>
  amount?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "0,00";

const renderPendingBadge = (pending?: boolean) =>
  pending ? (
    <span className="badge badge-warning badge-sm">Afventer</span>
  ) : (
    <span className="badge badge-success badge-sm">Bogført</span>
  );

export default function AdminTransaction(): JSX.Element {
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
    userNamesLoaded,
  } = useAdminTransactions();

  const debouncedSearch = useMemo(() => {
    let timeout: ReturnType<typeof setTimeout> | null = null;
    return (value: string) => {
      if (timeout) clearTimeout(timeout);
      timeout = setTimeout(() => setSearchTerm(value), 300);
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
          <div>
            <h1 className="text-2xl font-semibold">Alle transaktioner</h1>
            {!userNamesLoaded && (
              <p className="text-xs text-gray-500">Indlæser brugernavne…</p>
            )}
          </div>
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
                onChange={(event) => handleSearchChange(event.target.value)}
              />
            </label>

            <label className="form-control">
              <span className="label-text text-sm font-medium">Type</span>
              <select
                className="select select-bordered w-full"
                value={transactionTypeFilter}
                onChange={(event) => setTransactionTypeFilter(event.target.value as TransactionTypeFilter)}
              >
                <option value="all">Alle</option>
                <option value="bogført">Bogført</option>
                <option value="afventer">Afventer</option>
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
                onChange={(event) => setSortField(event.target.value as TransactionSortField)}
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
                      <th key={column.key} className={column.align ?? "text-left"}>
                        {column.sortableField ? (
                          <button
                            className="flex items-center gap-2"
                            onClick={() => toggleSort(column.sortableField!)}
                            type="button"
                          >
                            {column.label}
                            {sortField === column.sortableField && (
                              <span className="text-xs text-gray-500">
                                {sortDirection === "asc" ? "▲" : "▼"}
                              </span>
                            )}
                          </button>
                        ) : (
                          <span>{column.label}</span>
                        )}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {transactions.map((tx) => (
                    <tr key={tx.transactionID}>
                      <td className="opacity-90">{tx.transactionString ?? "-"}</td>
                      <td className="whitespace-nowrap">{tx.userFullName}</td>
                      <td>{renderPendingBadge(tx.pending)}</td>
                      <td>
                        <div className="font-medium whitespace-nowrap">
                          {new Date(tx.transactionDate ?? "").toLocaleString() || "-"}
                        </div>
                      </td>
                      <td className="whitespace-nowrap text-right">
                        <span className={tx.amount && tx.amount > 0 ? "text-green-600" : "text-red-600"}>
                          {formatAmount(tx.amount)}
                        </span>
                      </td>
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
                      <div className="text-lg font-semibold">{formatAmount(tx.amount)}</div>
                      <div className="text-sm text-gray-500">{tx.transactionString ?? "-"}</div>
                      <div className="text-sm text-gray-500">{tx.userFullName}</div>
                    </div>
                    <div className="text-right text-sm">
                      {new Date(tx.transactionDate ?? "").toLocaleString() || "-"}
                    </div>
                  </div>
                  <div className="mt-2">{renderPendingBadge(tx.pending)}</div>
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
                <button
                  className="btn btn-sm"
                  disabled={page >= totalPages}
                  onClick={() => handlePageChange("next")}
                  type="button"
                >
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
  { key: "transactionString", label: "Transaktionsnummer", sortableField: "transactionString" },
  { key: "userFullName", label: "Navn" },
  { key: "pending", label: "Status", sortableField: "pending" },
  { key: "transactionDate", label: "Dato", sortableField: "transactionDate" },
  { key: "amount", label: "Beløb", align: "text-right", sortableField: "amount" },
];

const SORTABLE_COLUMNS: SortableColumn[] = [
  { field: "transactionString", label: "Transaktionsnummer" },
  { field: "pending", label: "Status" },
  { field: "transactionDate", label: "Dato" },
  { field: "amount", label: "Beløb" },
];

type ColumnDefinition = {
  key: string;
  label: string;
  align?: string;
  sortableField?: TransactionSortField;
};

type SortableColumn = {
  field: TransactionSortField;
  label: string;
};
