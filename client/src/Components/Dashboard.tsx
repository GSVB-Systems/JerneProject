import Navbar from "./Navbar";
import SelectableBoard from "./Board";

export default function Dashboard() {
    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar/>
            <SelectableBoard/>
        </div>
    );
};
