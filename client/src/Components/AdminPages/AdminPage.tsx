import React, {type JSX, useState} from "react";
import { useNavigate } from "react-router-dom";
import { useCreateUser } from "../../hooks/useCreateUser";
import type { RegisterUserDto } from "../../models/ServerAPI";

export default function AdminPage(): JSX.Element {
  const [firstname, setFirstname] = useState("");
  const [lastname, setLastname] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("");
  const navigate = useNavigate();

  const closeModal = () => {
    const modal = document.getElementById("my_modal_1") as HTMLDialogElement | null;
    modal?.close();
  };

  const { error, createUser } = useCreateUser(closeModal);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const dto: RegisterUserDto = {
      firstname,
      lastname,
      email,
      password,
      role,
    };
    await createUser(dto);
  };

  const redirectToUsers = () => navigate("/brugere");

  return (
    <div className="min-h-screen bg-gray-50 flex items-start justify-center py-12">
      <main className="w-full max-w-2xl bg-white rounded-lg shadow-md p-6">
        <h1 className="text-2xl font-semibold mb-4">Admin Page</h1>

        <div className="flex gap-3 mb-6">
          <button
            className="btn "
            onClick={() => {
              const el = document.getElementById("my_modal_1") as HTMLDialogElement | null;
              el?.showModal();
            }}
          >
            Opret Bruger
          </button>

          <button className="btn" onClick={redirectToUsers}>
            Se Brugere
          </button>
        </div>

        <dialog id="my_modal_1" className="modal">
          <div className="modal-box max-w-lg">
            <h3 className="font-bold text-lg">Opret bruger</h3>
            <p className="py-2 text-sm text-gray-600">Udfyld felterne for at oprette en ny bruger.</p>

            <form id="createUserForm" onSubmit={handleSubmit} className="flex flex-col gap-4 mt-4">
              <label className="flex flex-col">
                <span className="font-medium text-sm">Fornavn</span>
                <input
                  id="firstname"
                  type="text"
                  className="input"
                  value={firstname}
                  onChange={(e) => setFirstname(e.target.value)}
                  required
                  placeholder="Fornavn"
                />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Efternavn</span>
                <input
                  id="lastname"
                  type="text"
                  className="input"
                  value={lastname}
                  onChange={(e) => setLastname(e.target.value)}
                  required
                  placeholder="Efternavn"
                />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Email</span>
                <input
                  id="email"
                  type="email"
                  className="input"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  placeholder="navn@eksempel.dk"
                />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Password</span>
                <input
                  id="password"
                  type="password"
                  className="input"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  placeholder="Adgangskode"
                />
              </label>

              <label className="flex flex-col">
                <span className="font-medium text-sm">Rolle</span>
                <select
                  id="role"
                  className="select"
                  value={role}
                  onChange={(e) => setRole(e.target.value)}
                  required
                >
                    <option value="" disabled>
                        Vælg en rolle
                    </option>
                  <option value="Bruger">Bruger</option>
                  <option value="Administrator">Administrator</option>
                </select>
              </label>

              <div className="modal-action flex justify-end gap-2 pt-2">
                <button type="button" className="btn btn-ghost" onClick={closeModal}>
                  Annuller
                </button>
                <button type="submit" className="btn">
                  Opret Bruger
                </button>
              </div>
            </form>

            {error && <p className="text-red-500 mt-3">{error}</p>}
          </div>
        </dialog>
      </main>
    </div>
  );
}