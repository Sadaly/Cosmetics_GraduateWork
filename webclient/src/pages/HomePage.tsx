import React from "react";
import { Link } from "react-router-dom";

const HomePage: React.FC = () => {
    return (
        <div style={{ padding: "2rem" }}>
            <h1>Главная страница</h1>
            <nav style={{ marginTop: "1rem", display: "flex", flexDirection: "column", gap: "10px" }}>
                <Link to="/patients">Пациенты</Link>
                <Link to="/schedule">Расписание</Link>
            </nav>
        </div>
    );
};

export default HomePage;
