import React, { useEffect, useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import type { PatientCard } from "../TypesFromServer/PatientCard";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Button } from "../Components/UI/Button";
import { Input } from "../Components/UI/Input";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";

const PatientsPage: React.FC = () => {
    const [patients, setPatients]         = useState<PatientCard[]>([]);
    const [loading, setLoading]           = useState(true);
    const [error, setError]               = useState<string | null>(null);
    const [newPatientName, setNewName]    = useState("");
    const [saving, setSaving]             = useState(false);
    const [startIndex, setStartIndex]     = useState(0);
    const [pageSize]                       = useState(10);
    const [hasMore, setHasMore]           = useState(true);

    const [searchName,       setSearchName]       = useState("");
    const [creationDateFrom, setCreationDateFrom] = useState("");
    const [creationDateTo,   setCreationDateTo]   = useState("");
    const [draftName, setDraftName] = useState("");
    const [draftFrom, setDraftFrom] = useState("");
    const [draftTo,   setDraftTo]   = useState("");

    const navigate    = useNavigate();
    const currentPage = Math.floor(startIndex / pageSize) + 1;

    const toUtc = (d: string, eod = false) =>
        d ? new Date(d + (eod ? "T23:59:59Z" : "T00:00:00Z")).toISOString() : "";

    const fetchPatients = useCallback(async (start: number, count: number, name: string, from: string, to: string) => {
        setLoading(true); setError(null);
        try {
            const params: Record<string, any> = { StartIndex: start, Count: count };
            if (name.trim()) params.PatientName     = name.trim();
            if (from)        params.CreationDateFrom = toUtc(from);
            if (to)          params.CreationDateTo   = toUtc(to, true);
            const r = await api.get("/PatientCards/Take", { params });
            const data: PatientCard[] = Array.isArray(r.data) ? r.data : [];
            setPatients(data);
            setHasMore(data.length === count);
        } catch { setError("Не удалось загрузить пациентов"); }
        finally  { setLoading(false); }
    }, []);

    useEffect(() => {
        fetchPatients(startIndex, pageSize, searchName, creationDateFrom, creationDateTo);
    }, [startIndex, pageSize, searchName, creationDateFrom, creationDateTo, fetchPatients]);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        setSearchName(draftName); setCreationDateFrom(draftFrom); setCreationDateTo(draftTo);
        setStartIndex(0);
    };
    const handleClear = () => {
        setDraftName(""); setDraftFrom(""); setDraftTo("");
        setSearchName(""); setCreationDateFrom(""); setCreationDateTo("");
        setStartIndex(0);
    };
    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newPatientName.trim()) return;
        setSaving(true);
        try {
            await api.post("/Patients", { fullname: newPatientName });
            setNewName("");
            fetchPatients(startIndex, pageSize, searchName, creationDateFrom, creationDateTo);
        } catch { alert("Не удалось создать пациента"); }
        finally   { setSaving(false); }
    };
    const handleDelete = async (patientId: string) => {
        if (!window.confirm("Удалить пациента?")) return;
        try {
            await api.delete(`/Patients/${patientId}`);
            setPatients(prev => prev.filter(p => p.patientId !== patientId));
        } catch { alert("Ошибка при удалении"); }
    };

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header title="Пациенты" subtitle="Управление карточками пациентов клиники" />

            {/* Add patient — kept high so it's reachable without scrolling */}
            <Card title="Добавить пациента">
                <form onSubmit={handleCreate} style={{ display: "flex", gap: "1rem", alignItems: "flex-end", flexWrap: "wrap" }}>
                    <div style={{ flex: "1 1 260px" }}>
                        <Input
                            label="ФИО нового пациента"
                            placeholder="Иванова Анна Сергеевна"
                            value={newPatientName}
                            onChange={e => setNewName(e.target.value)}
                        />
                    </div>
                    <div style={{ paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="success" isLoading={saving}>
                            {saving ? "Добавление..." : "+ Добавить"}
                        </Button>
                    </div>
                </form>
            </Card>

            {/* Search */}
            <Card title="Поиск пациентов">
                <form onSubmit={handleSearch} style={{ display: "flex", flexWrap: "wrap", gap: "1rem", alignItems: "flex-end" }}>
                    <div style={{ flex: "1 1 200px" }}>
                        <Input
                            label="ФИО пациента"
                            placeholder="Введите имя..."
                            value={draftName}
                            onChange={e => setDraftName(e.target.value)}
                        />
                    </div>
                    <div style={{ flex: "1 1 160px" }}>
                        <Input label="Добавлен с" type="date" value={draftFrom} onChange={e => setDraftFrom(e.target.value)} />
                    </div>
                    <div style={{ flex: "1 1 160px" }}>
                        <Input label="По" type="date" value={draftTo} onChange={e => setDraftTo(e.target.value)} />
                    </div>
                    <div style={{ display: "flex", gap: ".75rem", paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="primary">🔍 Найти</Button>
                        <Button type="button" variant="outline" onClick={handleClear}>✖ Очистить</Button>
                    </div>
                </form>
            </Card>

            {/* Table — pagination lives in the card header so clicking never jumps */}
            <Card
                title="Список пациентов"
                subtitle={`Страница ${currentPage}`}
                noPadding
                action={
                    <div style={{ display: "flex", alignItems: "center", gap: ".375rem" }}>
                        {startIndex > 0 && (
                            <Button size="sm" variant="outline" onClick={() => setStartIndex(0)}
                                title="В начало">⟳</Button>
                        )}
                        <Button size="sm" variant="outline" disabled={startIndex === 0}
                            onClick={() => setStartIndex(Math.max(0, startIndex - pageSize))}>
                            ← Назад
                        </Button>
                        <span style={{
                            fontSize: ".8125rem", fontWeight: 600,
                            color: "var(--text-secondary)",
                            minWidth: "5rem", textAlign: "center",
                        }}>
                            Стр. {currentPage}
                        </span>
                        <Button size="sm" variant="outline" disabled={!hasMore}
                            onClick={() => setStartIndex(startIndex + pageSize)}>
                            Вперёд →
                        </Button>
                    </div>
                }
            >
                {loading ? (
                    <div style={{ padding: "3rem" }}><Spinner /></div>
                ) : error ? (
                    <p style={{ padding: "2rem", color: "var(--color-danger)" }}>{error}</p>
                ) : patients.length === 0 ? (
                    <p style={{ padding: "2rem", color: "var(--text-muted)", textAlign: "center" }}>Пациенты не найдены</p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                {["ФИО", "Возраст", "Телефон", "Жалобы", "Адрес", ""].map(h => (
                                    <th key={h}>{h}</th>
                                ))}
                            </tr>
                        </thead>
                        <tbody>
                            {patients.map(p => (
                                <tr key={p.patientId}>
                                    <td style={{ fontWeight: 600, color: "var(--text-primary)" }}>{p.fullname || "—"}</td>
                                    <td>{p.age || "—"}</td>
                                    <td>{p.phoneNumber || "—"}</td>
                                    <td style={{ maxWidth: 200, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>{p.complaints || "—"}</td>
                                    <td>{p.address || "—"}</td>
                                    <td style={{ textAlign: "right" }}>
                                        <div style={{ display: "flex", gap: ".5rem", justifyContent: "flex-end" }}>
                                            <Button size="sm" variant="outline" onClick={() => navigate(`/patients/${p.id}`)}>
                                                Открыть
                                            </Button>
                                            <Button size="sm" variant="danger" onClick={() => handleDelete(p.patientId)}>
                                                Удалить
                                            </Button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </Card>
        </div>
    );
};

export default PatientsPage;
