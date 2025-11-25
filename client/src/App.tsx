import './App.css'
import LoginPage from "./Components/LoginPage.tsx";
import Dashboard from "./Components/Dashboard.tsx";
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
                        element: <LoginPage/>
                    },
                    {
                        path: '/dashboard',
                        element: (
                            <ProtectedRoute>
                                <Dashboard/>
                            </ProtectedRoute>
                        )
                    }
                ]
            }
        ])}/>
    )
}
export default App