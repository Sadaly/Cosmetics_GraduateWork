import React, { useEffect, useState, useCallback } from "react";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Button } from "../Components/UI/Button";
import { Input } from "../Components/UI/Input";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";

// ── Types ────────────────────────────────────────────────────────────────────

interface RefType {
    id: string;
    title: string;
}

interface Section {
    key: string;
    label: string;
    icon: string;
    apiBase: string;         // e.g. "HealthCondTypes"
    idField: string;         // field name returned by API, e.g. "healthCondTypeId"
    updateIdField: string;   // field name expected by PUT, e.g. "HealthCondTypeId"
    deleteRoute: string;     // e.g. "HealthCondTypes/{id}"
}

const SECTIONS: Section[] = [
    {
        key: "healthCondTypes",
        label: "Типы состояний здоровья",
        icon: "🩺",
        apiBase: "HealthCondTypes",
        idField: "healthCondTypeId",
        updateIdField: "HealthCondTypeId",
        deleteRoute: "HealthCondTypes",
    },
    {
        key: "skinCareTypes",
        label: "Типы ухода за кожей",
        icon: "✨",
        apiBase: "SkinCareTypes",
        idField: "skinCareTypeId",
        updateIdField: "SkinCareTypeId",
        deleteRoute: "SkinCareTypes",
    },
    {
        key: "skinFeatureTypes",
        label: "Типы особенностей кожи",
        icon: "🔬",
        apiBase: "SkinFeatureTypes",
        idField: "skinFeatureTypeId",
        updateIdField: "SkinFeatureTypeId",
        deleteRoute: "SkinFeatureTypes",
    },
    {
        key: "ageChangeTypes",
        label: "Типы возрастных изменений",
        icon: "⏳",
        apiBase: "AgeChangeTypes",
        idField: "ageChangeTypeId",
        updateIdField: "AgeChangeTypeId",
        deleteRoute: "AgeChangeTypes",
    },
    {
        key: "externalProcedureRecordTypes",
        label: "Типы внешних процедур",
        icon: "📋",
        apiBase: "ExternalProcedureRecordTypes",
        idField: "externalProcedureRecordTypeId",
        updateIdField: "ExternalProcedureRecordTypeId",
        deleteRoute: "ExternalProcedureRecordTypes",
    },
];

// ── Section panel ────────────────────────────────────────────────────────────

