import {type FormEvent, useState} from "react";
import { userClient} from "../../api-clients.ts";
import type {RegisterUserDto} from "../../models/ServerAPI.ts";
import {Link} from "react-router-dom";
import {useNavigate} from "react-router";
import {handleSubmit, useUsers} from "../../hooks/useUsers.ts";

export default function AdminPage() {
    const [firstName, setFirstName] = handleSubmit("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState("");
    const navigate = useNavigate();
    const [, setError] = useState<string | null>(null);

    const closeModal = () => {
        const modal = document.getElementById('my_modal_1') as HTMLDialogElement;
        modal.close();
    };

    

    handleSubmit()

    const redirectToUsers = () => {
        navigate("/brugere");
    }


    return (
        <div className="flex flex-col min-h-screen w-full">
            <h1 className="text-3xl font-bold underline">Admin Page</h1>

            {/* Open the modal using document.getElementById('ID').showModal() method */}
            <button className="btn" onClick={()=>document.getElementById('my_modal_1').showModal()}>Opret Bruger</button>
            <button className={"btn"} onClick={redirectToUsers}>Se Brugere</button>
            <dialog id="my_modal_1" className="modal">
                <div className="modal-box">
                    <h3 className="font-bold text-lg">Hello!</h3>
                    <p className="py-4">Udfyld nedestående felter med nødvændige oplysninger for at oprette bruger</p>

                    {/* FIX: Add your form here */}
                    <form id="createUserForm" onSubmit={handleSubmit} className="flex flex-col gap-4">
                        <p className="font-bold text-md">Fornavn:</p>
                        <input type="text" className="input"
                               value={firstName}
                               onChange={(e) => setFirstName(e.target.value)} />

                        <p className="font-bold text-md">Efternavn:</p>
                        <input type="text" className="input"
                               value={lastName}
                               onChange={(e) => setLastName(e.target.value)} />

                        <p className="font-bold text-md">Email:</p>
                        <input type="text" className="input"
                               value={email}
                               onChange={(e) => setEmail(e.target.value)} />

                        <p className="font-bold text-md">Password:</p>
                        <input type="text" className="input"
                               value={password}
                               onChange={(e) => setPassword(e.target.value)} />

                        <p className="font-bold text-md">Rolle:</p>
                        <select className="select" value={role}
                                onChange={(e) => setRole(e.target.value)}>
                            <option value="" disabled>Vælg en rolle</option>
                            <option value="Bruger">Bruger</option>
                            <option value="Administrator">Administrator</option>
                        </select>
                    </form>

                    <div className="modal-action">
                        <form method="dialog">
                            <button className="btn">Annuller</button>
                        </form>

                        {/* This now correctly submits the form above */}
                        <button type="submit" form="createUserForm" className="btn">
                            Opret Bruger
                        </button>
                    </div>
                </div>
            </dialog>

        </div>
    );
}