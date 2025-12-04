import { Navigate } from "react-router";
import type {JSX} from "react";
import {useJWT} from "../../hooks/useJWT.ts";


function getRoleFromJwt(jwt: string | null | undefined): string | null {
    if (!jwt) return null;
    try {
        const payloadBase64 = jwt.split(".")[1];
        const payloadJson = atob(payloadBase64);
        const payload = JSON.parse(payloadJson);
        return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? null;
    } catch {
        return null;
    }
}
export default function AdminProtectedRoute({ children }: { children: JSX.Element }) {

    const jwt = useJWT();


    if (getRoleFromJwt(jwt) !== "Administrator") {
        return <Navigate to="/login" replace />;
    }

    return children;
}