import { Navigate } from "react-router";
import type {JSX} from "react";
import {useJWT} from "../../hooks/useJWT.ts";

export default function ProtectedRoute({ children }: { children: JSX.Element }) {
    const jwt = useJWT();
    const token = jwt;

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return children;
}