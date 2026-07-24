import React, { useEffect, useState, useCallback } from "react";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Button } from "../Components/UI/Button";
import { Input } from "../Components/UI/Input";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";

interface DoctorItem { id: string; name: string; }

const PAGE_SIZE = 10;

const DoctorsPage: React.FC = () => {
    const [items, setItems]           = useState<DoctorItem[]>([]);
    const [loading, setLoading]       = useState(true);
    const [error, setError]           = useState<string | null>(null);
    const [hasMore, setHasMore]       = useState(true);
    const [startIndex, setStartIndex] = useState(0);
    const currentPage = Math.floor(startIndex / PAGE_SIZE) + 1;

    const [draftSearch, setDraftSearch] = useState("");
    const [search, setSearch]           = useState("");

    const [newName, setNewName]   = useState("");
    const [creating, setCreating] = useState(false);

    const [editingId, setEditingId]   = useState<string | null>(null);
    const [editName, setEditName]     = useState("");
    const [saving, setSaving]         = useState(false);
    const [deletingId, setDeletingId] = useState<string | null>(null);

    const load = useCallback(async (start: number, name: string) => {
        setLoading(true); setError(null);
        try {
            const params: Record<string, any> = { StartIndex: start, Count: PAGE_SIZE };
            if (name.trim()) params.Name = name.trim();
            const r = await api.get("/Doctors/Take", { params });
            const data: DoctorItem[] = Array.isArray(r.data) ? r.data : [];
            setItems(data);
            setHasMore(data.length === PAGE_SIZE);
        } catch { setError("Не удалось загрузить список врачей"); }
        finally  { setLoading(false); }
    }, []);

    useEffect(() => { load(startIndex, search); }, [startIndex, search, load]);

    const handleSearch = (e: React.FormEvent) => { e.preventDefault(); setSearch(draftSearch); setStartIndex(0); };
    const handleClear  = () => { setDraftSearch(""); setSearch(""); setStartIndex(0); };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newName.trim()) return;
        setCreating(true);
        try {
            await api.post("/Doctors", { name: newName.trim() });
            setNewName("");
            setStartIndex(0); load(0, search);
        } catch { setError("Ошибка при создании врача"); }
        finally   { setCreating(false); }
    };

    const startEdit = (item: DoctorItem) => { setEditingId(item.id); setEditName(item.name); };

    const handleUpdate = async (id: string) => {
        if (!editName.trim()) return;
        setSaving(true);
        try {
            await api.put("/Doctors", { doctorId: id, name: editName.trim() });
            setEditingId(null);
            load(startIndex, search);
        } catch { setError("Ошибка при обновлении"); }
        finally   { setSaving(false); }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Удалить врача? Это не затронет уже созданные процедуры.")) return;
        setDeletingId(id);
        try {
            await api.delete(`/Doctors/${id}`);
            setItems(prev => prev.filter(i => i.id !== id));
        } catch { setError("Ошибка при удалении"); }
        finally   { setDeletingId(null); }
    };

    const paginationBar = (
        <div style={{ display: "flex", alignItems: "center", gap: ".375rem" }}>
            {startIndex > 0 && (
                <Button size="sm" variant="outline" title="В начало" onClick={() => setStartIndex(0)}>⟳</Button>
            )}
            <Button size="sm" variant="outline" disabled={startIndex === 0}
                onClick={() => setStartIndex(Math.max(0, startIndex - PAGE_SIZE))}>← Назад</Button>
            <span style={{ fontSize: ".8125rem", fontWeight: 600, color: "var(--text-secondary)", minWidth: "5rem", textAlign: "center" }}>
                Стр. {currentPage}
            </span>
            <Button size="sm" variant="outline" disabled={!hasMore}
                onClick={() => setStartIndex(startIndex + PAGE_SIZE)}>Вперёд →</Button>
        </div>
    );

    const inputStyle: React.CSSProperties = {
        width: "100%", padding: ".625rem 1rem",
        border: "1.5px solid var(--color-primary)", borderRadius: "var(--radius-md)",
        fontSize: ".9375rem", outline: "none",
        fontFamily: "'Nunito', sans-serif",
        background: "var(--bg-surface)", color: "var(--text-primary)",
    };

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header title="Врачи" subtitle="Управление персоналом клиники" />

            {/* Create */}
            <Card title="Добавить врача">
                <form onSubmit={handleCreate} style={{ display: "flex", gap: "1rem", alignItems: "flex-end", flexWrap: "wrap" }}>
                    <div style={{ flex: "1 1 280px" }}>
                        <Input
                            label="ФИО врача"
                            placeholder="Петрова Наталья Ивановна"
                            value={newName}
                            onChange={e => setNewName(e.target.value)}
                        />
                    </div>
                    <div style={{ paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="success" isLoading={creating}>+ Добавить</Button>
                    </div>
                </form>
            </Card>

            {/* Search */}
            <Card title="Поиск врача">
                <form onSubmit={handleSearch} style={{ display: "flex", gap: "1rem", alignItems: "flex-end" }}>
                    <div style={{ flex: 1 }}>
                        <Input
                            label="Поиск по ФИО"
                            placeholder="Введите имя..."
                            value={draftSearch}
                            onChange={e => setDraftSearch(e.target.value)}
                        />
                    </div>
                    <div style={{ display: "flex", gap: ".75rem", paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="primary">🔍 Найти</Button>
                        <Button type="button" variant="outline" onClick={handleClear}>✖ Очистить</Button>
                    </div>
                </form>
            </Card>

            {/* List */}
            <Card
                title="Список врачей"
                subtitle={`Страница ${currentPage}${search ? ` · поиск: «${search}»` : ""}`}
                noPadding
                action={paginationBar}
            >
                {error && (
                    <div style={{ padding: "1rem 2rem", color: "var(--color-danger)", fontSize: ".875rem" }}>{error}</div>
                )}
                {loading ? (
                    <div style={{ padding: "3rem" }}><Spinner /></div>
                ) : items.length === 0 ? (
                    <p style={{ padding: "2rem", textAlign: "center", color: "var(--text-muted)" }}>
                        {search ? `По запросу «${search}» ничего не найдено` : "Нет врачей. Добавьте первого выше."}
                    </p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>ФИО врача</th>
                                <th style={{ width: 220, textAlign: "right" }}>Действия</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items.map((item, idx) => (
                                <tr key={item.id}>
                                    <td style={{ color: "var(--text-muted)", fontSize: ".8rem", width: 40 }}>
                                        {startIndex + idx + 1}
                                    </td>
                                    <td>
                                        {editingId === item.id ? (
                                            <input
                                                autoFocus
                                                value={editName}
                                                onChange={e => setEditName(e.target.value)}
                                                onKeyDown={e => {
                                                    if (e.key === "Enter")  handleUpdate(item.id);
                                                    if (e.key === "Escape") setEditingId(null);
                                                }}
                                                style={inputStyle}
                                            />
                                        ) : (
                                            <div style={{ display: "flex", alignItems: "center", gap: ".75rem" }}>
                                                <div style={{
                                                    width: 34, height: 34, borderRadius: "50%", flexShrink: 0,
                                                    background: "var(--color-primary-light)",
                                                    display: "flex", alignItems: "center", justifyContent: "center",
                                                    color: "var(--color-primary)", fontWeight: 700, fontSize: ".875rem",
                                                }}>
                                                    {(item.name || "?")[0].toUpperCase()}
                                                </div>
                                                <span style={{ fontWeight: 600, color: "var(--text-primary)" }}>{item.name}</span>
                                            </div>
                                        )}
                                    </td>
                                    <td style={{ textAlign: "right" }}>
                                        <div style={{ display: "flex", gap: ".5rem", justifyContent: "flex-end" }}>
                                            {editingId === item.id ? (
                                                <>
                                                    <Button size="sm" variant="success" isLoading={saving}
                                                        onClick={() => handleUpdate(item.id)}>✓ Сохранить</Button>
                                                    <Button size="sm" variant="outline" onClick={() => setEditingId(null)}>Отмена</Button>
                                                </>
                                            ) : (
                                                <>
                                                    <Button size="sm" variant="outline" onClick={() => startEdit(item)}>✎ Изменить</Button>
                                                    <Button size="sm" variant="danger"
                                                        isLoading={deletingId === item.id}
                                                        onClick={() => handleDelete(item.id)}>Удалить</Button>
                                                </>
                                            )}
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

export default DoctorsPage;
