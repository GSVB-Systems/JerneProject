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
            setError("Den nye adgangskode og gentagne adgangskode stemmer ikke overens");
            return;
        }

        if (!currentPassword || !newPassword) {
            setError("Udfyld venligst alle felter");
            return;
        }

        setLoading(true);

        try {
            const response = await authClient.userChangePassword(getUserIdFromJwt(jwt) ?? "", currentPassword, newPassword,);

            if(!response) {
                setError("Det mislykkedes at ændre adgangskoden - tjek dine oplysninger og prøv igen.");
                return;
            }

            navigate("/");
        } catch {
            setError("Adgangskoden skal indeholde mindst 8 tegn, herunder et stort bogstav, et tal og et specialtegn.");
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

