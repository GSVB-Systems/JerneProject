import logo from "../../resources/LoginLogo.png"; // ← adjust path to your logo file

export default function Dashboard() {
    return (
        <div style={styles.container}>
            {/* Logo in upper left corner */}
            <img src={logo} alt="Logo" style={styles.logo} />

            {/* Main text */}
            <h1 style={styles.title}>Welcome</h1>
        </div>
    );
}

const styles = {
    container: {
        height: "100vh",
        width: "100vw",
        background: "#f5f5f5",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        position: "relative",
    },
    logo: {
        width: "120px",
        position: "absolute",
        top: "20px",
        left: "20px",
    },
    title: {
        fontSize: "48px",
        fontWeight: "bold",
        margin: 0,
    },
};
