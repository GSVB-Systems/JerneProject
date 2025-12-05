import Navbar from "../Navbar";
import { useUsers } from "../../hooks/useUsers";

export default function ViewUsers(): JSX.Element {
  const users = useUsers();
  const list = users ?? [];

  if (!users) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="loader mb-4" /> {/* replace with your spinner if available */}
            <p className="text-sm text-gray-500">Loading users…</p>
          </div>
        </main>
      </div>
    );
  }

  if (list.length === 0) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navbar />
        <main className="flex-1 flex items-center justify-center">
          <p className="text-gray-600">No users found.</p>
        </main>
      </div>
    );
  }

  return (
    <div className="flex flex-col min-h-screen  w-full bg-base-100">
      <Navbar />
      <main className="p-6 max-w-6xl mx-auto w-full">
        <h1 className="text-2xl font-semibold mb-4">Users</h1>

        {/* Desktop table */}
        <div className="overflow-x-auto hidden md:block shadow rounded-lg">
          <table className="table w-full">
            <thead>
              <tr>
                <th />
                <th className="text-left">Name</th>
                <th className="text-left">Email</th>
                <th>Role</th>
                <th>First login</th>
                <th>Status</th>
                <th>Balance</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {list.map((user) => (
                <tr key={user.email}>
                  <th>
                    <label>
                      <input type="checkbox" className="checkbox" />
                    </label>
                  </th>
                  <td>{`${user.firstname} ${user.lastname}`}</td>
                  <td className="opacity-90">{user.email}</td>
                  <td>{user.role ? "Administrator" : "Bruger"}</td>
                  <td>{user.firstlogin ? "Yes" : "No"}</td>
                  <td>
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        user.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
                      }`}
                    >
                      {user.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td className="font-medium">{user.balance}</td>
                  <td>
                    <button className="btn btn-sm btn-ghost">Edit</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Mobile stacked cards */}
        <div className="md:hidden space-y-3">
          {list.map((user) => (
            <div key={user.email} className="card card-compact bg-base-200 p-4">
              <div className="flex items-center justify-between">
                <div>
                  <div className="text-lg font-semibold">{`${user.firstname} ${user.lastname}`}</div>
                  <div className="text-sm text-gray-500">{user.email}</div>
                </div>
                <div className="text-right">
                  <div className="text-sm">{user.role ? "Admin" : "User"}</div>
                  <div className="text-xs text-gray-500">{user.firstlogin ? "First login" : ""}</div>
                </div>
              </div>

              <div className="mt-3 flex items-center justify-between">
                <div className="text-sm">
                  <span className={`px-2 py-1 rounded-full text-xs font-medium ${user.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                    {user.isActive ? "Active" : "Inactive"}
                  </span>
                </div>
                <div className="text-sm font-medium">{user.balance}</div>
                <button className="btn btn-sm btn-ghost">Edit</button>
              </div>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
}