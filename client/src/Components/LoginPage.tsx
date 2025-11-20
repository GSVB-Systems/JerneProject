import { useState } from "react";
import logo from "../../resources/LoginLogo.png";
import {Outlet, useNavigate} from "react-router";



export default function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();

        console.log("Username:", username);
        console.log("Password:", password);

        navigate('dashboard');

    };

    return <>




        <div style={styles.container}>
            <form style={styles.form} onSubmit={handleSubmit}>
                <img src={logo} alt="Club Logo" style={styles.logo} />
                <h2>Login</h2>

                <input
                    type="username"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    style={styles.input}
                    required
                />

                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    style={styles.input}
                    required
                />

                <button type="submit" style={styles.button}>
                    Login
                </button>
            </form>
        </div>
    </>
}

const styles = {
    container: {
        display: "flex",
        height: "75vh",
        justifyContent: "center",
        alignItems: "center",
        background: "#f5f5f5",
    },
    form: {
        padding: "30px",
        width: "300px",
        background: "#fff",
        borderRadius: "12px",
        boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
        display: "flex",
        flexDirection: "column",
        gap: "15px",
    },
    input: {
        padding: "10px",
        fontSize: "16px",
        borderRadius: "8px",
        border: "1px solid #ccc",
    },
    button: {
        padding: "10px",
        background: "Red",
        color: "white",
        border: "none",
        borderRadius: "8px",
        cursor: "pointer",
        fontSize: "16px",
    },
    logo: {
        width: "120px",
        margin: "0 auto 10px auto",
    },
};
