import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";
import { useAuth } from "../Context/AuthContext";

interface RecentProcedure {
    procedureId: string;
    price: number;
    scheduledDate?: string;
    title: string;
    isComplete: boolean;
    isCancelled: boolean;
}

interface RecentPatient {
    patientId: string;
    fullname: string;
    cardId: string;
    age: number;
    creationDate: string;
}

const navCards = [
    { to: "/patients",  label: "Пациенты",     icon: "♥",  desc: "Карты и история",       accent: "var(--color-primary)" },
    { to: "/schedule",  label: "Расписание",    icon: "◷",  desc: "Записи и процедуры",    accent: "var(--color-accent)" },
    { to: "/analitics", label: "Аналитика",     icon: "◈",  desc: "Статистика клиники",    accent: "var(--color-success)" },
    { to: "/users",     label: "Пользователи",  icon: "⚙",  desc: "Управление доступом",   accent: "var(--color-warning)" },
    { to: "/reference", label: "Справочники",    icon: "📚", desc: "Типы и категории",       accent: "#8b5cf6" },
];

const HomePage: React.FC = () => {
    const [procedures, setProcedures] = useState<RecentProcedure[]>([]);
    const [patients, setPatients] = useState<RecentPatient[]>([]);
    const [loadingProcs, setLoadingProcs] = useState(true);
    const [loadingPats, setLoadingPats] = useState(true);
    const navigate = useNavigate();
    const { user } = useAuth();

    useEffect(() => {
        api.get("/Procedures/Take", { params: { StartIndex: 0, Count: 8 } })
            .then(r => {
                const data = Array.isArray(r.data) ? r.data : [];
                data.sort((a: RecentProcedure, b: RecentProcedure) => {
                    const da = a.scheduledDate ? new Date(a.scheduledDate).getTime() : 0;
                    const db = b.scheduledDate ? new Date(b.scheduledDate).getTime() : 0;
                    return db - da;
                });
                setProcedures(data);
            })
            .catch(() => setProcedures([]))
            .finally(() => setLoadingProcs(false));

        api.get("/Patients/Take", { params: { StartIndex: 0, Count: 5 } })
            .then(r => {
                const data = Array.isArray(r.data) ? r.data : [];
                data.sort((a: RecentPatient, b: RecentPatient) =>
                    new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime()
                );
                setPatients(data);
            })
            .catch(() => setPatients([]))
            .finally(() => setLoadingPats(false));
    }, []);

    const formatDate = (iso?: string) => {
        if (!iso) return "—";
        const d = new Date(iso);
        return isNaN(d.getTime()) ? "—" : d.toLocaleString("ru-RU", {
            day: "2-digit", month: "2-digit", year: "numeric",
            hour: "2-digit", minute: "2-digit",
        });
    };

    const getProcedureStatus = (p: RecentProcedure) => {
        if (p.isCancelled) return { label: "Отменена",  cls: "badge badge-danger" };
        if (p.isComplete)  return { label: "Завершена", cls: "badge badge-success" };
        return                     { label: "Ожидается", cls: "badge badge-warning" };
    };

    const greeting = () => {
        const h = new Date().getHours();
        if (h < 12) return "Доброе утро";
        if (h < 18) return "Добрый день";
        return "Добрый вечер";
    };

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header
                title={`${greeting()}${user?.email ? `, ${user.email}` : ""}`}
                subtitle="Добро пожаловать в систему управления клиникой Cosmetics"
            />

            {/* Nav cards */}
            <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(220px, 1fr))", gap: "1.25rem" }}>
                {navCards.map(({ to, label, icon, desc, accent }, i) => (
                    <Link
                        key={to}
                        to={to}
                        className={`card delay-${i + 1} animate-fade-in`}
                        style={{ textDecoration: "none", padding: "1.75rem", display: "flex", flexDirection: "column", gap: 12 }}
                        onMouseEnter={e => { e.currentTarget.style.transform = "translateY(-3px)"; e.currentTarget.style.boxShadow = "var(--shadow-lg)"; }}
                        onMouseLeave={e => { e.currentTarget.style.transform = ""; e.currentTarget.style.boxShadow = ""; }}
                    >
                        <div style={{
                            width: 48, height: 48, borderRadius: "var(--radius-md)",
                            background: `${accent}18`,
                            display: "flex", alignItems: "center", justifyContent: "center",
                            fontSize: "1.5rem", color: accent,
                        }}>{icon}</div>
                        <div>
                            <p style={{ fontWeight: 700, color: "var(--text-primary)", fontSize: "1rem", marginBottom: 2 }}>{label}</p>
                            <p style={{ fontSize: ".8125rem", color: "var(--text-muted)" }}>{desc}</p>
                        </div>
                        <span style={{ fontSize: ".8125rem", color: accent, fontWeight: 600, marginTop: "auto" }}>Перейти →</span>
                    </Link>
                ))}
            </div>

            {/* Activity */}
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1.25rem" }}>
                {/* Procedures */}
                <Card title="Последние процедуры" subtitle="8 самых актуальных записей"
                    action={<Link to="/schedule" style={{ fontSize: ".8125rem", color: "var(--color-primary)", fontWeight: 600 }}>Все →</Link>}
                >
                    {loadingProcs ? (
                        <Spinner />
                    ) : procedures.length === 0 ? (
                        <p style={{ color: "var(--text-muted)", textAlign: "center", padding: "2rem 0" }}>Нет данных</p>
                    ) : (
                        <ul style={{ listStyle: "none", margin: 0, padding: 0, display: "flex", flexDirection: "column", gap: ".75rem" }}>
                            {procedures.map(p => {
                                const st = getProcedureStatus(p);
                                return (
                                    <li key={p.procedureId} style={{
                                        display: "flex", justifyContent: "space-between", alignItems: "flex-start",
                                        paddingBottom: ".75rem", borderBottom: "1px solid var(--border-subtle)",
                                    }}>
                                        <div>
                                            <p style={{ fontWeight: 600, color: "var(--text-primary)", fontSize: ".9rem", marginBottom: 2 }}>
                                                {p.title || "Процедура"}
                                            </p>
                                            <p style={{ fontSize: ".8rem", color: "var(--text-muted)" }}>
                                                {formatDate(p.scheduledDate)} · {p.price.toLocaleString("ru-RU")} ₽
                                            </p>
                                        </div>
                                        <span className={st.cls}>{st.label}</span>
                                    </li>
                                );
                            })}
                        </ul>
                    )}
                </Card>

                {/* Patients */}
                <Card title="Новые пациенты" subtitle="5 последних добавленных"
                    action={<Link to="/patients" style={{ fontSize: ".8125rem", color: "var(--color-primary)", fontWeight: 600 }}>Все →</Link>}
                >
                    {loadingPats ? (
                        <Spinner />
                    ) : patients.length === 0 ? (
                        <p style={{ color: "var(--text-muted)", textAlign: "center", padding: "2rem 0" }}>Нет данных</p>
                    ) : (
                        <ul style={{ listStyle: "none", margin: 0, padding: 0, display: "flex", flexDirection: "column", gap: ".75rem" }}>
                            {patients.map(p => (
                                <li
                                    key={p.patientId}
                                    onClick={() => navigate(`/patients/${p.cardId}`)}
                                    style={{
                                        display: "flex", alignItems: "center", gap: "1rem",
                                        paddingBottom: ".75rem", borderBottom: "1px solid var(--border-subtle)",
                                        cursor: "pointer", borderRadius: "var(--radius-sm)",
                                        transition: "background var(--transition-fast)", padding: ".5rem",
                                    }}
                                    onMouseEnter={e => (e.currentTarget.style.background = "var(--bg-muted)")}
                                    onMouseLeave={e => (e.currentTarget.style.background = "transparent")}
                                >
                                    <div style={{
                                        width: 36, height: 36, borderRadius: "50%",
                                        background: "var(--color-primary-light)",
                                        display: "flex", alignItems: "center", justifyContent: "center",
                                        color: "var(--color-primary)", fontWeight: 700, fontSize: ".875rem", flexShrink: 0,
                                    }}>
                                        {(p.fullname || "П")[0].toUpperCase()}
                                    </div>
                                    <div style={{ flex: 1, minWidth: 0 }}>
                                        <p style={{ fontWeight: 600, color: "var(--text-primary)", fontSize: ".9rem", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>
                                            {p.fullname || "—"}
                                        </p>
                                        <p style={{ fontSize: ".8rem", color: "var(--text-muted)" }}>
                                            {p.age ? `${p.age} лет · ` : ""}Добавлен {formatDate(p.creationDate)}
                                        </p>
                                    </div>
                                    <span style={{ color: "var(--text-muted)", fontSize: ".875rem" }}>›</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </Card>
            </div>
        </div>
    );
};

export default HomePage;
