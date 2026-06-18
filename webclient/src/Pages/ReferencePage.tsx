import React, { useEffect, useState, useCallback } from "react";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Button } from "../Components/UI/Button";
import { Input } from "../Components/UI/Input";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";

// ── Config ────────────────────────────────────────────────────────────────────

interface Section {
    key: string;
    label: string;
    icon: string;
    apiBase: string;
    idField: string;
    updateIdField: string;
}

const SECTIONS: Section[] = [
    { key: "healthCondTypes",               label: "Типы состояний здоровья",  icon: "🩺", apiBase: "HealthCondTypes",               idField: "healthCondTypeId",               updateIdField: "HealthCondTypeId"               },
    { key: "ageChangeTypes",                label: "Типы возрастных изменений",icon: "⏳", apiBase: "AgeChangeTypes",                idField: "ageChangeTypeId",                updateIdField: "AgeChangeTypeId"                },
    { key: "skinFeatureTypes",              label: "Типы особенностей кожи",   icon: "🔬", apiBase: "SkinFeatureTypes",              idField: "skinFeatureTypeId",              updateIdField: "SkinFeatureTypeId"              },
    { key: "skinCareTypes",                 label: "Типы ухода за кожей",      icon: "✨", apiBase: "SkinCareTypes",                 idField: "skinCareTypeId",                 updateIdField: "SkinCareTypeId"                 },
    { key: "externalProcedureRecordTypes",  label: "Типы внешних процедур",    icon: "📋", apiBase: "ExternalProcedureRecordTypes",  idField: "externalProcedureRecordTypeId",  updateIdField: "ExternalProcedureRecordTypeId"  },
];

const PAGE_SIZE = 8;

interface RefType { id: string; title: string; }

// ── Procedure Type Panel ──────────────────────────────────────────────────────

interface ProcedureTypeItem { id: string; title: string; description: string; price: number; duration: number; }

