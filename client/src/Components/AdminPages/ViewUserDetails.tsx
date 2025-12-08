import { useNavigate, useParams } from "react-router-dom";
import Navbar from "../Navbar.tsx";
import { useUserDetails } from "../../hooks/useUserDetails.ts";
import { useRef } from "react";
import type { JSX } from "react";

export default function ViewUserDetails(): JSX.Element {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const {
    user,
    loading,
    error,
    formState,
    saving,
    saveMessage,
    saveError,
    handleFormChange,
    handleSubmit,
    editing,
    beginEdit,
    cancelEdit,
    deleting,
    deleteError,
    resetDeleteState,
    deleteUser,
  } = useUserDetails(userId);
  const deleteDialogRef = useRef<HTMLDialogElement | null>(null);

  const goBack = () => navigate("/brugere");
  const openDeleteDialog = () => {
    resetDeleteState();
    deleteDialogRef.current?.showModal();
  };
  const closeDeleteDialog = () => deleteDialogRef.current?.close();

  const confirmDelete = async () => {
    const success = await deleteUser();
    if (success) {
      closeDeleteDialog();
      navigate("/brugere");
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="loader mb-4" />
            <p className="text-sm text-gray-500">Henter brugerdata…</p>
          </div>
        </main>
      </div>
    );
  }

  if (error || !user) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex flex-col items-center justify-center gap-4 text-center px-4">
          <p className="text-red-500">{error ?? "Kunne ikke finde brugeren."}</p>
          <button className="btn" onClick={goBack}>
            Tilbage til brugere
          </button>
        </main>
      </div>
    );
  }

  return (
    <div className="flex flex-col min-h-screen bg-base-100">
      <Navbar />
      <main className="flex-1 px-4 py-8 max-w-4xl mx-auto w-full">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-3xl font-semibold">{`${user.firstname ?? ""} ${user.lastname ?? ""}`.trim() || "Bruger"}</h1>
            <p className="text-sm text-gray-500">{user.email}</p>
          </div>
          <div className="flex gap-3">
            <button className="btn btn-ghost" onClick={goBack}>
              Tilbage
            </button>
            {editing ? (
              <button type="button" className="btn btn-ghost" onClick={cancelEdit} disabled={saving}>
                Annuller
              </button>
            ) : (
              <button type="button" className="btn" onClick={beginEdit} disabled={!formState}>
                Rediger
              </button>
            )}
            <button type="button" className="btn btn-error" onClick={openDeleteDialog}>
              Slet bruger
            </button>
          </div>
        </div>

        {!editing ? (
          <section className="grid gap-6 md:grid-cols-2">
            <div className="card bg-base-200 shadow p-6">
              <h2 className="text-lg font-semibold mb-4">Profil</h2>
              <dl className="space-y-3 text-sm">
                <div className="flex justify-between">
                  <dt className="text-gray-500">Navn</dt>
                  <dd className="font-medium">{`${user.firstname ?? "-"} ${user.lastname ?? ""}`.trim()}</dd>
                </div>
                <div className="flex justify-between">
                  <dt className="text-gray-500">Email</dt>
                  <dd className="font-medium break-all">{user.email ?? "-"}</dd>
                </div>
                <div className="flex justify-between">
                  <dt className="text-gray-500">Rolle</dt>
                  <dd className="font-medium">{user.role ? "Administrator" : "Bruger"}</dd>
                </div>
                <div className="flex justify-between">
                  <dt className="text-gray-500">Første login</dt>
                  <dd className="font-medium">{user.firstlogin ? "Ja" : "Nej"}</dd>
                </div>
              </dl>
            </div>

            <div className="card bg-base-200 shadow p-6">
              <h2 className="text-lg font-semibold mb-4">Status</h2>
              <dl className="space-y-3 text-sm">
                <div className="flex justify-between items-center">
                  <dt className="text-gray-500">Aktiv</dt>
                  <dd>
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        user.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
                      }`}
                    >
                      {user.isActive ? "Aktiv" : "Inaktiv"}
                    </span>
                  </dd>
                </div>
                <div className="flex justify-between">
                  <dt className="text-gray-500">Balance</dt>
                  <dd className="font-medium">{user.balance ?? 0}</dd>
                </div>
              </dl>
            </div>
          </section>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-6">
            {saveMessage && (
              <div className="alert alert-success">
                <span>{saveMessage}</span>
              </div>
            )}
            {saveError && (
              <div className="alert alert-error">
                <span>{saveError}</span>
              </div>
            )}
            <section className="grid gap-6 md:grid-cols-2">
              <div className="card bg-base-200 shadow p-6">
                <h2 className="text-lg font-semibold mb-4">Profil</h2>
                <div className="space-y-4">
                  <div>
                    <label className="label">
                      <span className="label-text">Fornavn</span>
                    </label>
                    <input
                      type="text"
                      name="firstname"
                      value={formState?.firstname ?? ""}
                      onChange={handleFormChange}
                      className="input w-full"
                      required
                    />
                  </div>
                  <div>
                    <label className="label">
                      <span className="label-text">Efternavn</span>
                    </label>
                    <input
                      type="text"
                      name="lastname"
                      value={formState?.lastname ?? ""}
                      onChange={handleFormChange}
                      className="input w-full"
                      required
                    />
                  </div>
                  <div>
                    <label className="label">
                      <span className="label-text">Email</span>
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={formState?.email ?? ""}
                      onChange={handleFormChange}
                      className="input w-full"
                      required
                    />
                  </div>
                  <div>
                    <label className="label">
                      <span className="label-text">Rolle</span>
                    </label>
                    <select
                      name="role"
                      value={String(formState?.role ?? 0)}
                      onChange={handleFormChange}
                      className="select w-full"
                      required
                    >
                      <option value="0">Bruger</option>
                      <option value="1">Administrator</option>
                    </select>
                  </div>
                  <div className="form-control">
                    <label className="label cursor-pointer">
                      <span className="label-text">Første login</span>
                      <input
                        type="checkbox"
                        name="firstlogin"
                        checked={formState?.firstlogin ?? false}
                        onChange={handleFormChange}
                        className="toggle"
                      />
                    </label>
                  </div>
                </div>
              </div>

              <div className="card bg-base-200 shadow p-6">
                <h2 className="text-lg font-semibold mb-4">Status</h2>
                <div className="space-y-4">
                  <div className="flex justify-between">
                    <span className="text-gray-500">Aktiv</span>
                    <label className="toggle-label">
                      <input
                        type="checkbox"
                        name="isActive"
                        checked={formState?.isActive ?? false}
                        onChange={handleFormChange}
                        className="toggle"
                      />
                      <span className="toggle-circle" />
                    </label>
                  </div>
                  <div>
                    <label className="label">
                      <span className="label-text">Balance</span>
                    </label>
                    <input
                      type="number"
                      name="balance"
                      value={formState?.balance ?? 0}
                      step="0.01"
                      onChange={handleFormChange}
                      className="input w-full"
                      required
                    />
                  </div>
                </div>
              </div>
            </section>

            <div className="flex justify-end gap-4">
              <button type="button" className="btn btn-ghost" onClick={cancelEdit} disabled={saving}>
                Fortryd
              </button>
              <button
                type="submit"
                className={`btn btn ${saving ? "loading" : ""}`}
                disabled={saving || !formState}
              >
                {saving ? "Gemmer…" : "Gem ændringer"}
              </button>
            </div>
          </form>
        )}
      </main>

      <dialog ref={deleteDialogRef} className="modal">
        <div className="modal-box">
          <h3 className="font-bold text-lg text-red-600">Slet bruger</h3>
          <p className="py-2">Er du sikker på, at du vil slette denne bruger? Denne handling kan ikke fortrydes.</p>
          {deleteError && <p className="text-sm text-red-500">{deleteError}</p>}
          <div className="modal-action">
            <button className="btn" onClick={closeDeleteDialog} disabled={deleting}>
              Annuller
            </button>
            <button
              className={`btn btn-error ${deleting ? "loading" : ""}`}
              onClick={confirmDelete}
              disabled={deleting}
            >
              {deleting ? "Sletter…" : "Slet"}
            </button>
          </div>
        </div>
      </dialog>
    </div>
  );
}
