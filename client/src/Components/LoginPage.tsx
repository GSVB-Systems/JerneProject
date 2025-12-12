import React, {type FormEvent, useState,} from "react";
import { useNavigate } from "react-router";
import logo from "../../resources/Logo1.png";
import {useAtom} from "jotai";
import {tokenAtom} from "../atoms/token.ts";

function parseJwt(token: string): Record<string, any> | null {
    try {
        const payload = token.split(".")[1];
        const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
        const json = decodeURIComponent(
            atob(base64)
                .split("")
                .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                .join("")
        );
        return JSON.parse(json);
    } catch {
        return null;
    }
}

export default function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    const [, setJwt] = useAtom(tokenAtom);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);

        try {
            const res = await fetch("/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password }),
            });

            if (!res.ok) {
                setError("Invalid credentials");
                return;
            }

            const data = await res.json();

            if (data?.token) {
                setJwt(data.token);

                const payload = parseJwt(data.token);
                const id = payload?.id ?? payload?.sub;
                if (!id) {
                    setError("Token missing user id");
                    return;
                }

                try {
                    const userRes = await fetch(`/api/Users/GetUserByID/${id}`, {
                        method: "GET",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${data.token}`,
                        },
                    });

                    if (!userRes.ok) {
                        setError("Failed to fetch user");
                        return;
                    }

                    const user = await userRes.json() as { firstlogin?: boolean };

                    if (user?.firstlogin === true) {
                        navigate("/firstlogin");
                    } else {
                        navigate("/");
                    }
                } catch {
                    setError("Failed to fetch user");
                }
            } else {
                setError("No token returned");
            }
        } catch {
            setError("Network error");
        }
    };

    return (
        <>
            <div style={styles.container}>
                <form style={styles.form} onSubmit={handleSubmit}>
                    <img src={logo} alt="Club Logo" style={styles.logo} />
                    <h2>Login</h2>
                    <input
                        //type="email"
                        type="text"
                        placeholder="Email"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        style={styles.input}
                        required
                    />
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        style={styles.input}
                        required
                    />
                    {error && <div style={{ color: "red", fontSize: "14px" }}>{error}</div>}
                    <button type="submit" style={styles.button}>Login</button>
                </form>
            </div>
        </>
    );
}

const styles: Record<string, React.CSSProperties> = {
    container: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
    },
    form: {
        padding: "30px",
        width: "300px",
        background: "#fff",
        borderRadius: "12px",
        boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
        display: "flex",
        flexDirection: "column",
        gap: "15px",
    },
    input: {
        padding: "10px",
        fontSize: "16px",
        borderRadius: "8px",
        border: "1px solid #ccc",
    },
    button: {
        padding: "10px",
        background: "red",
        color: "white",
        border: "none",
        borderRadius: "8px",
        cursor: "pointer",
        fontSize: "16px",
    },
    logo: {
        width: "200px",
        margin: "auto",
    },
};
