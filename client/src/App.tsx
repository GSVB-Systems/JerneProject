import './App.css'
import LoginPage from "./Components/LoginPage.tsx";
import Dashboard from "./Components/Dashboard.tsx";
import BoardHistory from "./Components/BoardHistory.tsx";
import ProtectedRoute from "./Components/ProtectedRouting/ProtectedRoute.tsx";
import AdminProtectedRoute from "./Components/ProtectedRouting/AdminProtectedRouting.tsx";
import { createBrowserRouter, Outlet, RouterProvider } from "react-router";
import ViewUsers from "./Components/AdminPages/ViewUsers.tsx";

function App() {


    return (
        <RouterProvider router={createBrowserRouter([
            {
                path: '/',
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
                        path: '/boardhistory',
                        element: (
                            <ProtectedRoute>
                                <BoardHistory/>
                            </ProtectedRoute>
                        )
                    },
                    {
                        path: '/brugere',
                        element: (
                            <AdminProtectedRoute>
                                <ViewUsers/>
                            </AdminProtectedRoute>
                        )
                    },
                    {
                        path: '/login',
                        element: <LoginPage/>
                    }
                ]
            }
        ])}/>
    )
}
export default App