const SectionPanel: React.FC<{ section: Section }> = ({ section }) => {
    const [items, setItems]           = useState<RefType[]>([]);
    const [loading, setLoading]       = useState(true);
    const [newTitle, setNewTitle]     = useState("");
    const [creating, setCreating]     = useState(false);
    const [editingId, setEditingId]   = useState<string | null>(null);
    const [editTitle, setEditTitle]   = useState("");
    const [saving, setSaving]         = useState(false);
    const [deletingId, setDeletingId] = useState<string | null>(null);
    const [error, setError]           = useState<string | null>(null);

    const load = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const r = await api.get(`/${section.apiBase}/All`);
            const data = Array.isArray(r.data) ? r.data : [];
            setItems(data.map((d: any) => ({
                id: d[section.idField] ?? d.id,
                title: d.title ?? d.Title ?? "",
            })));
        } catch {
            setError("Не удалось загрузить данные");
        } finally {
            setLoading(false);
        }
    }, [section]);

    useEffect(() => { load(); }, [load]);

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newTitle.trim()) return;
        setCreating(true);
        try {
            await api.post(`/${section.apiBase}`, { title: newTitle.trim() });
            setNewTitle("");
            load();
        } catch { setError("Ошибка при создании"); }
        finally   { setCreating(false); }
    };

    const startEdit = (item: RefType) => {
        setEditingId(item.id);
        setEditTitle(item.title);
    };

    const handleUpdate = async (id: string) => {
        if (!editTitle.trim()) return;
        setSaving(true);
        try {
            await api.put(`/${section.apiBase}`, {
                [section.updateIdField]: id,
                title: editTitle.trim(),
            });
            setEditingId(null);
            load();
        } catch { setError("Ошибка при обновлении"); }
        finally   { setSaving(false); }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Удалить запись?")) return;
        setDeletingId(id);
        try {
            await api.delete(`/${section.deleteRoute}/${id}`);
            setItems(prev => prev.filter(i => i.id !== id));
        } catch { setError("Ошибка при удалении"); }
        finally   { setDeletingId(null); }
    };

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "1.25rem" }}>
            {/* Add new */}
            <Card title="Добавить запись">
                <form onSubmit={handleCreate} style={{ display: "flex", gap: "1rem", alignItems: "flex-end" }}>
                    <div style={{ flex: 1 }}>
                        <Input
                            label="Название"
                            placeholder={`Новый тип...`}
                            value={newTitle}
                            onChange={e => setNewTitle(e.target.value)}
                        />
                    </div>
                    <div style={{ paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="success" isLoading={creating}>
                            + Добавить
                        </Button>
                    </div>
                </form>
            </Card>

            {/* List */}
            <Card title="Список" subtitle={`${items.length} записей`} noPadding>
                {error && (
                    <div style={{ padding: "1rem 2rem", color: "var(--color-danger)", fontSize: ".875rem" }}>{error}</div>
                )}
                {loading ? (
                    <div style={{ padding: "3rem" }}><Spinner /></div>
                ) : items.length === 0 ? (
                    <p style={{ padding: "2rem", textAlign: "center", color: "var(--text-muted)" }}>
                        Нет записей. Добавьте первую выше.
                    </p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>Название</th>
                                <th style={{ width: 200, textAlign: "right" }}>Действия</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items.map(item => (
                                <tr key={item.id}>
                                    <td>
                                        {editingId === item.id ? (
                                            <input
                                                autoFocus
                                                value={editTitle}
                                                onChange={e => setEditTitle(e.target.value)}
                                                onKeyDown={e => {
                                                    if (e.key === "Enter") handleUpdate(item.id);
                                                    if (e.key === "Escape") setEditingId(null);
                                                }}
                                                style={{
                                                    width: "100%", padding: "6px 10px",
                                                    border: "1px solid var(--color-primary)",
                                                    borderRadius: "var(--radius-sm)",
                                                    fontSize: ".9rem", outline: "none",
                                                    background: "var(--bg-base)",
                                                    color: "var(--text-primary)",
                                                }}
                                            />
                                        ) : (
                                            <span style={{ fontWeight: 500, color: "var(--text-primary)" }}>
                                                {item.title}
                                            </span>
                                        )}
                                    </td>
                                    <td style={{ textAlign: "right" }}>
                                        <div style={{ display: "flex", gap: ".5rem", justifyContent: "flex-end" }}>
                                            {editingId === item.id ? (
                                                <>
                                                    <Button size="sm" variant="success" isLoading={saving}
                                                        onClick={() => handleUpdate(item.id)}>
                                                        ✓ Сохранить
                                                    </Button>
                                                    <Button size="sm" variant="outline"
                                                        onClick={() => setEditingId(null)}>
                                                        Отмена
                                                    </Button>
                                                </>
                                            ) : (
                                                <>
                                                    <Button size="sm" variant="outline"
                                                        onClick={() => startEdit(item)}>
                                                        ✎ Изменить
                                                    </Button>
                                                    <Button size="sm" variant="danger"
                                                        isLoading={deletingId === item.id}
                                                        onClick={() => handleDelete(item.id)}>
                                                        Удалить
                                                    </Button>
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

// ── Main page ────────────────────────────────────────────────────────────────

const ReferencePage: React.FC = () => {
    const [activeKey, setActiveKey] = useState(SECTIONS[0].key);
    const activeSection = SECTIONS.find(s => s.key === activeKey)!;

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header
                title="Справочники"
                subtitle="Управление типами и категориями данных клиники"
            />

            {/* Tab bar */}
            <div style={{
                display: "flex", flexWrap: "wrap", gap: ".5rem",
                borderBottom: "1px solid var(--border-subtle)",
                paddingBottom: "1rem",
            }}>
                {SECTIONS.map(s => (
                    <button
                        key={s.key}
                        onClick={() => setActiveKey(s.key)}
                        style={{
                            display: "flex", alignItems: "center", gap: ".5rem",
                            padding: ".5rem 1rem",
                            borderRadius: "var(--radius-md)",
                            border: activeKey === s.key
                                ? "2px solid var(--color-primary)"
                                : "2px solid var(--border-subtle)",
                            background: activeKey === s.key
                                ? "var(--color-primary-light)"
                                : "var(--bg-base)",
                            color: activeKey === s.key
                                ? "var(--color-primary)"
                                : "var(--text-secondary)",
                            fontWeight: activeKey === s.key ? 700 : 500,
                            fontSize: ".875rem",
                            cursor: "pointer",
                            transition: "all var(--transition-fast)",
                        }}
                    >
                        <span>{s.icon}</span>
                        <span>{s.label}</span>
                    </button>
                ))}
            </div>

            {/* Active section */}
            <SectionPanel key={activeKey} section={activeSection} />
        </div>
    );
};

export default ReferencePage;
