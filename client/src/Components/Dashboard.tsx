import Navbar from "./Navbar";
import SelectableBoard from "./Board";
import AdminPage from "./AdminPage";
import { useAtom } from "jotai";
import { tokenAtom } from "../atoms/token";

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

export default function Dashboard() {
    const [jwt] = useAtom(tokenAtom);
    const role = getRoleFromJwt(jwt);
    const isAdmin = role === "Administrator";

    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar />
            {isAdmin ? <AdminPage /> : <SelectableBoard />}
        </div>
    );
}