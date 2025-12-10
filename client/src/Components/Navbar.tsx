import logo from "../../resources/Logo1.png";
import {Link} from "react-router-dom";
import {useNavigate} from "react-router";

export default function Navbar({ DKK = 0 }) {
    const navigate = useNavigate();
    const formattedDKK = new Intl.NumberFormat("da-DK", {
        style: "currency",
        currency: "DKK",
        currencyDisplay: "code",
        minimumFractionDigits: 0,
    }).format(DKK);

    function handlePointsClick() {
        navigate("/transactions");
    }

    return (
        <div className="navbar bg-base-100 shadow-sm w-full">
            {/* Left side: Logo */}
            <div className="flex-1">
                <a href="/" className="btn btn-ghost btn-circle normal-case w-16">
                    <img src={logo} alt="Logo"  />
                </a>
            </div>

            {/* Right side: Points + Burger Menu */}
            <div className="flex-none flex items-center gap-4">

                {/* Points indicator */}
                <div className="btn btn-ghost normal-case text-lg" onClick={handlePointsClick}>
                    {formattedDKK}
                </div>


                {/* Burger Menu */}
                <div className="dropdown dropdown-end">
                    <div tabIndex={0} role="button" className="btn btn-ghost btn-circle">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            className="h-6 w-6"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"
                                  d="M4 6h16M4 12h16M4 18h16" />
                        </svg>
                    </div>

                    {/* Dropdown menu */}
                    <ul tabIndex={0} className="menu menu-sm dropdown-content bg-base-100 rounded-box z-50 mt-3 w-52 p-2 shadow">
                        <li>
                            <Link to="/profile" className="w-full text-left block">
                                Profil
                            </Link>
                        </li>
                        <li>
                            <Link to="/aktiveboards" className="w-full text-left block">
                                Aktive Boards
                            </Link>
                        </li>
                        <li>
                            <Link to="/boardhistory" className="w-full text-left block">
                                Board Historik
                            </Link>
                        </li>
                        <li>
                            <Link to="/login" className="w-full text-left block" onClick={() => sessionStorage.clear()}>
                                Log ud
                            </Link>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    );
}
