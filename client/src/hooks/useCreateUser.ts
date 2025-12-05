import {type FormEvent, useState} from "react";
import type {RegisterUserDto} from "../models/ServerAPI.ts";
import {userClient} from "../api-clients.ts";

import AdminPage from "../Components/AdminPages/AdminPage.tsx";

export const useCreateUser = async (e: FormEvent<HTMLFormElement>) => {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState("");
    const [, setError] = useState<string | null>(null);
    e.preventDefault();
    setError(null);

    try {
        const dto: RegisterUserDto = {
            firstname: firstName,
            lastname: lastName,
            email,
            password,
            role
        };

        userClient.create(dto);

        AdminPage.CloseModal();          // <-- close modal on success
    } catch {
        setError("Network error");
    }
};