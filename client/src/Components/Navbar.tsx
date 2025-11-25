import logo from "../../resources/Logo1.png";

export default function Navbar({ DKK = 0 }) {
    const formattedDKK = new Intl.NumberFormat("da-DK", {
        style: "currency",
        currency: "DKK",
        currencyDisplay: "code",
        minimumFractionDigits: 0,
    }).format(DKK);

    return (
        <div className="navbar bg-base-100 shadow-sm w-full">
            {/* Left side: Logo */}
            <div className="flex-1">
                <a className="btn btn-ghost btn-circle normal-case w-16">
                    <img src={logo} alt="Logo"  />
                </a>
            </div>

            {/* Right side: Points + Burger Menu */}
            <div className="flex-none flex items-center gap-4">

                {/* Points indicator */}
                <div className="btn btn-ghost normal-case text-lg">
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
                    <ul
                        tabIndex={0}
                        className="menu menu-sm dropdown-content bg-base-100 rounded-box z-50 mt-3 w-52 p-2 shadow">
                        <li><a>Profile</a></li>
                        <li><a>Settings</a></li>
                        <li><a>Logout</a></li>
                    </ul>
                </div>
            </div>
        </div>
    );
}
