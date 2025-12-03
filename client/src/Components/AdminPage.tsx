import {type FormEvent, useState} from "react";

export default function AdminPage() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState("");
    const [error, setError] = useState<string | null>(null);


    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);

        try {
            const res = await fetch("/api/Users/create", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({firstName, lastName, email, password, role}),
            });

            if (!res.ok) {
                setError("Failed to create user");
                return;
            }

            const data = await res.json();
            console.log("User created:", data);
        } catch {
        setError("Network error");
    }
    };

    return (
        <div className="flex flex-col min-h-screen w-full">
            <h1 className="text-3xl font-bold underline">Admin Page</h1>

            {/* Open the modal using document.getElementById('ID').showModal() method */}
            <button className="btn" onClick={()=>document.getElementById('my_modal_1').showModal()}>Opret Bruger</button>
            <dialog id="my_modal_1" className="modal">
                <div className="modal-box">
                    <h3 className="font-bold text-lg">Hello!</h3>
                    <p className="py-4">Udfyld nedestående felter med nødvændige oplysninger for at oprette bruger</p>
                    <div className="flex flex-col gap-4">
                        <p className="font-bold text-md">Fornavn:</p>
                        <input type="text" placeholder="Fornavn" className="input"
                               value={firstName}
                               onChange={(e) => setFirstName(e.target.value)} />
                        <p className="font-bold text-md">Efternavn:</p>
                        <input type="text" placeholder="Efternavn" className="input"value={lastName}
                               onChange={(e) => setLastName(e.target.value)} />
                        <p className="font-bold text-md">Email:</p>
                        <input type="text" placeholder="Email" className="input"value={email}
                               onChange={(e) => setEmail(e.target.value)} />
                        <p className="font-bold text-md">Password:</p>
                        <input type="text" placeholder="Kodenavn" className="input"value={password}
                               onChange={(e) => setPassword(e.target.value)} />
                        <p className="font-bold text-md">Rolle:</p>
                        <select defaultValue="Vælg en rolle" className="select" value={email}
                                onChange={(e) => setRole(e.target.value)}>
                            <option disabled={true}>Vælg en rolle</option>
                            <option>Bruger</option>
                            <option>Administrator</option>
                        </select>
                    </div>
                    <div className="modal-action">
                        <form onSubmit={handleSubmit}>
                            {/* if there is a button in form, it will close the modal */}
                            <button className="btn">Annuller</button>
                            <button type="submit" className="btn" >Opret Bruger</button>
                        </form>
                    </div>
                </div>
            </dialog>
        </div>
    );
}