const ProcedureTypePanel: React.FC = () => {
    const [items, setItems]           = useState<ProcedureTypeItem[]>([]);
    const [loading, setLoading]       = useState(true);
    const [error, setError]           = useState<string | null>(null);
    const [hasMore, setHasMore]       = useState(true);
    const [startIndex, setStartIndex] = useState(0);
    const currentPage = Math.floor(startIndex / PAGE_SIZE) + 1;

    const [draftSearch, setDraftSearch] = useState("");
    const [search, setSearch]           = useState("");

    const [newTitle, setNewTitle]       = useState("");
    const [newDescr, setNewDescr]       = useState("");
    const [newDuration, setNewDuration] = useState("");
    const [newPrice, setNewPrice]       = useState("");
    const [creating, setCreating]       = useState(false);

    const [editingId, setEditingId]         = useState<string | null>(null);
    const [editTitle, setEditTitle]         = useState("");
    const [editDescr, setEditDescr]         = useState("");
    const [editDuration, setEditDuration]   = useState("");
    const [editPrice, setEditPrice]         = useState("");
    const [saving, setSaving]               = useState(false);
    const [deletingId, setDeletingId]       = useState<string | null>(null);

    const load = useCallback(async (start: number, typename: string) => {
        setLoading(true); setError(null);
        try {
            const params: Record<string, any> = { StartIndex: start, Count: PAGE_SIZE };
            if (typename.trim()) params.Typename = typename.trim();
            const r = await api.get("/ProcedureTypes/Take", { params });
            const data: ProcedureTypeItem[] = Array.isArray(r.data) ? r.data : [];
            setItems(data);
            setHasMore(data.length === PAGE_SIZE);
        } catch { setError("Не удалось загрузить типы процедур"); }
        finally  { setLoading(false); }
    }, []);

    useEffect(() => { load(startIndex, search); }, [startIndex, search, load]);

    const handleSearch = (e: React.FormEvent) => { e.preventDefault(); setSearch(draftSearch); setStartIndex(0); };
    const handleClear  = () => { setDraftSearch(""); setSearch(""); setStartIndex(0); };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newTitle.trim()) return;
        setCreating(true);
        try {
            await api.post("/ProcedureTypes", {
                title:       newTitle.trim(),
                description: newDescr.trim(),
                standartDur:   parseInt(newDuration) || 0,
                standartPrice: parseInt(newPrice)    || 0,
            });
            setNewTitle(""); setNewDescr(""); setNewDuration(""); setNewPrice("");
            setStartIndex(0); load(0, search);
        } catch { setError("Ошибка при создании"); }
        finally   { setCreating(false); }
    };

    const startEdit = (item: ProcedureTypeItem) => {
        setEditingId(item.id);
        setEditTitle(item.title);
        setEditDescr(item.description ?? "");
        setEditDuration(item.duration ? String(item.duration) : "");
        setEditPrice(item.price ? String(item.price) : "");
    };

    const handleUpdate = async (id: string) => {
        if (!editTitle.trim()) return;
        setSaving(true);
        try {
            await api.put("/ProcedureTypes", {
                procedureTypeId: id,
                title:         editTitle.trim()    || undefined,
                descr:         editDescr.trim()    || undefined,
                duration:      editDuration        ? parseInt(editDuration) : undefined,
                standartPrice: editPrice           ? parseInt(editPrice)    : undefined,
            });
            setEditingId(null);
            load(startIndex, search);
        } catch (e: any) {
            const msg = e?.response?.data?.detail
                ?? e?.response?.data?.message
                ?? (typeof e?.response?.data === "string" ? e.response.data : null)
                ?? "Ошибка при обновлении";
            setError(msg);
        }
        finally { setSaving(false); }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Удалить тип процедуры?")) return;
        setDeletingId(id);
        try {
            await api.delete(`/ProcedureTypes/${id}`);
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
        border: "1.5px solid var(--border-default)", borderRadius: "var(--radius-md)",
        fontSize: ".9375rem", outline: "none",
        fontFamily: "'Nunito', sans-serif",
        background: "var(--bg-surface)", color: "var(--text-primary)",
    };
    const inputActiveStyle: React.CSSProperties = { ...inputStyle, borderColor: "var(--color-primary)" };

    return (
        <div style={{ display: "flex", flexDirection: "column", gap: "1.25rem" }}>

            {/* Create */}
            <Card title="Добавить тип процедуры">
                <form onSubmit={handleCreate} style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
                    <div style={{ display: "flex", flexWrap: "wrap", gap: "1rem" }}>
                        <div style={{ flex: "2 1 220px" }}>
                            <Input label="Название *" placeholder="Гиалуроновая маска, пилинг..." value={newTitle} onChange={e => setNewTitle(e.target.value)} />
                        </div>
                        <div style={{ flex: "1 1 140px" }}>
                            <Input label="Длительность (мин)" type="number" min="0" placeholder="60" value={newDuration} onChange={e => setNewDuration(e.target.value)} />
                        </div>
                        <div style={{ flex: "1 1 140px" }}>
                            <Input label="Цена (₽)" type="number" min="0" placeholder="2000" value={newPrice} onChange={e => setNewPrice(e.target.value)} />
                        </div>
                    </div>
                    <div style={{ display: "flex", flexWrap: "wrap", gap: "1rem", alignItems: "flex-end" }}>
                        <div style={{ flex: "1 1 300px" }}>
                            <Input label="Описание" placeholder="Краткое описание процедуры (необязательно)" value={newDescr} onChange={e => setNewDescr(e.target.value)} />
                        </div>
                        <div style={{ paddingBottom: "1.25rem" }}>
                            <Button type="submit" variant="success" isLoading={creating}>+ Добавить</Button>
                        </div>
                    </div>
                </form>
            </Card>

            {/* Search */}
            <Card title="Поиск">
                <form onSubmit={handleSearch} style={{ display: "flex", gap: "1rem", alignItems: "flex-end" }}>
                    <div style={{ flex: 1 }}>
                        <Input label="Поиск по названию" placeholder="Введите текст..." value={draftSearch} onChange={e => setDraftSearch(e.target.value)} />
                    </div>
                    <div style={{ display: "flex", gap: ".75rem", paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="primary">🔍 Найти</Button>
                        <Button type="button" variant="outline" onClick={handleClear}>✖ Сбросить</Button>
                    </div>
                </form>
            </Card>

            {/* List */}
            <Card
                title="Список типов процедур"
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
                        {search ? `По запросу «${search}» ничего не найдено` : "Нет типов процедур. Добавьте первый выше."}
                    </p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Название</th>
                                <th>Длительность</th>
                                <th>Цена</th>
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
                                            <div style={{ display: "flex", flexDirection: "column", gap: ".375rem" }}>
                                                <input autoFocus value={editTitle} onChange={e => setEditTitle(e.target.value)}
                                                    placeholder="Название" style={inputActiveStyle} />
                                                <input value={editDescr} onChange={e => setEditDescr(e.target.value)}
                                                    placeholder="Описание (необязательно)" style={inputStyle} />
                                            </div>
                                        ) : (
                                            <span style={{ fontWeight: 500, color: "var(--text-primary)" }}>{item.title}</span>
                                        )}
                                    </td>
                                    <td>
                                        {editingId === item.id ? (
                                            <input value={editDuration} onChange={e => setEditDuration(e.target.value)}
                                                type="number" min="0" placeholder="мин"
                                                onKeyDown={e => { if (e.key === "Enter") handleUpdate(item.id); if (e.key === "Escape") setEditingId(null); }}
                                                style={{ ...inputStyle, width: 80 }} />
                                        ) : (
                                            <span style={{ color: "var(--text-secondary)" }}>
                                                {item.duration ? `${item.duration} мин` : "—"}
                                            </span>
                                        )}
                                    </td>
                                    <td>
                                        {editingId === item.id ? (
                                            <input value={editPrice} onChange={e => setEditPrice(e.target.value)}
                                                type="number" min="0" placeholder="₽"
                                                onKeyDown={e => { if (e.key === "Enter") handleUpdate(item.id); if (e.key === "Escape") setEditingId(null); }}
                                                style={{ ...inputStyle, width: 100 }} />
                                        ) : (
                                            <span style={{ fontWeight: 600, color: "var(--color-success)" }}>
                                                {item.price ? `${item.price.toLocaleString("ru-RU")} ₽` : "—"}
                                            </span>
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
                                                    <Button size="sm" variant="danger" isLoading={deletingId === item.id}
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

// ── Section panel ─────────────────────────────────────────────────────────────

const SectionPanel: React.FC<{ section: Section }> = ({ section }) => {
    const [items, setItems]           = useState<RefType[]>([]);
    const [loading, setLoading]       = useState(true);
    const [error, setError]           = useState<string | null>(null);
    const [hasMore, setHasMore]       = useState(true);
    const [startIndex, setStartIndex] = useState(0);

    // Search — draft vs committed
    const [draftSearch, setDraftSearch] = useState("");
    const [search, setSearch]           = useState("");

    // Create
    const [newTitle, setNewTitle] = useState("");
    const [creating, setCreating] = useState(false);

    // Edit
    const [editingId, setEditingId] = useState<string | null>(null);
    const [editTitle, setEditTitle] = useState("");
    const [saving, setSaving]       = useState(false);

    // Delete
    const [deletingId, setDeletingId] = useState<string | null>(null);

    const currentPage = Math.floor(startIndex / PAGE_SIZE) + 1;

    const load = useCallback(async (start: number, typename: string) => {
        setLoading(true);
        setError(null);
        try {
            const params: Record<string, any> = { StartIndex: start, Count: PAGE_SIZE };
            if (typename.trim()) params.Typename = typename.trim();
            const r = await api.get(`/${section.apiBase}/Take`, { params });
            const data: any[] = Array.isArray(r.data) ? r.data : [];
            setItems(data.map(d => ({
                id:    d[section.idField] ?? d.id ?? d.Id,
                title: d.title ?? d.Title ?? "",
            })));
            setHasMore(data.length === PAGE_SIZE);
        } catch {
            setError("Не удалось загрузить данные");
        } finally {
            setLoading(false);
        }
    }, [section]);

    useEffect(() => { load(startIndex, search); }, [startIndex, search, load]);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        setSearch(draftSearch);
        setStartIndex(0);
    };

    const handleClear = () => {
        setDraftSearch("");
        setSearch("");
        setStartIndex(0);
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newTitle.trim()) return;
        setCreating(true);
        try {
            await api.post(`/${section.apiBase}`, { title: newTitle.trim() });
            setNewTitle("");
            // reload first page with current search
            setStartIndex(0);
            load(0, search);
        } catch { setError("Ошибка при создании"); }
        finally   { setCreating(false); }
    };

    const startEdit = (item: RefType) => { setEditingId(item.id); setEditTitle(item.title); };

    const handleUpdate = async (id: string) => {
        if (!editTitle.trim()) return;
        setSaving(true);
        try {
            await api.put(`/${section.apiBase}`, { [section.updateIdField]: id, title: editTitle.trim() });
            setEditingId(null);
            load(startIndex, search);
        } catch { setError("Ошибка при обновлении"); }
        finally   { setSaving(false); }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Удалить запись?")) return;
        setDeletingId(id);
        try {
            await api.delete(`/${section.apiBase}/${id}`);
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
                            placeholder="Введите название нового типа..."
                            value={newTitle}
                            onChange={e => setNewTitle(e.target.value)}
                        />
                    </div>
                    <div style={{ paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="success" isLoading={creating}>+ Добавить</Button>
                    </div>
                </form>
            </Card>

            {/* Search */}
            <Card title="Поиск">
                <form onSubmit={handleSearch} style={{ display: "flex", gap: "1rem", alignItems: "flex-end" }}>
                    <div style={{ flex: 1 }}>
                        <Input
                            label="Поиск по названию"
                            placeholder="Введите текст..."
                            value={draftSearch}
                            onChange={e => setDraftSearch(e.target.value)}
                        />
                    </div>
                    <div style={{ display: "flex", gap: ".75rem", paddingBottom: "1.25rem" }}>
                        <Button type="submit" variant="primary">🔍 Найти</Button>
                        <Button type="button" variant="outline" onClick={handleClear}>✖ Сбросить</Button>
                    </div>
                </form>
            </Card>

            {/* List */}
            <Card
                title="Список"
                subtitle={`Страница ${currentPage}${search ? ` · поиск: «${search}»` : ""}`}
                noPadding
            >
                {error && (
                    <div style={{ padding: "1rem 2rem", color: "var(--color-danger)", fontSize: ".875rem" }}>
                        {error}
                    </div>
                )}
                {loading ? (
                    <div style={{ padding: "3rem" }}><Spinner /></div>
                ) : items.length === 0 ? (
                    <p style={{ padding: "2rem", textAlign: "center", color: "var(--text-muted)" }}>
                        {search ? `По запросу «${search}» ничего не найдено` : "Нет записей. Добавьте первую выше."}
                    </p>
                ) : (
                    <>
                        <table>
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Название</th>
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
                                                    value={editTitle}
                                                    onChange={e => setEditTitle(e.target.value)}
                                                    onKeyDown={e => {
                                                        if (e.key === "Enter")  handleUpdate(item.id);
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
                                                        <Button size="sm" variant="outline" onClick={() => startEdit(item)}>
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

                        {/* Pagination */}
                        <div style={{
                            display: "flex", justifyContent: "center", alignItems: "center",
                            gap: "1rem", padding: "1.25rem",
                            borderTop: "1px solid var(--border-subtle)",
                        }}>
                            {startIndex > 0 && (
                                <Button size="sm" variant="outline" onClick={() => setStartIndex(0)}>
                                    ⟳ В начало
                                </Button>
                            )}
                            <Button size="sm" variant="outline"
                                disabled={startIndex === 0}
                                onClick={() => setStartIndex(Math.max(0, startIndex - PAGE_SIZE))}>
                                ← Назад
                            </Button>
                            <span style={{ fontSize: ".875rem", fontWeight: 600, color: "var(--text-secondary)" }}>
                                Стр. {currentPage}
                            </span>
                            <Button size="sm" variant="outline"
                                disabled={!hasMore}
                                onClick={() => setStartIndex(startIndex + PAGE_SIZE)}>
                                Вперёд →
                            </Button>
                        </div>
                    </>
                )}
            </Card>
        </div>
    );
};

// ── Main page ─────────────────────────────────────────────────────────────────

const PROCEDURE_TYPES_KEY = "procedureTypes";

const ALL_TABS = [
    { key: PROCEDURE_TYPES_KEY, label: "Типы процедур", icon: "💊" },
    ...SECTIONS.map(s => ({ key: s.key, label: s.label, icon: s.icon })),
];

const ReferencePage: React.FC = () => {
    const [activeKey, setActiveKey] = useState(PROCEDURE_TYPES_KEY);

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header title="Справочники" subtitle="Управление типами и категориями данных клиники" />

            {/* Tabs */}
            <div style={{
                display: "flex", flexWrap: "wrap", gap: ".5rem",
                borderBottom: "1px solid var(--border-subtle)", paddingBottom: "1rem",
            }}>
                {ALL_TABS.map(s => (
                    <button
                        key={s.key}
                        onClick={() => setActiveKey(s.key)}
                        style={{
                            display: "flex", alignItems: "center", gap: ".5rem",
                            padding: ".5rem 1rem",
                            borderRadius: "var(--radius-md)",
                            border: activeKey === s.key ? "2px solid var(--color-primary)" : "2px solid var(--border-subtle)",
                            background: activeKey === s.key ? "var(--color-primary-light)" : "var(--bg-base)",
                            color: activeKey === s.key ? "var(--color-primary)" : "var(--text-secondary)",
                            fontWeight: activeKey === s.key ? 700 : 500,
                            fontSize: ".875rem", cursor: "pointer",
                            transition: "all var(--transition-fast)",
                        }}
                    >
                        <span>{s.icon}</span>
                        <span>{s.label}</span>
                    </button>
                ))}
            </div>

            {/* key= forces remount on tab switch, resetting all state cleanly */}
            {activeKey === PROCEDURE_TYPES_KEY
                ? <ProcedureTypePanel key={PROCEDURE_TYPES_KEY} />
                : <SectionPanel key={activeKey} section={SECTIONS.find(s => s.key === activeKey)!} />
            }
        </div>
    );
};

export default ReferencePage;
