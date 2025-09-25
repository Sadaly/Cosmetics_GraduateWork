import React from "react";
import { Link, Outlet, useNavigate } from "react-router-dom";

const MainLayout: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        // очистка токена (если используется)
        localStorage.removeItem("authToken");
        navigate("/");
    };

    return (
        <div>
            {/* Header */}
            <header
                style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    padding: "1rem 2rem",
                    backgroundColor: "#f5f5f5",
                    borderBottom: "1px solid #ddd",
                }}
            >
                <nav style={{ display: "flex", gap: "20px" }}>
                    <Link to="/home">Главная</Link>
                    <Link to="/patients">Пациенты</Link>
                    <Link to="/schedule">Расписание</Link>
                </nav>

                <button
                    onClick={handleLogout}
                    style={{
                        padding: "6px 12px",
                        border: "none",
                        backgroundColor: "#d9534f",
                        color: "white",
                        borderRadius: "4px",
                        cursor: "pointer",
                    }}
                >
                    Выйти
                </button>
            </header>

            {/* Контент страницы */}
            <main style={{ padding: "2rem" }}>
                <Outlet />
            </main>
        </div>
    );
};

export default MainLayout;
