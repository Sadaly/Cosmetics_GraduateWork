import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

type Patient = {
    id: string;
    fullName: string;
    phone?: string;
    email?: string;
};

const PatientsPage: React.FC = () => {
    const [patients, setPatients] = useState<Patient[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchPatients = async () => {
            try {
                const response = await axios.get("https://localhost:7135/api/Patients/All", { withCredentials: true });
                setPatients(response.data); // ⚡️ предполагаем, что API возвращает массив пациентов
            } catch (err: any) {
                console.error("Ошибка загрузки пациентов", err);
                setError("Не удалось загрузить пациентов");
            } finally {
                setLoading(false);
            }
        };

        fetchPatients();
    }, []);

    const handleDelete = async (id: string) => {
        if (!window.confirm("Удалить пациента?")) return;

        try {
            await axios.delete(`https://localhost:7135/api/Patients/${id}`, { withCredentials: true });
            setPatients((prev) => prev.filter((p) => p.id !== id));
        } catch (err) {
            console.error("Ошибка удаления пациента", err);
            alert("Ошибка при удалении");
        }
    };

    if (loading) return <p style={{ padding: "2rem" }}>Загрузка...</p>;
    if (error) return <p style={{ padding: "2rem", color: "red" }}>{error}</p>;

    return (
        <div style={{ padding: "2rem" }}>
            <h1>Пациенты</h1>
            <button
                onClick={() => navigate("/patients/create")}
                style={{
                    margin: "1rem 0",
                    padding: "8px 12px",
                    border: "none",
                    backgroundColor: "#5cb85c",
                    color: "white",
                    borderRadius: "4px",
                    cursor: "pointer",
                }}
            >
                ➕ Добавить пациента
            </button>

            {patients.length === 0 ? (
                <p>Пациентов пока нет</p>
            ) : (
                <table
                    style={{
                        width: "100%",
                        borderCollapse: "collapse",
                        marginTop: "1rem",
                    }}
                >
                    <thead>
                        <tr>
                            <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                ФИО
                            </th>
                            <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                Телефон
                            </th>
                            <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                Email
                            </th>
                            <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {patients.map((p) => (
                            <tr key={p.id}>
                                <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                    {p.fullName}
                                </td>
                                <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                    {p.phone || "-"}
                                </td>
                                <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                    {p.email || "-"}
                                </td>
                                <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                    <button
                                        onClick={() => navigate(`/patients/${p.id}`)}
                                        style={{
                                            marginRight: "10px",
                                            padding: "4px 8px",
                                            border: "1px solid #0275d8",
                                            backgroundColor: "white",
                                            color: "#0275d8",
                                            borderRadius: "4px",
                                            cursor: "pointer",
                                        }}
                                    >
                                        Открыть
                                    </button>
                                    <button
                                        onClick={() => handleDelete(p.id)}
                                        style={{
                                            padding: "4px 8px",
                                            border: "none",
                                            backgroundColor: "#d9534f",
                                            color: "white",
                                            borderRadius: "4px",
                                            cursor: "pointer",
                                        }}
                                    >
                                        Удалить
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default PatientsPage;
