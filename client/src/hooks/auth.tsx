import { useNavigate } from "react-router-dom";
import { authClient } from "../api-clients";
import type { UserDto, LoginRequest } from "../models/ServerAPI.ts";
import { useAtom } from "jotai";
import { tokenAtom, userInfoAtom } from "../atoms/token";

type AuthHook = {
    user: UserDto | null;
    login: (request: LoginRequest) => Promise<void>;
    logout: () => void;
};

export const useAuth = () => {
    const [_, setJwt] = useAtom(tokenAtom);
    const [user] = useAtom(userInfoAtom);
    const navigate = useNavigate();

    const login = async (request: LoginRequest) => {
        const response = await authClient.login(request);
        setJwt(response.jwt!);
        const jwt = response.jwt!;
        const payloadBase64 = jwt.split(".")[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        if (payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] === "Admin") {
            navigate("/admin");
        } else {
            navigate("/");
        }
    };

    const logout = async () => {
        setJwt(null);
        navigate("/login");
    };

    return {
        user,
        login,
        logout,
    } as AuthHook;
};