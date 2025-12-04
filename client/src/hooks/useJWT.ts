import {TOKEN_KEY} from "../atoms/token.ts";


export const useJWT = () => {
    const jwt = sessionStorage.getItem(TOKEN_KEY);
    return jwt;
};