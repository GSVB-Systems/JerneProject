import type { JSX } from "react";
import { useNavigate } from "react-router-dom";
import AdminBoard from "./AdminBoard.tsx";

// Create-user flow has been moved into ViewUsers, so this page only links there.

export default function AdminPage(): JSX.Element {
  const navigate = useNavigate();

  const redirectToUsers = () => navigate("/brugere");
  const redirectToTransactions = () => navigate("/admintransactions");
  const redirectToWinners = () => navigate("/winnerhistory");

  return (
    <div className="min-h-screen bg-gray-50 flex items-start justify-center py-12">
      <main className="w-full max-w-2xl bg-white rounded-lg shadow-md p-6">
        <h1 className="text-2xl font-semibold mb-4">Admin Page</h1>

        <div className="flex flex-col gap-6 mb-6 ">
          <button className="btn" onClick={redirectToUsers}>
            Se Brugere
          </button>
          <button className="btn" onClick={redirectToTransactions}>
            Se Transaktioner
          </button>
          <button className="btn" onClick={redirectToWinners}>
            Se Vinderhistorik
          </button>
        </div>
        <AdminBoard></AdminBoard>
      </main>
    </div>
  );
}