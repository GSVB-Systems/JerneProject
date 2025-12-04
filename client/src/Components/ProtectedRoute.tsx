import { Navigate } from "react-router";
import type {JSX} from "react";
import {useAtom} from "jotai";
import {tokenAtom} from "../atoms/token.ts";

export default function ProtectedRoute({ children }: { children: JSX.Element }) {
    const [jwt,] = useAtom(tokenAtom);
    const token = jwt;

    if (!token) {
        return <Navigate to="/login" replace />;
    }

    return children;
}