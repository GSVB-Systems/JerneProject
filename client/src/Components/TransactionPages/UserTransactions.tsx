import React, {useMemo} from "react";
import type { JSX } from "react";
import Navbar from "../Navbar";
import { useUserTransactions } from "../../hooks/useUserTransactions";
import type { TransactionSortField, TransactionTypeFilter } from "../../hooks/useUserTransactions";
import { useCreateTransaction } from "../../hooks/useCreateTransaction";

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
    refresh,
  } = useUserTransactions();
  const {
    amount,
    transactionString,
    setAmount,
    setTransactionString,
    createTransaction,
    resetForm,
    error: createError,
    isSubmitting,
  } = useCreateTransaction();

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

  const closeModal = () => {
    const modal = document.getElementById("my_modal_1") as HTMLDialogElement | null;
    modal?.close();
    resetForm();
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const succeeded = await createTransaction();
    if (succeeded) {
      closeModal();
      await refresh();
    }
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
          <button className="btn" onClick={() => {
              const el = document.getElementById("my_modal_1") as HTMLDialogElement | null;
              el?.showModal();
            }}
          > Opret transaktion</button>
          <p className="text-sm text-gray-500">{`Antal: ${total}`}</p>
        </header>

        <dialog id="my_modal_1" className="modal">
          <div className="modal-box max-w-lg">
            <h3 className="font-bold text-lg">Opret Transaktion</h3>
            <p className="py-2 text-sm text-gray-600">Udfyld felterne for at oprette en ny Transaktion.</p>

            <form id="createUserForm" onSubmit={handleSubmit} className="flex flex-col gap-4 mt-4">
              <label className="flex flex-col">
                <span className="font-medium text-sm">Beløb</span>
                <input
                    id="beløb"
                    type="text"
                    className="input"
                    value={amount}
                    onChange={(e) => setAmount(e.target.value)}
                    required
                    placeholder="Beløb"
                />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Transaktionsnummer</span>
                <input
                    id="transaktionsnummer"
                    type="text"
                    className="input"
                    value={transactionString}
                    onChange={(e) => setTransactionString(e.target.value)}
                    required
                    placeholder="Transaktionsnummer"
                />
              </label>
              <div className="modal-action flex justify-end gap-2 pt-2">
                <button type="button" className="btn btn-ghost" onClick={closeModal}>
                  Annuller
                </button>
                <button type="submit" className="btn" disabled={isSubmitting}>
                  {isSubmitting ? "Opretter..." : "Opret Transaktion"}
                </button>
              </div>
            </form>

            {(createError ?? error) && <p className="text-red-500 mt-3">{createError ?? error}</p>}
          </div>
        </dialog>

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
                      <td className="opacity-90">{tx.transactionString ?? "-"}</td>
                      <td>
                        <div className="font-medium whitespace-nowrap">
                          {new Date(tx.transactionDate ?? "").toLocaleString() || "-"}
                        </div>
                      </td>
                      <td className="whitespace-nowrap text-right">
                        <span className={tx.amount && tx.amount > 0 ? "text-green-600" : "text-red-600"}>
                          {tx.amount?.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) ?? "0,00"}
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
  { field: "transactionString", label: "Beskrivelse" },
  { field: "transactionDate", label: "Dato" },
  { field: "amount", label: "Beløb", align: "text-right" },
];

const SORTABLE_COLUMNS = TABLE_COLUMNS;

type ColumnDefinition = {
  field: TransactionSortField;
  label: string;
  align?: string;
};
