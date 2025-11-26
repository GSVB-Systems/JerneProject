import Navbar from "./Navbar.tsx";
import SelectableBoard from "./Board.tsx";

export default function Dashboard() {
    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar/>
            <SelectableBoard/>
        </div>
    );
};
