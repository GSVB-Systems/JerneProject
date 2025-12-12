import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router";
import {authClient} from "../api-clients.ts";
import {useJWT} from "./useJWT.ts";

export function useFirstLogin() {
    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [repeatPassword, setRepeatPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const jwt = useJWT();


    function getUserIdFromJwt(jwt: string | null | undefined): string | null {
        if (!jwt) return null;
        try {
            const payloadBase64 = jwt.split(".")[1];
            const payloadJson = atob(payloadBase64);
            const payload = JSON.parse(payloadJson);
            return payload["sub"] ?? null;
        } catch {
            return null;
        }
    }



    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);


        if (newPassword !== repeatPassword) {
            setError("New passwords do not match");
            return;
        }

        if (!currentPassword || !newPassword) {
            setError("Please fill all fields");
            return;
        }

        setLoading(true);

        try {
            const response = await authClient.userChangePassword(getUserIdFromJwt(jwt) ?? "", currentPassword, newPassword,);

            if(!response) {
                setError("Det mislykkedes at ændre adgangskoden");
                return;
            }

            navigate("/");
        } catch {
            setError("Network error");
        } finally {
            setLoading(false);
        }
    };

    return {
        currentPassword,
        setCurrentPassword,
        newPassword,
        setNewPassword,
        repeatPassword,
        setRepeatPassword,
        error,
        loading,
        handleSubmit,
    };
}

