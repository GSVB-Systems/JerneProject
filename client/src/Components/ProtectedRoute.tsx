import { Navigate } from "react-router";
import type {JSX} from "react";

export default function ProtectedRoute({ children }: { children: JSX.Element }) {
    const token = localStorage.getItem("token");

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return children;
}