import "./App.css";
import LoginPage from "./Components/LoginPage.tsx";
import Dashboard from "./Components/Dashboard.tsx";
import BoardHistory from "./Components/BoardHistory.tsx";
import ProtectedRoute from "./Components/ProtectedRouting/ProtectedRoute.tsx";
import AdminProtectedRoute from "./Components/ProtectedRouting/AdminProtectedRouting.tsx";
import { createBrowserRouter, Outlet, RouterProvider } from "react-router";
import ViewUsers from "./Components/AdminPages/ViewUsers.tsx";
import ViewUserDetails from "./Components/AdminPages/ViewUserDetails.tsx";
import FirstLoginPage from "./Components/FirstLoginPage.tsx";
import UserTransactions from "./Components/TransactionPages/UserTransactions.tsx";
import UserProtectedRoute from "./Components/ProtectedRouting/UserProtectedRouting.tsx";
import AdminTransaction from "./Components/AdminPages/AdminTransaction.tsx";

function App() {


    return (
        <RouterProvider router={createBrowserRouter([
            {
                path: "/",
                element: <Outlet/>,
                children: [
                    {
                        index: true,
                        element: (
                            <ProtectedRoute>
                                <Dashboard/>
                            </ProtectedRoute>
                        )
                    },
                    {
                        path: "/boardhistory",
                        element: (
                            <ProtectedRoute>
                                <BoardHistory/>
                            </ProtectedRoute>
                        )
                    },
                    {
                        path: "/transactions",
                        element: (
                            <UserProtectedRoute>
                                <UserTransactions/>
                            </UserProtectedRoute>
                        )
                    },
                    {
                        path: "/admintransactions",
                        element: (
                            <AdminProtectedRoute>
                                <AdminTransaction/>
                            </AdminProtectedRoute>
                        )
                    },
                    {
                        path: "/firstlogin",
                        element: (
                            <ProtectedRoute>
                                <FirstLoginPage/>
                            </ProtectedRoute>
                        )
                    },
                    {
                        path: "/brugere",
                        element: (
                            <AdminProtectedRoute>
                                <ViewUsers/>
                            </AdminProtectedRoute>
                        )
                    },
                    {
                        path: "/brugere/:userId",
                        element: (
                            <AdminProtectedRoute>
                                <ViewUserDetails/>
                            </AdminProtectedRoute>
                        )
                    },
                    {
                        path: "/login",
                        element: <LoginPage/>
                    }
                ]
            }
        ])}/>
    );
}
export default App;