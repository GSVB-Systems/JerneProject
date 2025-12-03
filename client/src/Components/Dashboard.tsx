import Navbar from "./Navbar";
import SelectableBoard from "./Board";
import AdminPage from "./AdminPage.tsx";

export default function Dashboard() {
    return (
        <div className="flex flex-col min-h-screen w-full">
            <Navbar/>


            {/*<SelectableBoard/>*/}
            <AdminPage/>

        </div>
    );
};
