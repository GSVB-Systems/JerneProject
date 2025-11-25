import Navbar from "./Navbar.tsx";
import SelectableBoard from "./Board.tsx";

export default function Dashboard() {
    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar/>
            <SelectableBoard/>

            <h1 style={styles.title}>Welcome</h1>
        </div>
    );
}

const styles = {
    title: {
        fontSize: "48px",
        fontWeight: "bold",
        margin: 0,
        textAlign: "center"
    },
};
