import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import type { Patient } from "../TypesFromServer/Patient";

const PatientsPage: React.FC = () => {
    const [patients, setPatients] = useState<Patient[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchPatients = async () => {
            try {
                const response = await axios.get("https://localhost:7135/api/Patients/All", { withCredentials: true });
                setPatients(response.data);
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
            setPatients((prev) => prev.filter((p) => p.patientId !== id));
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
                <p>Пациентов пока нет</p>) : (
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
                                    Возраст
                                </th>
                                <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                    Телефон
                                </th>
                                <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                    Жалобы
                                </th>
                                <th style={{ borderBottom: "1px solid #ddd", textAlign: "left", padding: "8px" }}>
                                    Адрес
                                </th>

                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Действия</th>
                            </tr>
                        </thead>
                        <tbody>
                            {patients.map((p) => (
                                <tr key={p.patientId}>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        {p.fullname}
                                    </td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        {p.age}
                                    </td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        {p.phoneNumber}
                                    </td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        {p.complaints}
                                    </td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        {p.address}
                                    </td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        <button
                                            onClick={() => navigate(`/patients/${p.patientId}`)}
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
                                            onClick={() => handleDelete(p.patientId)}
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
