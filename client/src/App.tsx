import './App.css'
import LoginPage from "./Components/LoginPage.tsx";
import Dashboard from "./Components/Dashboard.tsx";
import BoardHistory from "./Components/BoardHistory.tsx";
import ProtectedRoute from "./Components/ProtectedRoute.tsx";
import { createBrowserRouter, Outlet, RouterProvider } from "react-router";

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
                        path: '/login',
                        element: <LoginPage/>
                    }
                ]
            }
        ])}/>
    )
}
export default App