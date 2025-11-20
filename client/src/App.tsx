import './App.css'
import LoginPage from "./Components/LoginPage.tsx";
import Dashboard from "./Components/Dashboard.tsx";
import {createBrowserRouter, Outlet, RouterProvider} from "react-router";

function App() {


  return(
    <RouterProvider router={createBrowserRouter([
        {
            path: '/',
            element: <Outlet ></Outlet >,
            children: [
                {
                    path: "/",
                    element: <LoginPage/>

                },
                {

                    path: '/dashboard',
                    element: <Dashboard/>
                }
            ]
        }
    ])}/>
    )
}

export default App
