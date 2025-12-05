import { useEffect, useState} from "react";
import {userClient} from "../api-clients.ts";



export const useUsers = () => {

    const [users, setUsers] = useState<any[]>([]);

    useEffect(() => {
        const fetchUsers = async () => {
            const response = await userClient.getAll();


            // Get the text content and parse it
            const textData = await response.data?.text();
            const parsedUsers = JSON.parse(textData);

            setUsers(parsedUsers);
        };

        fetchUsers();

    }, []);
    return users;
}

