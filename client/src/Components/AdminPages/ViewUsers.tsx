import { useEffect, useState } from "react";
import Navbar from "../Navbar.tsx";
import { userClient } from "../../api-clients.ts";

export default function ViewUsers() {
    const [users, setUsers] = useState<any[]>([]);

    useEffect(() => {
        const fetchUsers = async () => {
            const response = await userClient.getAll();


            // Get the text content and parse it
            const textData = await response.data?.text();
            const parsedUsers = JSON.parse(textData);

            setUsers(parsedUsers);
        };

        fetchUsers();
    }, []);




    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar />
            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>First login</th>
                        <th>Status</th>
                        <th>Balance</th>
                    </tr>
                    </thead>
                    <tbody>
                    {users.map((user) => (
                        <tr key={user.email}>
                            <th>
                                <label>
                                    <input type="checkbox" className="checkbox" />
                                </label>
                            </th>
                            <td>{`${user.firstname} ${user.lastname}`}</td>
                            <td>{user.email}</td>
                            <td>{user.role ? "Administrator" : "Bruger"}</td>
                            <td>{user.firstlogin ? "Yes" : "No"}</td>
                            <td>{user.isActive ? "Active" : "Inactive"}</td>
                            <td>{user.balance}</td>
                        </tr>
                    ))}
                    </tbody>

                    <tfoot>
                    <tr>
                        <th></th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>First login</th>
                        <th>Status</th>
                        <th>Balance</th>
                    </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    );
}
