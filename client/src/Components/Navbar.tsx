import logo from "../../resources/Logo1.png";
import { Link, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { useBalance } from "../hooks/useNavbar";

export default function Navbar() {
    const { loadUserBalance } = useBalance();
    const [balance, setBalance] = useState<number>(0);
    const navigate = useNavigate();

    useEffect(() => {
        loadUserBalance().then(b => {
            if (b !== undefined) setBalance(b);
        });
    }, [loadUserBalance]);

    const formattedDKK = new Intl.NumberFormat("da-DK", {
        style: "currency",
        currency: "DKK",
        currencyDisplay: "code",
        minimumFractionDigits: 0,
    }).format(balance);

    return (
        <div className="navbar bg-base-100 shadow-sm w-full">
            <div className="flex-1">
                <a href="/" className="btn btn-ghost btn-circle normal-case w-16">
                    <img src={logo} alt="Logo" />
                </a>
            </div>

            <div className="flex-none flex items-center gap-4">
                <div
                    className="btn btn-ghost normal-case text-lg"
                    onClick={() => navigate("/transactions")}
                >
                    {formattedDKK}
                </div>

                <div className="dropdown dropdown-end">
                    <div tabIndex={0} role="button" className="btn btn-ghost btn-circle">
                        ☰
                    </div>

                    <ul className="menu menu-sm dropdown-content bg-base-100 rounded-box z-50 mt-3 w-52 p-2 shadow">
                        <li><Link to="/profile">Profil</Link></li>
                        <li><Link to="/aktiveboards">Aktive Boards</Link></li>
                        <li><Link to="/boardhistory">Board Historik</Link></li>
                        <li>
                            <Link
                                to="/login"
                                onClick={() => sessionStorage.clear()}
                            >
                                Log ud
                            </Link>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    );
}
