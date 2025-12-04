import Navbar from "../Navbar.tsx";
import { userClient} from "../../api-clients.ts";




export default function ViewUsers({ users }) {



    const getUsers = async () => {
        const users = userClient.getAll();

    }

    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar />
            <div className="overflow-x-auto">
                <table className="table">
                    <thead>
                    <tr>
                        <th>
                            <label>
                                <input type="checkbox" className="checkbox" />
                            </label>
                        </th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Favorite Color</th>
                        <th></th>
                    </tr>
                    </thead>
                    <tbody>
                    {users && users.map((user, index) => (
                        <tr key={index}>
                            <th>
                                <label>
                                    <input type="checkbox" className="checkbox" />
                                </label>
                            </th>
                            <td>
                                <div className="flex items-center gap-3">
                                    <div className="avatar">
                                        <div className="mask mask-squircle h-12 w-12">
                                            <img
                                                src={user.avatar}
                                                alt={user.name}
                                            />
                                        </div>
                                    </div>
                                    <div>
                                        <div className="font-bold">{user.name}</div>
                                        <div className="text-sm opacity-50">{user.country}</div>
                                    </div>
                                </div>
                            </td>
                            <td>
                                {user.company}
                                <br />
                                <span className="badge badge-ghost badge-sm">{user.email}</span>
                            </td>
                            <td>{user.color}</td>
                            <th>
                                <button className="btn btn-ghost btn-xs">details</button>
                            </th>
                        </tr>
                    ))}
                    </tbody>
                    <tfoot>
                    <tr>
                        <th></th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Favorite Color</th>
                        <th></th>
                    </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    );
}
