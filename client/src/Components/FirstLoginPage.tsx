import logo from "../../resources/Logo1.png";
import { useFirstLogin } from "../hooks/useFirstLogin";

export default function FirstLoginPage() {
    const {
        currentPassword,
        setCurrentPassword,
        newPassword,
        setNewPassword,
        repeatPassword,
        setRepeatPassword,
        error,
        loading,
        handleSubmit,
    } = useFirstLogin();

    return (
        <>
            <div style={styles.container}>
                <form style={styles.form} onSubmit={handleSubmit}>
                    <img src={logo} alt="Club Logo" style={styles.logo} />
                    <h2>Reset Password</h2>

                    <input
                        type="password"
                        placeholder="Current Password"
                        value={currentPassword}
                        onChange={(e) => setCurrentPassword(e.target.value)}
                        style={styles.input}
                        required
                    />

                    <input
                        type="password"
                        placeholder="New Password"
                        value={newPassword}
                        onChange={(e) => setNewPassword(e.target.value)}
                        style={styles.input}
                        required
                    />

                    <input
                        type="password"
                        placeholder="Repeat New Password"
                        value={repeatPassword}
                        onChange={(e) => setRepeatPassword(e.target.value)}
                        style={styles.input}
                        required
                    />

                    {error && <div style={{ color: "red", fontSize: "14px" }}>{error}</div>}

                    <button type="submit" style={styles.button} disabled={loading}>
                        {loading ? "Resetting..." : "Reset Password"}
                    </button>
                </form>
            </div>
        </>
    );
}

const styles: Record<string, React.CSSProperties> = {
    container: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
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
        background: "red",
        color: "white",
        border: "none",
        borderRadius: "8px",
        cursor: "pointer",
        fontSize: "16px",
    },
    logo: {
        width: "200px",
        margin: "auto",
    },
};