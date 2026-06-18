import React from "react";
import { Link, Outlet, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";

const navLinks = [
    { to: "/home", label: "Главная" },
    { to: "/patients", label: "Пациенты" },
    { to: "/schedule", label: "Расписание" },
    { to: "/analitics", label: "Аналитика" },
    { to: "/users", label: "Пользователи" },
];

const MainLayout: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { logout } = useAuth();

    const handleLogout = () => {
        logout();
        navigate("/");
    };

    return (
        <div>
            <header style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                padding: "0 2rem",
                height: 56,
                backgroundColor: "#1e293b",
                borderBottom: "1px solid #334155",
            }}>
                <nav style={{ display: "flex", gap: 4 }}>
                    {navLinks.map(({ to, label }) => {
                        const active = location.pathname === to || (to !== "/home" && location.pathname.startsWith(to));
                        return (
                            <Link
                                key={to}
                                to={to}
                                style={{
                                    padding: "6px 14px",
                                    borderRadius: 6,
                                    textDecoration: "none",
                                    fontSize: "0.9rem",
                                    fontWeight: active ? 600 : 400,
                                    color: active ? "#fff" : "#94a3b8",
                                    background: active ? "#334155" : "transparent",
                                    transition: "background 0.15s, color 0.15s",
                                }}
                            >
                                {label}
                            </Link>
                        );
                    })}
                </nav>

                <button
                    onClick={handleLogout}
                    style={{
                        padding: "6px 14px",
                        border: "1px solid #475569",
                        backgroundColor: "transparent",
                        color: "#94a3b8",
                        borderRadius: 6,
                        cursor: "pointer",
                        fontSize: "0.85rem",
                    }}
                >
                    Выйти
                </button>
            </header>

            <main style={{ minHeight: "calc(100vh - 56px)", background: "#f8fafc" }}>
                <Outlet />
            </main>
        </div>
    );
};

export default MainLayout;
