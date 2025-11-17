import { useMemo } from "react";

export function useEnv() {
    return useMemo(() => {
        return {
            apiUrl: import.meta.env.VITE_API_URL,
        };
    }, []);
}