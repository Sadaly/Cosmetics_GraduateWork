import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import type { PatientCard } from "../TypesFromServer/PatientCard";
import { count } from "node:console";

const PatientsPage: React.FC = () => {
    const [patients, setPatients] = useState<PatientCard[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [newPatientName, setNewPatientName] = useState("");
    const [saving, setSaving] = useState(false);
    const [startIndex, setStartIndex] = useState(0);
    const [pageSize, setPageSize] = useState(3);
    const [hasMore, setHasMore] = useState(true);
    const [searchName, setSearchName] = useState("");
    const [creationDateFrom, setCreationDateFrom] = useState("");
    const [creationDateTo, setCreationDateTo] = useState("");
    const navigate = useNavigate();

    const currentPage = Math.floor(startIndex / pageSize) + 1;

    const fetchPatients = async (start: number, count: number) => {
        setLoading(true);
        try {
            const response = await axios.get("https://localhost:7135/api/Patients/Take", {
                params: {
                    Fullname: searchName || undefined,
                    CreationDateFrom: creationDateFrom || undefined,
                    CreationDateTo: creationDateTo || undefined,
                    StartIndex: start,
                    Count: count,
                },
                withCredentials: true,
            });
            const data: PatientCard[] = response.data;
            setPatients(data);
            setHasMore(data.length === count);
        } catch (err) {
            console.error("Ошибка загрузки пациентов", err);
            setError("Не удалось загрузить пациентов");
        } finally {
            setLoading(false);
        }
    };

    // Load data when pagination, filters, or search changes
    useEffect(() => {
        fetchPatients(startIndex, pageSize);
    }, [startIndex, pageSize, searchName, creationDateFrom, creationDateTo]);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        setStartIndex(0); // reset to first page on new search
        fetchPatients(0, pageSize);
    };

    const handleCreatePatient = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newPatientName.trim()) return alert("Введите ФИО пациента");
        setSaving(true);
        try {
            const response = await axios.post(
                "https://localhost:7135/api/Patients",
                { fullname: newPatientName },
                { withCredentials: true }
            );
            const newId = response.data;
            if (!newId) throw new Error("Patient ID not returned from server");

            const fullPatientRes = await axios.get(
                `https://localhost:7135/api/Patients/${newId}`,
                { withCredentials: true }
            );

            if (!hasMore && fullPatientRes.data < pageSize)
                setPatients((prev) => [fullPatientRes.data, ...prev]);
            setNewPatientName("");
        } catch (err) {
            console.error("Ошибка при создании пациента", err);
            alert("Не удалось создать пациента");
        } finally {
            setSaving(false);
        }
    };

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

    const handlePageSizeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newSize = Number(e.target.value);
        setPageSize(newSize);
        setStartIndex(0);
    };

    if (loading) return <p style={{ padding: "2rem" }}>Загрузка...</p>;
    if (error) return <p style={{ padding: "2rem", color: "red" }}>{error}</p>;

    return (
        <div style={{ padding: "2rem" }}>
            <h1>Пациенты</h1>

            {/* Add new patient */}
            <form onSubmit={handleCreatePatient} style={{ display: "flex", gap: "10px", marginBottom: "1.5rem" }}>
                <input
                    type="text"
                    placeholder="Введите ФИО"
                    value={newPatientName}
                    onChange={(e) => setNewPatientName(e.target.value)}
                    style={{
                        padding: "8px",
                        border: "1px solid #ccc",
                        borderRadius: "4px",
                        flex: "1",
                        maxWidth: "300px",
                    }}
                />
                <button
                    type="submit"
                    disabled={saving}
                    style={{
                        padding: "8px 12px",
                        backgroundColor: "#5cb85c",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                        cursor: saving ? "wait" : "pointer",
                    }}
                >
                    {saving ? "Добавление..." : "Добавить"}
                </button>
            </form>

            {/* Search form */}
            <form
                onSubmit={handleSearch}
                style={{
                    display: "flex",
                    gap: "10px",
                    flexWrap: "wrap",
                    marginBottom: "1.5rem",
                    alignItems: "center",
                }}
            >
                <input
                    type="text"
                    placeholder="Поиск по ФИО"
                    value={searchName}
                    onChange={(e) => setSearchName(e.target.value)}
                    style={{
                        padding: "8px",
                        border: "1px solid #ccc",
                        borderRadius: "4px",
                        width: "200px",
                    }}
                />
                <label>
                    С:
                    <input
                        type="date"
                        value={creationDateFrom}
                        onChange={(e) => setCreationDateFrom(e.target.value)}
                        style={{ marginLeft: "5px" }}
                    />
                </label>
                <label>
                    По:
                    <input
                        type="date"
                        value={creationDateTo}
                        onChange={(e) => setCreationDateTo(e.target.value)}
                        style={{ marginLeft: "5px" }}
                    />
                </label>
                <button
                    type="submit"
                    style={{
                        padding: "8px 12px",
                        backgroundColor: "#0275d8",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                        cursor: "pointer",
                    }}
                >
                    🔍 Найти
                </button>
                <button
                    type="button"
                    onClick={() => {
                        setSearchName("");
                        setCreationDateFrom("");
                        setCreationDateTo("");
                        setStartIndex(0);
                    }}
                    style={{
                        padding: "8px 12px",
                        backgroundColor: "#6c757d",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                    }}
                >
                    ✖ Очистить
                </button>
            </form>

            {/* Page size slider */}
            <div style={{ marginBottom: "1rem" }}>
                <label htmlFor="pageSize" style={{ marginRight: "10px" }}>
                    Кол-во пациентов на странице: <strong>{pageSize}</strong>
                </label>
                <input
                    id="pageSize"
                    type="range"
                    min="1"
                    max="20"
                    value={pageSize}
                    onChange={handlePageSizeChange}
                    style={{ verticalAlign: "middle" }}
                />
            </div>

            {patients.length === 0 ? (
                <p>Пациенты не найдены</p>
            ) : (
                <>
                    <table style={{ width: "100%", borderCollapse: "collapse" }}>
                        <thead>
                            <tr>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>ФИО</th>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Возраст</th>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Телефон</th>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Жалобы</th>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Адрес</th>
                                <th style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>Действия</th>
                            </tr>
                        </thead>
                        <tbody>
                            {patients.map((p) => (
                                <tr key={p.patientId}>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>{p.fullname || "-"}</td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>{p.age ?? "-"}</td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>{p.phoneNumber ?? "-"}</td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>{p.complaints ?? "-"}</td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>{p.address ?? "-"}</td>
                                    <td style={{ borderBottom: "1px solid #ddd", padding: "8px" }}>
                                        <button onClick={() => navigate(`/patients/${p.patientId}`)} style={{ marginRight: "10px" }}>
                                            Открыть
                                        </button>
                                        <button
                                            onClick={() => handleDelete(p.patientId)}
                                            style={{ backgroundColor: "#d9534f", color: "white" }}
                                        >
                                            Удалить
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {/* Pagination Controls */}
                    <div
                        style={{
                            marginTop: "1rem",
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            gap: "12px",
                        }}
                    >
                        {startIndex > 0 && (
                            <button
                                onClick={() => setStartIndex(0)}
                                style={{
                                    backgroundColor: "#0275d8",
                                    color: "white",
                                    borderRadius: "4px",
                                    padding: "4px 10px",
                                }}
                            >
                                ⟳ В начало
                            </button>
                        )}
                        <button
                            disabled={startIndex === 0}
                            onClick={() => setStartIndex(Math.max(0, startIndex - pageSize))}
                        >
                            ← Назад
                        </button>
                        <span style={{ fontWeight: "bold" }}>Страница {currentPage}</span>
                        <button
                            disabled={!hasMore}
                            onClick={() => setStartIndex(startIndex + pageSize)}
                        >
                            Вперед →
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default PatientsPage;
