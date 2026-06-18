import React, { useState, useEffect, useMemo } from 'react';
import * as XLSX from 'xlsx';
import {
    BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, PieChart, Pie, Cell,
    ResponsiveContainer, Legend, AreaChart, Area,
} from 'recharts';
import api from '../api/api';
import { format, subMonths } from 'date-fns';
import { ru } from 'date-fns/locale';
import { Header } from '../Components/Layout/Header';
import { Card } from '../Components/UI/Card';
import { Button } from '../Components/UI/Button';
import { Badge } from '../Components/UI/Badge';
import { Spinner } from '../Components/UI/Spinner';
import { useToast } from '../Hooks/useToast';

// Rose Atelier chart colors
const COLORS = ['#7B3F62', '#C8865A', '#3A7D5E', '#3A6B9E', '#A0607E', '#B88A3E'];

const AnalyticsDashboardPage: React.FC = () => {
    const { showError } = useToast();
    const [patientData, setPatientData]         = useState<any[]>([]);
    const [procedureData, setProcedureData]     = useState<any[]>([]);
    const [doctorData, setDoctorData]           = useState<any[]>([]);
    const [procedureTypeData, setProcedureTypeData] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [dateRange, setDateRange] = useState({
        from: subMonths(new Date(), 6),
        to:   new Date(),
    });
    const [selectedDoctor, setSelectedDoctor] = useState('all');

    const fetchData = async () => {
        setLoading(true);
        try {
            const [p, proc, d] = await Promise.all([
                api.get('/Patients/All'),
                api.get('/Procedures/All'),
                api.get('/Doctors/All'),
            ]);
            setPatientData(Array.isArray(p.data)      ? p.data      : []);
            setProcedureData(Array.isArray(proc.data) ? proc.data   : []);
            setDoctorData(Array.isArray(d.data)       ? d.data      : []);
        } catch {
            showError('Ошибка загрузки данных аналитики');
        } finally {
            setLoading(false);
        }
        // Load procedure types separately — needed for duration in export
        try {
            const pt = await api.get('/ProcedureTypes/All');
            setProcedureTypeData(Array.isArray(pt.data) ? pt.data : []);
        } catch {
            // Non-critical: export will show '—' for duration if this fails
        }
    };

    useEffect(() => { fetchData(); }, []);

    // ── Filtered data ──────────────────────────────────────────────────────────
    const filteredProcedures = useMemo(() => procedureData.filter(proc => {
        const d = proc.scheduledDate ? new Date(proc.scheduledDate)
                : proc.creationDate  ? new Date(proc.creationDate) : null;
        if (!d || isNaN(d.getTime())) return false;
        const inRange   = d >= dateRange.from && d <= dateRange.to;
        const byDoctor  = selectedDoctor === 'all' || proc.doctorId === selectedDoctor || proc.doctor?.id === selectedDoctor;
        return inRange && byDoctor;
    }), [procedureData, dateRange, selectedDoctor]);

    const filteredPatients = useMemo(() => patientData.filter(p => {
        const d = p.creationDate ? new Date(p.creationDate) : new Date();
        return d >= dateRange.from && d <= dateRange.to;
    }), [patientData, dateRange]);

    // ── Charts data ────────────────────────────────────────────────────────────
    const ageGroups = useMemo(() => {
        const groups: Record<string, number> = { '0–18': 0, '19–30': 0, '31–45': 0, '46–60': 0, '60+': 0 };
        filteredPatients.forEach(p => {
            const a = p.age || 0;
            if      (a <= 18) groups['0–18']++;
            else if (a <= 30) groups['19–30']++;
            else if (a <= 45) groups['31–45']++;
            else if (a <= 60) groups['46–60']++;
            else              groups['60+']++;
        });
        return Object.entries(groups).map(([name, value]) => ({ name, value }));
    }, [filteredPatients]);

    const procPopularity = useMemo(() => {
        const map: Record<string, number> = {};
        filteredProcedures.forEach(p => { const t = p.title || 'Неизвестно'; map[t] = (map[t] || 0) + 1; });
        return Object.entries(map).map(([name, value]) => ({ name, value })).sort((a,b)=>b.value-a.value).slice(0,6);
    }, [filteredProcedures]);

    const monthlyTrends = useMemo(() => {
        const counts: Record<string, number> = {};
        const revenue: Record<string, number> = {};
        filteredProcedures.forEach(p => {
            const d = p.scheduledDate ? new Date(p.scheduledDate) : p.creationDate ? new Date(p.creationDate) : null;
            if (!d || isNaN(d.getTime())) return;
            const key = `${d.getFullYear()}-${d.getMonth()}`;
            counts[key]  = (counts[key]  || 0) + 1;
            revenue[key] = (revenue[key] || 0) + (p.price || 0);
        });
        const result = [];
        const cur = new Date(dateRange.from);
        while (cur <= dateRange.to) {
            const key = `${cur.getFullYear()}-${cur.getMonth()}`;
            result.push({ date: format(cur, 'MMM yyyy', { locale: ru }), count: counts[key] || 0, revenue: revenue[key] || 0 });
            cur.setMonth(cur.getMonth() + 1);
        }
        return result;
    }, [filteredProcedures, dateRange]);

    const doctorWorkload = useMemo(() => {
        const stats: Record<string, any> = {};
        doctorData.forEach(d => { stats[d.id] = { id: d.id, name: d.fullname || d.name || 'Врач', procedures: 0, revenue: 0 }; });
        filteredProcedures.forEach(p => {
            const did = p.doctorId || p.doctor?.id;
            if (did && stats[did]) { stats[did].procedures++; stats[did].revenue += (p.price || 0); }
        });
        return Object.values(stats).sort((a: any, b: any) => b.procedures - a.procedures).slice(0, 5);
    }, [filteredProcedures, doctorData]);

    const totalRevenue     = filteredProcedures.reduce((s, p) => s + (p.price || 0), 0);
    const avgProcCost      = filteredProcedures.length ? Math.round(totalRevenue / filteredProcedures.length) : 0;
    const avgRevPerPatient = filteredPatients.length   ? Math.round(totalRevenue / filteredPatients.length)   : 0;

    // ── Excel export ───────────────────────────────────────────────────────────
    const handleExport = () => {
        const wb = XLSX.utils.book_new();

        // Build lookup maps for patient names, doctor names, and procedure type durations
        const patientMap: Record<string, string> = {};
        patientData.forEach(p => {
            // /Patients/All returns PatientCard objects; card id may be in p.id or p.cardId
            const cardId = p.id ?? p.cardId;
            if (cardId) patientMap[cardId] = p.fullname || '—';
        });

        const doctorMap: Record<string, string> = {};
        doctorData.forEach(d => {
            if (d.id) doctorMap[d.id] = d.fullname || d.name || '—';
        });

        // typeMap: typeId → duration string (handles 0, undefined, different field names)
        const typeMap: Record<string, string> = {};
        procedureTypeData.forEach(t => {
            const key = t.id ?? t.Id;
            if (key == null) return;
            // Backend may return "duration" or "standartDuration" (camelCase of StandartDuration)
            const dur = t.duration ?? t.standartDuration ?? t.Duration ?? t.StandartDuration;
            typeMap[String(key)] = dur != null ? String(dur) : '—';
        });

        // Sheet 1: filtered procedures
        const procRows = filteredProcedures.map(p => ({
            'Название':       p.title || '—',
            'Пациент':        patientMap[p.patientCardId] || '—',
            'Врач':           p.doctorId ? (doctorMap[p.doctorId] || '—') : '—',
            'Дата':           p.scheduledDate ? new Date(p.scheduledDate).toLocaleString('ru-RU') : '—',
            'Цена (₽)':       p.price || 0,
            'Длит. (мин)':    p.typeId ? (typeMap[String(p.typeId)] ?? '—') : '—',
            'Завершена':      p.isComplete  ? 'Да' : 'Нет',
            'Отменена':       p.isCancelled ? 'Да' : 'Нет',
        }));
        const ws1 = XLSX.utils.json_to_sheet(procRows.length > 0 ? procRows : [{ 'Нет данных': '' }]);
        XLSX.utils.book_append_sheet(wb, ws1, 'Процедуры');

        // Sheet 2: doctor workload
        const docRows = (doctorWorkload as any[]).map(d => ({
            'Врач':           d.name,
            'Процедур':       d.procedures,
            'Выручка (₽)':    d.revenue,
        }));
        const ws2 = XLSX.utils.json_to_sheet(docRows.length > 0 ? docRows : [{ 'Нет данных': '' }]);
        XLSX.utils.book_append_sheet(wb, ws2, 'Загруженность врачей');

        // Sheet 3: monthly trends
        const trendRows = monthlyTrends.map(m => ({
            'Месяц':          m.date,
            'Процедур':       m.count,
            'Выручка (₽)':    m.revenue,
        }));
        const ws3 = XLSX.utils.json_to_sheet(trendRows.length > 0 ? trendRows : [{ 'Нет данных': '' }]);
        XLSX.utils.book_append_sheet(wb, ws3, 'Динамика по месяцам');

        // Sheet 4: age distribution
        const ageRows = ageGroups.map(a => ({ 'Возрастная группа': a.name, 'Кол-во пациентов': a.value }));
        const ws4 = XLSX.utils.json_to_sheet(ageRows);
        XLSX.utils.book_append_sheet(wb, ws4, 'Возраст пациентов');

        // Download
        const fromStr = format(dateRange.from, 'dd.MM.yyyy');
        const toStr   = format(dateRange.to,   'dd.MM.yyyy');
        XLSX.writeFile(wb, `Аналитика_${fromStr}–${toStr}.xlsx`);
    };

    if (loading) return (
        <div className="flex items-center justify-center" style={{ minHeight: '60vh' }}><Spinner size="lg" /></div>
    );

    const KpiCard = ({ label, value, sub, color }: { label: string; value: string; sub?: string; color: string }) => (
        <div className="card animate-fade-in" style={{ padding: '1.5rem' }}>
            <p style={{ fontSize: '.8125rem', color: 'var(--text-muted)', marginBottom: '.5rem' }}>{label}</p>
            <p style={{ fontFamily: "'Cormorant Garamond', serif", fontSize: '1.875rem', fontWeight: 700, color, lineHeight: 1 }}>{value}</p>
            {sub && <p style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginTop: '.5rem' }}>{sub}</p>}
        </div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <Header
                title="Аналитика"
                subtitle="Статистика и показатели клиники"
                actions={
                    <div style={{ display: 'flex', gap: '.75rem' }}>
                        <Button onClick={handleExport} variant="outline">📥 Экспорт Excel</Button>
                        <Button onClick={fetchData}    variant="outline">↻ Обновить</Button>
                    </div>
                }
            />

            {/* Filters */}
            <Card title="Фильтры">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div>
                        <label className="form-label">Период</label>
                        <div style={{ display: 'flex', gap: '.5rem', alignItems: 'center' }}>
                            <input type="date" className="form-input"
                                value={format(dateRange.from, 'yyyy-MM-dd')}
                                onChange={e => setDateRange(r => ({ ...r, from: new Date(e.target.value) }))}
                            />
                            <span style={{ color: 'var(--text-muted)', flexShrink: 0 }}>—</span>
                            <input type="date" className="form-input"
                                value={format(dateRange.to, 'yyyy-MM-dd')}
                                onChange={e => setDateRange(r => ({ ...r, to: new Date(e.target.value) }))}
                            />
                        </div>
                    </div>
                    <div>
                        <label className="form-label">Врач</label>
                        <select className="form-input" value={selectedDoctor} onChange={e => setSelectedDoctor(e.target.value)}>
                            <option value="all">Все врачи</option>
                            {doctorData.map(d => (
                                <option key={d.id} value={d.id}>{d.fullname || d.name}</option>
                            ))}
                        </select>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'flex-end' }}>
                        <Badge variant="primary" className="h-fit">
                            Процедур в выборке: {filteredProcedures.length}
                        </Badge>
                    </div>
                </div>
            </Card>

            {/* KPIs */}
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                <KpiCard label="Выручка"          value={`${totalRevenue.toLocaleString('ru-RU')} ₽`} color="var(--color-primary)"  />
                <KpiCard label="Процедур"          value={String(filteredProcedures.length)} sub={`Ср. чек: ${avgProcCost} ₽`} color="var(--color-accent)"  />
                <KpiCard label="Пациентов"         value={String(filteredPatients.length)}               color="var(--color-success)" />
                <KpiCard label="Доход / пациент"   value={`${avgRevPerPatient} ₽`} sub="в среднем"       color="var(--color-info)"    />
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <Card title="📈 Динамика выручки">
                    <ResponsiveContainer width="100%" height={280}>
                        <AreaChart data={monthlyTrends}>
                            <defs>
                                <linearGradient id="gRev" x1="0" y1="0" x2="0" y2="1">
                                    <stop offset="5%"  stopColor="#7B3F62" stopOpacity={.3} />
                                    <stop offset="95%" stopColor="#7B3F62" stopOpacity={0}  />
                                </linearGradient>
                                <linearGradient id="gCnt" x1="0" y1="0" x2="0" y2="1">
                                    <stop offset="5%"  stopColor="#3A7D5E" stopOpacity={.3} />
                                    <stop offset="95%" stopColor="#3A7D5E" stopOpacity={0}  />
                                </linearGradient>
                            </defs>
                            <CartesianGrid strokeDasharray="3 3" stroke="var(--border-subtle)" />
                            <XAxis dataKey="date" tick={{ fontSize: 11, fill: 'var(--text-muted)' }} />
                            <YAxis tick={{ fontSize: 11, fill: 'var(--text-muted)' }} />
                            <Tooltip contentStyle={{ borderRadius: 10, border: '1px solid var(--border-default)', fontFamily: 'Nunito', fontSize: 13 }} />
                            <Legend />
                            <Area type="monotone" dataKey="revenue" name="Выручка (₽)" stroke="#7B3F62" fill="url(#gRev)" strokeWidth={2} />
                            <Area type="monotone" dataKey="count"   name="Процедур"    stroke="#3A7D5E" fill="url(#gCnt)" strokeWidth={2} />
                        </AreaChart>
                    </ResponsiveContainer>
                </Card>

                <Card title="👥 Возрастная структура">
                    <ResponsiveContainer width="100%" height={280}>
                        <PieChart>
                            <Pie data={ageGroups} cx="50%" cy="50%" outerRadius={100} dataKey="value"
                                labelLine={false} label={({ name, value }) => value > 0 ? `${name}: ${value}` : ''}>
                                {COLORS.map((c, i) => <Cell key={i} fill={c} />)}
                            </Pie>
                            <Tooltip contentStyle={{ borderRadius: 10, fontFamily: 'Nunito', fontSize: 13 }} />
                        </PieChart>
                    </ResponsiveContainer>
                </Card>

                <Card title="🏆 Популярные процедуры">
                    <ResponsiveContainer width="100%" height={280}>
                        <BarChart data={procPopularity} layout="vertical" margin={{ left: 20 }}>
                            <CartesianGrid strokeDasharray="3 3" stroke="var(--border-subtle)" horizontal={false} />
                            <XAxis type="number" tick={{ fontSize: 11, fill: 'var(--text-muted)' }} />
                            <YAxis type="category" dataKey="name" width={120} tick={{ fontSize: 11, fill: 'var(--text-muted)' }} />
                            <Tooltip contentStyle={{ borderRadius: 10, fontFamily: 'Nunito', fontSize: 13 }} />
                            <Bar dataKey="value" name="Кол-во" fill="#7B3F62" radius={[0,4,4,0]} />
                        </BarChart>
                    </ResponsiveContainer>
                </Card>

                {/* Dual-axis bar chart: left = procedure count, right = revenue */}
                <Card title="👨‍⚕️ Загруженность врачей">
                    <ResponsiveContainer width="100%" height={280}>
                        <BarChart data={doctorWorkload} margin={{ right: 20 }}>
                            <CartesianGrid strokeDasharray="3 3" stroke="var(--border-subtle)" />
                            <XAxis dataKey="name" tick={{ fontSize: 10, fill: 'var(--text-muted)' }} />
                            <YAxis
                                yAxisId="left"
                                orientation="left"
                                tick={{ fontSize: 11, fill: 'var(--text-muted)' }}
                                label={{ value: 'Процедур', angle: -90, position: 'insideLeft', offset: 10, style: { fontSize: 10, fill: 'var(--text-muted)' } }}
                            />
                            <YAxis
                                yAxisId="right"
                                orientation="right"
                                tick={{ fontSize: 11, fill: 'var(--text-muted)' }}
                                tickFormatter={(v: number) => v >= 1000 ? `${Math.round(v / 1000)}k` : String(v)}
                                label={{ value: 'Выручка', angle: 90, position: 'insideRight', offset: 10, style: { fontSize: 10, fill: 'var(--text-muted)' } }}
                            />
                            <Tooltip
                                contentStyle={{ borderRadius: 10, fontFamily: 'Nunito', fontSize: 13 }}
                                formatter={(value: number, name: string) =>
                                    name === 'Выручка' ? [`${value.toLocaleString('ru-RU')} ₽`, name] : [value, name]
                                }
                            />
                            <Legend />
                            <Bar yAxisId="left"  dataKey="procedures" name="Процедуры" fill="#7B3F62" radius={[4,4,0,0]} />
                            <Bar yAxisId="right" dataKey="revenue"    name="Выручка"   fill="#C8865A" radius={[4,4,0,0]} />
                        </BarChart>
                    </ResponsiveContainer>
                </Card>
            </div>
        </div>
    );
};

export default AnalyticsDashboardPage;
