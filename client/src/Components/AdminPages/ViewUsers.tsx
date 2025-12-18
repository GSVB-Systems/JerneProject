import { useMemo } from "react";
import type { JSX } from "react";
import { useUsers } from "../../hooks/useUsers";
import type {
  SortField,
  StatusFilter,
  RoleFilter,
} from "../../hooks/useUsers";
import Navbar from "../Navbar";
import { useNavigate } from "react-router-dom";
import { useCreateUser } from "../../hooks/useCreateUser";

export default function ViewUsers(): JSX.Element {
  const navigate = useNavigate();
  const closeModal = () => {
    const modal = document.getElementById("create_user_modal") as HTMLDialogElement | null;
    modal?.close();
  };

  const { error: createUserError, formValues, updateField, createUser } = useCreateUser(() => {
    closeModal();
  });

  const openModal = () => {
    const modal = document.getElementById("create_user_modal") as HTMLDialogElement | null;
    modal?.showModal();
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    await createUser();
  };

  const {
    users: list,
    total,
    page,
      totalPages,
    visibleStart,
    visibleEnd,
    loading,
    hasLoadedOnce,
    error,
    searchTerm,
    statusFilter,
    roleFilter,
    sortField,
    sortDirection,
    setSearchTerm,
    setStatusFilter,
    setRoleFilter,
    setSortField,
    toggleSortDirection,
    toggleSort,
    handlePageChange,
    resetFilters,
  } = useUsers();

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

  const handleViewDetails = (id?: string) => {
    if (!id) return;
    navigate(`/brugere/${id}`);
  };

  const showEmptyState = !loading && list.length === 0 && !error;

  if (loading && list.length === 0) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="loader mb-4" />
            <p className="text-sm text-gray-500">Indlæser brugere…</p>
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
          <h1 className="text-2xl font-semibold">Brugere</h1>
          <div className="flex flex-row items-center gap-4">
            <button className="btn" onClick={openModal}>
              Opret Bruger
            </button>
            <p className="text-sm text-gray-500">{`Antal brugere: ${total}`}</p>
          </div>
        </header>


        <dialog id="create_user_modal" className="modal">
          <div className="modal-box max-w-lg">
            <h3 className="font-bold text-lg">Opret bruger</h3>
            <p className="py-2 text-sm text-gray-600">Udfyld felterne for at oprette en ny bruger.</p>

            <form className="flex flex-col gap-4 mt-4" id="createUserForm" onSubmit={handleSubmit}>
              <label className="flex flex-col">
                <span className="font-medium text-sm">Fornavn</span>
                <input className="input" id="firstname" onChange={(event) => updateField("firstname", event.target.value)} placeholder="Fornavn" required type="text" value={formValues.firstname ?? ""} />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Efternavn</span>
                <input className="input" id="lastname" onChange={(event) => updateField("lastname", event.target.value)} placeholder="Efternavn" required type="text" value={formValues.lastname ?? ""} />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Email</span>
                <input className="input" id="email" onChange={(event) => updateField("email", event.target.value)} placeholder="navn@eksempel.dk" required type="email" value={formValues.email ?? ""} />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Password</span>
                <input className="input" id="password" onChange={(event) => updateField("password", event.target.value)} placeholder="Adgangskode" required type="password" value={formValues.password ?? ""} />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Bekræft password</span>
                <input className="input" id="confirmPassword" onChange={(event) => updateField("confirmPassword", event.target.value)} placeholder="Gentag adgangskode" required type="password" value={formValues.confirmPassword ?? ""} />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Rolle</span>
                <select className="select" id="role" onChange={(event) => updateField("role", event.target.value)} required value={formValues.role ?? ""}>
                  <option disabled value="">
                    Vælg en rolle
                  </option>
                  <option value="Bruger">Bruger</option>
                  <option value="Administrator">Administrator</option>
                </select>
              </label>
              <div className="modal-action flex justify-end gap-2 pt-2">
                <button className="btn btn-ghost" onClick={closeModal} type="button">
                  Annuller
                </button>
                <button className="btn" type="submit">
                  Opret Bruger
                </button>
              </div>
            </form>

            {createUserError && <p className="text-red-500 mt-3">{createUserError}</p>}
          </div>
        </dialog>

        <section className="bg-base-200 rounded-lg p-4 space-y-4">
          <div className="grid gap-4 md:grid-cols-4">
            <label className="form-control">
              <span className="label-text text-sm font-medium">Søg</span>
              <input className="input input-bordered w-full" placeholder="Navn eller email" type="text" defaultValue={searchTerm} onChange={(event) => {
                handleSearchChange(event.target.value);
              }} />
            </label>
            <label className="form-control">
              <span className="label-text text-sm font-medium">Status</span>
              <select className="select select-bordered w-full" value={statusFilter} onChange={(event) => {
                setStatusFilter(event.target.value as StatusFilter);
              }}>
                <option value="all">Alle</option>
                <option value="active">Aktive</option>
                <option value="inactive">Inaktive</option>
              </select>
            </label>
            <label className="form-control">
              <span className="label-text text-sm font-medium">Rolle</span>
              <select className="select select-bordered w-full" value={roleFilter} onChange={(event) => {
                setRoleFilter(event.target.value as RoleFilter);
              }}>
                <option value="all">Alle roller</option>
                <option value="admin">Administrator</option>
                <option value="user">Bruger</option>
              </select>
            </label>
           </div>
          <div className="flex flex-wrap items-center gap-3 text-sm">
            <button className="btn btn-sm" onClick={resetFilters} type="button">
              Nulstil filtre
            </button>
            <div className="flex items-center gap-2">
              <span className="font-medium">Sorter:</span>
              <select className="select select-bordered select-sm" value={sortField} onChange={(event) => {
                setSortField(event.target.value as SortField);
              }}>
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
            <p className="text-gray-600">{hasLoadedOnce ? "Ingen brugere matcher dette filter." : "Indtast for at søge."}</p>
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
                    <th />
                  </tr>
                </thead>
                <tbody>
                  {list.map((user) => (
                    <tr key={user.email}>

                      <td>{`${user.firstname ?? ""} ${user.lastname ?? ""}`.trim() || "N/A"}</td>
                      <td className="opacity-90">{user.email}</td>
                      <td>{user.role ? "Administrator" : "Bruger"}</td>
                      <td>
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${user.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                          {user.isActive ? "Aktiv" : "Inaktiv"}
                        </span>
                      </td>
                      <td className="font-medium">{user.balance ?? 0}</td>
                      <td>
                        <button className="btn btn-sm btn-ghost" disabled={!user.userID} onClick={() => handleViewDetails(user.userID)}>
                          Detaljer
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="md:hidden space-y-3">
              {list.map((user) => (
                <div key={user.email} className="card card-compact bg-base-200 p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <div className="text-lg font-semibold">{`${user.firstname ?? ""} ${user.lastname ?? ""}`.trim() || "N/A"}</div>
                      <div className="text-sm text-gray-500">{user.email}</div>
                    </div>
                    <div className="text-right">
                      <div className="text-sm">{user.role ? "Admin" : "User"}</div>
                    </div>
                  </div>

                  <div className="mt-3 flex items-center justify-between">
                    <div className="text-sm">
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${user.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                        {user.isActive ? "Active" : "Inactive"}
                      </span>
                    </div>
                    <div className="text-sm font-medium">{user.balance ?? 0}</div>
                    <button className="btn btn-sm btn-ghost" disabled={!user.userID} onClick={() => handleViewDetails(user.userID)}>
                      Detaljer
                    </button>
                  </div>
                </div>
              ))}
            </div>

            <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between pt-4">
              <p className="text-sm text-gray-600">{`Viser ${visibleStart}-${visibleEnd} of ${total}`}</p>
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
  { field: "firstname", label: "Navn" },
  { field: "email", label: "Email" },
  { field: "role", label: "Rolle" },
  { field: "isActive", label: "Status", align: "text-center" },
  { field: "balance", label: "Balance", align: "text-right" },
];

const SORTABLE_COLUMNS = TABLE_COLUMNS;

type ColumnDefinition = {
  field: SortField;
  label: string;
  align?: string;
};
