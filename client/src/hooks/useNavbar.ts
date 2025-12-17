import { useJWT } from "./useJWT";
import { userClient } from "../api-clients";
import type { UserDto } from "../models/ServerAPI";

const getUserIdFromJwt = (jwt: string | null | undefined): string | null => {
    if (!jwt) return null;

    try {
        const payloadBase64 = jwt.split(".")[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        return payload.sub ?? null;
    } catch {
        return null;
    }
};

const getRoleFromJwt = (jwt: string | null | undefined): string | null => {
    if (!jwt) return null;

    try {
        const payloadBase64 = jwt.split(".")[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? null;
    } catch {
        return null;
    }
};

export const useBalance = () => {
    const jwt = useJWT();
    const userId = getUserIdFromJwt(jwt);

    const loadUserBalance = async (): Promise<number | undefined> => {
        if (!userId) return undefined;

        const response = await userClient.getById(userId);
        const json = await response.data.text();
        const user = JSON.parse(json) as UserDto;

        return user.balance;
    };

    return {
        loadUserBalance,
        isAdmin: getRoleFromJwt(jwt) === "Administrator",
    };
};
