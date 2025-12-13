import { TOKEN_KEY, tokenStorage } from "./atoms/token.ts";
import { BoardClient, TransactionClient, UsersClient } from "./models/ServerAPI.ts";

const customFetch = async (url: RequestInfo, init?: RequestInit) => {
    const token = tokenStorage.getItem(TOKEN_KEY, null);

    if (token) {
        // Copy of existing init or new object, with copy of existing headers or
        // new object including Bearer token.
        init = {
            ...(init ?? {}),
            headers: {
                ...(init?.headers ?? {}),
                Authorization: `Bearer ${token}`,
            },
        };
    }
    return await fetch(url, init);
};

const baseUrl = undefined;
export const userClient = new UsersClient(baseUrl, { fetch: customFetch });
export const transactionClient = new TransactionClient(baseUrl, { fetch: customFetch });
export const boardClient = new BoardClient(baseUrl, { fetch: customFetch });