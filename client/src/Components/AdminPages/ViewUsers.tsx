import Navbar from "../Navbar.tsx";
import {useUsers} from "../../hooks/useUsers.ts";

export default function ViewUsers() {

    const users = useUsers()




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
                        <th>Edit</th>
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
                            <td><button className="btn btn-sm">Edit</button></td>
                        </tr>
                    ))}
                    </tbody>
                    
                </table>
            </div>
        </div>
    );
}
