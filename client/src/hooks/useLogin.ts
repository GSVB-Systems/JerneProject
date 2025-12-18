import {type FormEvent, useState } from "react";
import { useNavigate } from "react-router";
import { useAtom } from "jotai";
import { tokenAtom } from "../atoms/token";

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

export function useLogin() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const [, setJwt] = useAtom(tokenAtom);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);
        setIsLoading(true);

        try {
            const normalizedUsername = username.toLowerCase();

            const res = await fetch("/api/Auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username: normalizedUsername, password }),
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

                    const user = (await userRes.json()) as { firstlogin?: boolean };

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
        } finally {
            setIsLoading(false);
        }
    };

    return {
        username,
        password,
        error,
        isLoading,
        setUsername,
        setPassword,
        handleSubmit,
    };
}
