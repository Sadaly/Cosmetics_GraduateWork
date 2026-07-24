import React, { useEffect, useState } from "react";
import api from "../api/api";
import { Card } from "../Components/UI/Card";
import { Button } from "../Components/UI/Button";
import { Input } from "../Components/UI/Input";
import { Header } from "../Components/Layout/Header";
import { Spinner } from "../Components/UI/Spinner";

interface UserItem {
    userId: string;
    username: string;
    email: string;
    registrationDate: string;
    updateDate: string;
}

const UsersPage: React.FC = () => {
    const [users, setUsers]               = useState<UserItem[]>([]);
    const [loading, setLoading]           = useState(true);
    const [error, setError]               = useState<string | null>(null);
    const [username, setUsername]         = useState("");
    const [email, setEmail]               = useState("");
    const [password, setPassword]         = useState("");
    const [creating, setCreating]         = useState(false);
    const [createError, setCreateError]   = useState<string | null>(null);
    const [createSuccess, setCreateSucc]  = useState(false);
    const [deletingId, setDeletingId]     = useState<string | null>(null);
    const [startIndex, setStartIndex]     = useState(0);
    const pageSize                         = 10;
    const [hasMore, setHasMore]           = useState(true);

    const fetchUsers = async (start: number) => {
        setLoading(true); setError(null);
        try {
            const r = await api.get("/Users/Take", { params: { StartIndex: start, Count: pageSize } });
            const data: UserItem[] = Array.isArray(r.data) ? r.data : [];
            setUsers(data);
            setHasMore(data.length === pageSize);
        } catch { setError("Не удалось загрузить пользователей"); }
        finally   { setLoading(false); }
    };

    useEffect(() => { fetchUsers(startIndex); }, [startIndex]);

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        setCreating(true); setCreateError(null); setCreateSucc(false);
        try {
            await api.post("/Users/Register", { username, email, password });
            setUsername(""); setEmail(""); setPassword("");
            setCreateSucc(true);
            setStartIndex(0); fetchUsers(0);
        } catch (e: any) {
            setCreateError(e.response?.data?.detail || "Ошибка при создании пользователя");
        } finally { setCreating(false); }
    };

    const handleDelete = async (userId: string) => {
        if (!window.confirm("Удалить пользователя?")) return;
        setDeletingId(userId);
        try {
            await api.delete(`/Users/${userId}`);
            setUsers(prev => prev.filter(u => u.userId !== userId));
        } catch (e: any) {
            alert(e.response?.data?.detail || "Не удалось удалить пользователя");
        } finally { setDeletingId(null); }
    };

    const formatDate = (iso: string) => {
        const d = new Date(iso);
        return isNaN(d.getTime()) ? "—" : d.toLocaleString("ru-RU", { day: "2-digit", month: "2-digit", year: "numeric" });
    };

    const currentPage = Math.floor(startIndex / pageSize) + 1;

    return (
        <div className="animate-fade-in" style={{ display: "flex", flexDirection: "column", gap: "1.75rem" }}>
            <Header title="Пользователи" subtitle="Создание учётных записей и управление доступом к системе" />

            {/* Create form */}
            <Card title="Новый пользователь">
                <form onSubmit={handleCreate}>
                    <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: "1rem", marginBottom: "1rem" }}>
                        <Input
                            label="Имя пользователя"
                            placeholder="Иванова А.В."
                            value={username}
                            onChange={e => setUsername(e.target.value)}
                            required
                        />
                        <Input
                            label="Email"
                            type="email"
                            placeholder="user@clinic.ru"
                            value={email}
                            onChange={e => setEmail(e.target.value)}
                            required
                        />
                        <Input
                            label="Пароль"
                            type="password"
                            placeholder="Минимум 6 символов"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    {createError && (
                        <div style={{
                            marginBottom: "1rem", padding: ".75rem 1rem",
                            background: "var(--color-danger-light)", borderRadius: "var(--radius-md)",
                            color: "var(--color-danger)", fontSize: ".875rem", fontWeight: 500,
                        }}>{createError}</div>
                    )}
                    {createSuccess && (
                        <div style={{
                            marginBottom: "1rem", padding: ".75rem 1rem",
                            background: "var(--color-success-light)", borderRadius: "var(--radius-md)",
                            color: "var(--color-success)", fontSize: ".875rem", fontWeight: 500,
                        }}>✓ Пользователь успешно создан</div>
                    )}

                    <Button type="submit" variant="primary" isLoading={creating}>
                        {creating ? "Создание..." : "Создать пользователя"}
                    </Button>
                </form>
            </Card>

            {/* Users table */}
            <Card title="Список пользователей" subtitle={`Страница ${currentPage}`} noPadding>
                {loading ? (
                    <div style={{ padding: "3rem" }}><Spinner /></div>
                ) : error ? (
                    <p style={{ padding: "2rem", color: "var(--color-danger)" }}>{error}</p>
                ) : users.length === 0 ? (
                    <p style={{ padding: "2rem", textAlign: "center", color: "var(--text-muted)" }}>Пользователи не найдены</p>
                ) : (
                    <>
                        <table>
                            <thead>
                                <tr>
                                    {["Пользователь", "Email", "Дата регистрации", ""].map(h => <th key={h}>{h}</th>)}
                                </tr>
                            </thead>
                            <tbody>
                                {users.map(u => (
                                    <tr key={u.userId}>
                                        <td>
                                            <div style={{ display: "flex", alignItems: "center", gap: "0.75rem" }}>
                                                <div style={{
                                                    width: 34, height: 34, borderRadius: "50%",
                                                    background: "var(--color-primary-light)",
                                                    display: "flex", alignItems: "center", justifyContent: "center",
                                                    color: "var(--color-primary)", fontWeight: 700, fontSize: ".875rem", flexShrink: 0,
                                                }}>
                                                    {u.username[0]?.toUpperCase() ?? "U"}
                                                </div>
                                                <span style={{ fontWeight: 600, color: "var(--text-primary)" }}>{u.username}</span>
                                            </div>
                                        </td>
                                        <td>{u.email}</td>
                                        <td>{formatDate(u.registrationDate)}</td>
                                        <td style={{ textAlign: "right" }}>
                                            <Button
                                                size="sm"
                                                variant="danger"
                                                isLoading={deletingId === u.userId}
                                                onClick={() => handleDelete(u.userId)}
                                            >
                                                Удалить
                                            </Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                        <div style={{ display: "flex", justifyContent: "center", gap: "1rem", padding: "1.25rem", borderTop: "1px solid var(--border-subtle)" }}>
                            <Button size="sm" variant="outline" disabled={startIndex === 0} onClick={() => setStartIndex(Math.max(0, startIndex - pageSize))}>
                                ← Назад
                            </Button>
                            <span style={{ fontSize: ".875rem", fontWeight: 600, color: "var(--text-secondary)", lineHeight: "2rem" }}>
                                Стр. {currentPage}
                            </span>
                            <Button size="sm" variant="outline" disabled={!hasMore} onClick={() => setStartIndex(startIndex + pageSize)}>
                                Вперёд →
                            </Button>
                        </div>
                    </>
                )}
            </Card>
        </div>
    );
};

export default UsersPage;
