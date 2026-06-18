import React, { useState, useEffect, useMemo, useRef } from 'react';
import {
    format, startOfWeek, endOfWeek, eachDayOfInterval,
    addWeeks, subWeeks, isSameDay, parseISO, addDays, subDays, isWithinInterval,
    startOfDay, endOfDay, startOfMonth, endOfMonth, addMonths, subMonths, getDay,
} from 'date-fns';
import { ru } from 'date-fns/locale';
import api from '../api/api';
import { Header } from '../Components/Layout/Header';
import { Card } from '../Components/UI/Card';
import { Button } from '../Components/UI/Button';
import { Modal } from '../Components/UI/Modal';
import { Input } from '../Components/UI/Input';
import { Spinner } from '../Components/UI/Spinner';
import { useToast } from '../Hooks/useToast';

// ─── Types ────────────────────────────────────────────────────────────────────
interface Procedure {
    procedureId: string; patientCardId: string; typeId: string;
    doctorId?: string | null; title: string; scheduledDate: string;
    price: number; duration?: number;
    isComplete?: boolean; isCancelled?: boolean;
}
interface PatientCard { id: string; patientId: string; fullname: string; age: number; phoneNumber: string; }
interface ProcedureType { id: string; title: string; duration: number; price: number; }
interface Doctor { id: string; name: string; }
type ViewMode = 'day' | 'week' | 'month' | 'list';

// ─── Searchable dropdown ──────────────────────────────────────────────────────
interface DropdownItem { id: string; label: string; sub?: string; }
interface SearchDropdownProps {
    label: string; required?: boolean; optional?: boolean; hint?: string;
    placeholder: string; value: string;
    onChange: (v: string) => void;
    items: DropdownItem[];
    onSelect: (id: string, label: string) => void;
    selectedLabel?: string;
}

const SearchDropdown: React.FC<SearchDropdownProps> = ({
    label, required, optional, hint, placeholder, value, onChange, items, onSelect, selectedLabel,
}) => {
    const [open, setOpen] = useState(false);
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const h = (e: MouseEvent) => { if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false); };
        document.addEventListener('mousedown', h);
        return () => document.removeEventListener('mousedown', h);
    }, []);

    const filtered = useMemo(() => {
        if (!value.trim()) return items;
        const q = value.toLowerCase();
        return items.filter(i => i.label.toLowerCase().includes(q));
    }, [items, value]);

    return (
        <div className="form-group" ref={ref}>
            <label className="form-label">
                {label}{' '}
                {required && <span style={{ color: 'var(--color-danger)' }}>*</span>}
                {optional && <span style={{ color: 'var(--text-muted)', fontWeight: 400 }}> (необязательно)</span>}
            </label>
            {hint && <p style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginBottom: '.375rem' }}>{hint}</p>}
            <div style={{ position: 'relative' }}>
                <input
                    type="text"
                    placeholder={placeholder}
                    value={value}
                    onChange={e => { onChange(e.target.value); setOpen(true); }}
                    onFocus={() => setOpen(true)}
                    autoComplete="off"
                    className="form-input"
                    style={selectedLabel && !open ? { paddingRight: '2.75rem' } : undefined}
                />
                {/* Green tick badge — shown inside the input on the right when a value is selected */}
                {selectedLabel && !open && (
                    <div style={{
                        position: 'absolute', right: '.625rem', top: '50%',
                        transform: 'translateY(-50%)',
                        width: 22, height: 22, borderRadius: '50%',
                        background: 'var(--color-success)',
                        display: 'flex', alignItems: 'center', justifyContent: 'center',
                        color: '#fff', fontSize: '.75rem', fontWeight: 700,
                        pointerEvents: 'none', flexShrink: 0,
                    }}>✓</div>
                )}
                {open && filtered.length > 0 && (
                    <div style={{
                        position: 'absolute', top: '100%', left: 0, right: 0, zIndex: 200,
                        background: 'var(--bg-surface)', border: '1px solid var(--border-default)',
                        borderRadius: 'var(--radius-md)', boxShadow: 'var(--shadow-lg)',
                        maxHeight: 220, overflowY: 'auto', marginTop: 4,
                    }}>
                        {filtered.map(item => (
                            <button
                                key={item.id} type="button"
                                onClick={() => { onSelect(item.id, item.label); setOpen(false); }}
                                style={{
                                    display: 'block', width: '100%', textAlign: 'left',
                                    padding: '.625rem 1rem', background: 'none', border: 'none', cursor: 'pointer',
                                    transition: 'background var(--transition-fast)',
                                }}
                                onMouseEnter={e => (e.currentTarget.style.background = 'var(--bg-muted)')}
                                onMouseLeave={e => (e.currentTarget.style.background = 'none')}
                            >
                                <div style={{ fontSize: '.875rem', fontWeight: 500, color: 'var(--text-primary)' }}>{item.label}</div>
                                {item.sub && <div style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginTop: 1 }}>{item.sub}</div>}
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

// ─── Page ─────────────────────────────────────────────────────────────────────
const SchedulePage: React.FC = () => {
    const { showSuccess, showError, showWarning } = useToast();
    const [currentDate, setCurrentDate] = useState(new Date());
    const [viewMode, setViewMode] = useState<ViewMode>('week');
    const [procedures, setProcedures] = useState<Procedure[]>([]);
    const [selectedProcedure, setSelectedProcedure] = useState<Procedure | null>(null);
    const [isDetailsOpen, setIsDetailsOpen] = useState(false);
    const [isAddOpen, setIsAddOpen] = useState(false);
    const [patients, setPatients] = useState<PatientCard[]>([]);
    const [procedureTypes, setProcedureTypes] = useState<ProcedureType[]>([]);
    const [doctors, setDoctors] = useState<Doctor[]>([]);
    const [loadingRef, setLoadingRef] = useState(false);
    const [formLoading, setFormLoading] = useState(false);
    const [deleteTarget, setDeleteTarget] = useState<string | null>(null);
    const [completedIds, setCompletedIds] = useState<Set<string>>(() => {
        try { return new Set(JSON.parse(localStorage.getItem('completedProcedures') ?? '[]') as string[]); }
        catch { return new Set<string>(); }
    });
    const [proceeding, setProceeding] = useState(false);
    const [timeConflictWarning, setTimeConflictWarning] = useState<string | null>(null);

    // Reschedule & cancel state
    const [isRescheduleMode, setIsRescheduleMode] = useState(false);
    const [rescheduleDate, setRescheduleDate] = useState('');
    const [rescheduleLoading, setRescheduleLoading] = useState(false);
    const [cancelTarget, setCancelTarget] = useState<string | null>(null);
    const [cancelLoading, setCancelLoading] = useState(false);

    const [dateRange, setDateRange] = useState({
        start: startOfWeek(new Date(), { weekStartsOn: 1 }),
        end:   endOfWeek(new Date(),   { weekStartsOn: 1 }),
    });

    // Form state
    const [form, setForm] = useState({ patientCardId: '', typeId: '', doctorId: '', scheduledDate: '', price: '' });
    const [search, setSearch] = useState({ patient: '', type: '', doctor: '' });

    const weekDays = useMemo(() => eachDayOfInterval({ start: dateRange.start, end: dateRange.end }), [dateRange]);
    const dayHours = Array.from({ length: 13 }, (_, i) => i + 8);

    const monthDays = useMemo(() => {
        if (viewMode !== 'month') return [];
        return eachDayOfInterval({ start: startOfMonth(currentDate), end: endOfMonth(currentDate) });
    }, [currentDate, viewMode]);

    const monthFirstDayOffset = useMemo(() => {
        if (viewMode !== 'month') return 0;
        return (getDay(startOfMonth(currentDate)) + 6) % 7;
    }, [currentDate, viewMode]);

    const sortedProcedures = useMemo(() =>
        [...procedures].sort((a, b) => {
            if (!a.scheduledDate) return 1;
            if (!b.scheduledDate) return -1;
            return new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime();
        }),
        [procedures]
    );

    const loadReferenceData = async () => {
        setLoadingRef(true);
        try {
            const [p, t, d] = await Promise.all([
                api.get('/PatientCards/All'),
                api.get('/ProcedureTypes/All'),
                api.get('/Doctors/All'),
            ]);
            setPatients(Array.isArray(p.data) ? p.data : []);
            setProcedureTypes(Array.isArray(t.data) ? t.data : []);
            setDoctors(Array.isArray(d.data) ? d.data : []);
        } catch { showError('Ошибка загрузки справочных данных'); }
        finally { setLoadingRef(false); }
    };

    const fetchProcedures = async () => {
        try {
            const r = await api.get('/Procedures/All');
            const data = (Array.isArray(r.data) ? r.data : []).filter((p: Procedure) => {
                if (!p.scheduledDate) return false;
                return isWithinInterval(parseISO(p.scheduledDate), { start: dateRange.start, end: dateRange.end });
            });
            setProcedures(data);
        } catch { showError('Ошибка загрузки процедур'); }
    };

    useEffect(() => { loadReferenceData(); }, []);
    useEffect(() => { fetchProcedures(); }, [dateRange]);

    // Conflict warning when the user picks a date/time in the Add form
    useEffect(() => {
        if (!form.scheduledDate || !isAddOpen) { setTimeConflictWarning(null); return; }
        const selected = new Date(form.scheduledDate);
        const selType = procedureTypes.find(t => t.id === form.typeId);
        const selDuration = selType?.duration || 60;
        const selEnd = new Date(selected.getTime() + selDuration * 60000);
        const conflict = procedures.find(p => {
            if (!p.scheduledDate) return false;
            if (p.isCancelled || completedIds.has(p.procedureId)) return false;
            const pStart = parseISO(p.scheduledDate);
            const pEnd = new Date(pStart.getTime() + (p.duration || 60) * 60000);
            return selected < pEnd && selEnd > pStart;
        });
        if (conflict) {
            setTimeConflictWarning(
                `«${conflict.title}» (${format(parseISO(conflict.scheduledDate), 'dd.MM HH:mm', { locale: ru })})`
            );
        } else {
            setTimeConflictWarning(null);
        }
    }, [form.scheduledDate, form.typeId, procedures, procedureTypes, completedIds, isAddOpen]);

    const hasConflict = (proc: Procedure) => {
        if (proc.isCancelled || completedIds.has(proc.procedureId)) return false;
        const d1 = parseISO(proc.scheduledDate);
        const dur1 = proc.duration || 60;
        const end1 = new Date(d1.getTime() + dur1 * 60000);
        return procedures.some(p => {
            if (p.procedureId === proc.procedureId || !p.scheduledDate) return false;
            if (p.isCancelled || completedIds.has(p.procedureId)) return false;
            const d2 = parseISO(p.scheduledDate);
            const end2 = new Date(d2.getTime() + (p.duration || 60) * 60000);
            return d1 < end2 && end1 > d2;
        });
    };

    const navigateCalendar = (dir: 1 | -1) => {
        if (viewMode === 'week') {
            const fn = dir > 0 ? addWeeks : subWeeks;
            setCurrentDate(fn(currentDate, 1));
            setDateRange({ start: fn(dateRange.start, 1), end: fn(dateRange.end, 1) });
        } else if (viewMode === 'day') {
            const fn = dir > 0 ? addDays : subDays;
            const d = fn(currentDate, 1);
            setCurrentDate(d);
            setDateRange({ start: startOfDay(d), end: endOfDay(d) });
        } else {
            const fn = dir > 0 ? addMonths : subMonths;
            const d = fn(currentDate, 1);
            setCurrentDate(d);
            setDateRange({ start: startOfMonth(d), end: endOfMonth(d) });
        }
    };

    const goToday = () => {
        const t = new Date();
        setCurrentDate(t);
        if (viewMode === 'week') {
            setDateRange({ start: startOfWeek(t, { weekStartsOn: 1 }), end: endOfWeek(t, { weekStartsOn: 1 }) });
        } else if (viewMode === 'day') {
            setDateRange({ start: startOfDay(t), end: endOfDay(t) });
        } else {
            setDateRange({ start: startOfMonth(t), end: endOfMonth(t) });
        }
    };

    const switchView = (mode: ViewMode) => {
        const t = new Date();
        setViewMode(mode);
        setCurrentDate(t);
        if (mode === 'week') {
            setDateRange({ start: startOfWeek(t, { weekStartsOn: 1 }), end: endOfWeek(t, { weekStartsOn: 1 }) });
        } else if (mode === 'day') {
            setDateRange({ start: startOfDay(t), end: endOfDay(t) });
        } else {
            setDateRange({ start: startOfMonth(t), end: endOfMonth(t) });
        }
    };

    const handleAdd = async () => {
        if (!form.patientCardId) { showError('Выберите пациента'); return; }
        if (!form.typeId)        { showError('Выберите тип процедуры'); return; }
        if (!form.scheduledDate) { showError('Укажите дату и время'); return; }
        if (timeConflictWarning) {
            showWarning(`Пересечение с процедурой ${timeConflictWarning}. Запись будет создана.`);
        }
        setFormLoading(true);
        try {
            await api.post('/Procedures', {
                patientCardId: form.patientCardId,
                typeId: form.typeId,
                doctorId: form.doctorId || null,
                scheduledDate: new Date(form.scheduledDate).toISOString(),
                price: parseInt(form.price) || 0,
                duration: 0,
            });
            setIsAddOpen(false);
            setForm({ patientCardId: '', typeId: '', doctorId: '', scheduledDate: '', price: '' });
            setSearch({ patient: '', type: '', doctor: '' });
            setTimeConflictWarning(null);
            showSuccess('Запись успешно создана');
            fetchProcedures();
        } catch (e: any) {
            showError(e.response?.data?.message || 'Ошибка создания записи');
        } finally { setFormLoading(false); }
    };

    const handleDelete = async () => {
        if (!deleteTarget) return;
        try {
            await api.delete(`/Procedures/${deleteTarget}`);
            showSuccess('Запись удалена');
            fetchProcedures();
            setIsDetailsOpen(false);
        } catch { showError('Ошибка удаления'); }
        finally { setDeleteTarget(null); }
    };

    const handleProceed = async () => {
        if (!selectedProcedure) return;
        setProceeding(true);
        try {
            await api.put('/Procedures/Proceed', { procedureId: selectedProcedure.procedureId });
            setCompletedIds(prev => {
                const next = new Set([...prev, selectedProcedure.procedureId]);
                localStorage.setItem('completedProcedures', JSON.stringify([...next]));
                return next;
            });
            showSuccess('Процедура отмечена как завершённая');
        } catch { showError('Ошибка при обновлении статуса'); }
        finally { setProceeding(false); }
    };

    const handleReschedule = async () => {
        if (!selectedProcedure || !rescheduleDate) return;
        setRescheduleLoading(true);
        try {
            const type = procedureTypes.find(t => t.id === selectedProcedure.typeId);
            await api.put('/Procedures/UpdateDate', {
                procedureId: selectedProcedure.procedureId,
                scheduledDate: new Date(rescheduleDate).toISOString(),
                duration: type?.duration ?? 0,
            });
            showSuccess('Запись перенесена');
            fetchProcedures();
            setIsRescheduleMode(false);
            setIsDetailsOpen(false);
        } catch (e: any) {
            showError(e.response?.data?.detail || 'Ошибка при переносе записи');
        } finally { setRescheduleLoading(false); }
    };

    const handleCancelProcedure = async () => {
        if (!cancelTarget) return;
        setCancelLoading(true);
        try {
            await api.put('/Procedures/Cancel', { procedureId: cancelTarget });
            showSuccess('Запись отменена');
            fetchProcedures();
            setCancelTarget(null);
            setIsDetailsOpen(false);
        } catch { showError('Ошибка при отмене записи'); }
        finally { setCancelLoading(false); }
    };

    // DropdownItems helpers
    const patientItems  = useMemo(() => patients.map(p => ({ id: p.id, label: p.fullname, sub: `${p.phoneNumber ?? ''} • ${p.age ?? '?'} лет` })), [patients]);
    const typeItems     = useMemo(() => procedureTypes.map(t => ({ id: t.id, label: t.title, sub: `⏱ ${t.duration} мин  💰 ${t.price} ₽` })), [procedureTypes]);
    const doctorItems   = useMemo(() => doctors.map(d => ({ id: d.id, label: d.name })), [doctors]);

    const selPatient = patients.find(p => p.id === form.patientCardId);
    const selType    = procedureTypes.find(t => t.id === form.typeId);
    const selDoctor  = doctors.find(d => d.id === form.doctorId);

    const isDone = (id: string) => completedIds.has(id);

    const ProcedureCard = ({ proc, compact = false }: { proc: Procedure; compact?: boolean }) => {
        const conflict = hasConflict(proc);
        const cancelled = proc.isCancelled === true;
        const done = isDone(proc.procedureId);
        return (
            <div
                onClick={() => { setSelectedProcedure(proc); setIsDetailsOpen(true); setIsRescheduleMode(false); }}
                style={{
                    padding: compact ? '.5rem .625rem' : '.625rem .875rem',
                    borderRadius: 'var(--radius-sm)',
                    cursor: 'pointer',
                    background: cancelled
                        ? 'var(--bg-muted)'
                        : done
                            ? 'linear-gradient(135deg, var(--color-success), #2D6048)'
                            : conflict
                                ? 'linear-gradient(135deg, #B84054, #C8865A)'
                                : 'linear-gradient(135deg, var(--color-primary), #A0607E)',
                    color: cancelled ? 'var(--text-muted)' : '#fff',
                    opacity: (done || cancelled) ? 0.8 : 1,
                    border: cancelled ? '1px dashed var(--border-default)' : 'none',
                    transition: 'all var(--transition-fast)',
                }}
                onMouseEnter={e => (e.currentTarget.style.transform = 'scale(1.02)')}
                onMouseLeave={e => (e.currentTarget.style.transform = '')}
            >
                <div style={{ fontWeight: 700, fontSize: compact ? '.75rem' : '.8125rem', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
                    textDecoration: cancelled ? 'line-through' : 'none' }}>
                    {done ? '✓ ' : cancelled ? '✗ ' : ''}{proc.title || 'Процедура'}
                </div>
                <div style={{ fontSize: '.6875rem', opacity: .8, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {patients.find(p => p.id === proc.patientCardId)?.fullname ?? 'Пациент'}
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 2, fontSize: '.6875rem', opacity: .7 }}>
                    <span>{proc.scheduledDate ? format(parseISO(proc.scheduledDate), 'HH:mm') : ''}</span>
                    <span>{cancelled ? 'Отменена' : proc.price ? `${proc.price} ₽` : ''}</span>
                </div>
                {conflict && !cancelled && <div style={{ fontSize: '.625rem', fontWeight: 700, color: '#FFD580', marginTop: 2 }}>⚠ Пересечение</div>}
            </div>
        );
    };

    const todayCount  = procedures.filter(p => p.scheduledDate && isSameDay(parseISO(p.scheduledDate), new Date())).length;
    const totalRev    = procedures.reduce((s, p) => s + (p.price || 0), 0);
    const conflictCnt = procedures.filter(p => hasConflict(p)).length;

    const VIEW_LABELS: Record<ViewMode, string> = { day: 'День', week: 'Неделя', month: 'Месяц', list: 'Список' };
    const WEEK_HEADERS = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];

    const headerTitle = viewMode === 'week'
        ? `${format(dateRange.start, 'd MMMM', { locale: ru })} — ${format(dateRange.end, 'd MMMM yyyy', { locale: ru })}`
        : viewMode === 'day'
            ? format(currentDate, 'd MMMM yyyy', { locale: ru })
            : format(currentDate, 'LLLL yyyy', { locale: ru });

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <Header
                title="Расписание"
                subtitle="Управление записями на процедуры"
                actions={
                    <Button onClick={() => { loadReferenceData(); setIsAddOpen(true); }} variant="success" isLoading={loadingRef}>
                        + Записать пациента
                    </Button>
                }
            />

            {/* Controls */}
            <Card>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: '1rem' }}>
                    <div style={{ display: 'flex', gap: '.5rem', alignItems: 'center' }}>
                        <Button variant="outline" size="sm" onClick={() => navigateCalendar(-1)}>← Назад</Button>
                        <Button variant="outline" size="sm" onClick={goToday}>Сегодня</Button>
                        <Button variant="outline" size="sm" onClick={() => navigateCalendar(1)}>Вперёд →</Button>
                    </div>

                    <h2 style={{
                        fontFamily: "'Cormorant Garamond', serif",
                        fontSize: '1.125rem', fontWeight: 600, color: 'var(--text-primary)',
                    }}>
                        {headerTitle}
                    </h2>

                    <div style={{
                        display: 'flex', background: 'var(--bg-muted)',
                        borderRadius: 'var(--radius-md)', padding: 4,
                    }}>
                        {(['day', 'week', 'month', 'list'] as ViewMode[]).map(m => (
                            <button key={m} onClick={() => switchView(m)}
                                style={{
                                    padding: '.4rem .875rem', borderRadius: 6, border: 'none', cursor: 'pointer',
                                    fontFamily: "'Nunito', sans-serif", fontWeight: 600, fontSize: '.8125rem',
                                    background: viewMode === m ? 'var(--bg-surface)' : 'transparent',
                                    color: viewMode === m ? 'var(--color-primary)' : 'var(--text-muted)',
                                    boxShadow: viewMode === m ? 'var(--shadow-sm)' : 'none',
                                    transition: 'all var(--transition-fast)',
                                }}>
                                {VIEW_LABELS[m]}
                            </button>
                        ))}
                    </div>
                </div>
            </Card>

            {/* Calendar — week view */}
            {viewMode === 'week' && (
                <div className="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-7 gap-3">
                    {weekDays.map((day, i) => {
                        const dayProcs = procedures.filter(p => p.scheduledDate && isSameDay(parseISO(p.scheduledDate), day));
                        const isToday = isSameDay(day, new Date());
                        return (
                            <div key={i} className="card" style={{
                                minHeight: 140,
                                border: isToday ? '2px solid var(--color-primary)' : undefined,
                                overflow: 'hidden',
                            }}>
                                <div style={{
                                    textAlign: 'center', padding: '.625rem',
                                    background: isToday ? 'var(--color-primary-light)' : 'var(--bg-muted)',
                                    borderBottom: '1px solid var(--border-subtle)',
                                }}>
                                    <div style={{ fontSize: '.6875rem', fontWeight: 700, color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '.06em' }}>
                                        {format(day, 'EEE', { locale: ru })}
                                    </div>
                                    <div style={{
                                        fontFamily: "'Cormorant Garamond', serif",
                                        fontSize: '1.5rem', fontWeight: 700,
                                        color: isToday ? 'var(--color-primary)' : 'var(--text-primary)',
                                    }}>
                                        {format(day, 'd')}
                                    </div>
                                </div>
                                <div style={{ padding: '.5rem', display: 'flex', flexDirection: 'column', gap: '.375rem' }}>
                                    {dayProcs.length === 0
                                        ? <div style={{ textAlign: 'center', color: 'var(--text-muted)', fontSize: '.75rem', padding: '1rem 0' }}>Нет записей</div>
                                        : dayProcs.map(p => <ProcedureCard key={p.procedureId} proc={p} compact />)
                                    }
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}

            {/* Calendar — day view */}
            {viewMode === 'day' && (
                <Card noPadding>
                    <div>
                        {dayHours.map(hour => {
                            const slotProcs = procedures.filter(p => {
                                if (!p.scheduledDate) return false;
                                const d = parseISO(p.scheduledDate);
                                return isSameDay(d, currentDate) && d.getHours() === hour;
                            });
                            return (
                                <div key={hour} style={{
                                    display: 'flex', minHeight: 72,
                                    background: slotProcs.some(p => hasConflict(p)) ? 'rgba(184,64,84,.04)' : undefined,
                                    borderBottom: '1px solid var(--border-subtle)',
                                }}>
                                    <div style={{
                                        width: 72, flexShrink: 0, padding: '.875rem .5rem',
                                        textAlign: 'center', borderRight: '1px solid var(--border-subtle)',
                                        background: 'var(--bg-muted)',
                                    }}>
                                        <div style={{ fontSize: '.8125rem', fontWeight: 700, color: 'var(--text-secondary)' }}>
                                            {String(hour).padStart(2, '0')}:00
                                        </div>
                                    </div>
                                    <div style={{ flex: 1, padding: '.5rem .75rem', display: 'flex', flexDirection: 'column', gap: '.375rem' }}>
                                        {slotProcs.length === 0
                                            ? <div style={{ display: 'flex', alignItems: 'center', height: '100%', color: 'var(--text-muted)', fontSize: '.8125rem' }}>Свободно</div>
                                            : slotProcs.map(p => <ProcedureCard key={p.procedureId} proc={p} />)
                                        }
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </Card>
            )}

            {/* Calendar — month view */}
            {viewMode === 'month' && (
                <Card noPadding>
                    {/* Day-of-week headers */}
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(7, 1fr)', borderBottom: '1px solid var(--border-subtle)' }}>
                        {WEEK_HEADERS.map(d => (
                            <div key={d} style={{
                                textAlign: 'center', padding: '.5rem',
                                fontSize: '.6875rem', fontWeight: 700, color: 'var(--text-muted)',
                                textTransform: 'uppercase', letterSpacing: '.06em',
                                borderRight: '1px solid var(--border-subtle)',
                            }}>{d}</div>
                        ))}
                    </div>
                    {/* Day cells */}
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(7, 1fr)' }}>
                        {/* Leading empty cells */}
                        {Array.from({ length: monthFirstDayOffset }).map((_, i) => (
                            <div key={`e${i}`} style={{
                                minHeight: 90,
                                borderRight: '1px solid var(--border-subtle)',
                                borderBottom: '1px solid var(--border-subtle)',
                                background: 'var(--bg-muted)',
                            }} />
                        ))}
                        {monthDays.map((day, i) => {
                            const dayProcs = procedures.filter(p => p.scheduledDate && isSameDay(parseISO(p.scheduledDate), day));
                            const isToday = isSameDay(day, new Date());
                            return (
                                <div key={i}
                                    onClick={() => {
                                        setCurrentDate(day);
                                        setDateRange({ start: startOfDay(day), end: endOfDay(day) });
                                        setViewMode('day');
                                    }}
                                    style={{
                                        minHeight: 90, padding: '.375rem',
                                        borderRight: '1px solid var(--border-subtle)',
                                        borderBottom: '1px solid var(--border-subtle)',
                                        cursor: 'pointer',
                                        background: isToday ? 'var(--color-primary-light)' : undefined,
                                        transition: 'background var(--transition-fast)',
                                    }}
                                    onMouseEnter={e => { if (!isToday) e.currentTarget.style.background = 'var(--bg-muted)'; }}
                                    onMouseLeave={e => { if (!isToday) e.currentTarget.style.background = ''; }}
                                >
                                    <div style={{
                                        fontWeight: isToday ? 700 : 400,
                                        color: isToday ? 'var(--color-primary)' : 'var(--text-secondary)',
                                        fontSize: '.875rem', marginBottom: 2,
                                    }}>{format(day, 'd')}</div>
                                    {dayProcs.slice(0, 3).map(p => (
                                        <div key={p.procedureId} style={{
                                            fontSize: '.6875rem', fontWeight: 600,
                                            background: isDone(p.procedureId) ? 'var(--color-success)' : 'var(--color-primary)',
                                            color: '#fff', borderRadius: 3,
                                            padding: '1px 4px', marginBottom: 2,
                                            overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
                                        }}>
                                            {format(parseISO(p.scheduledDate), 'HH:mm')} {p.title || 'Процедура'}
                                        </div>
                                    ))}
                                    {dayProcs.length > 3 && (
                                        <div style={{ fontSize: '.625rem', color: 'var(--text-muted)', fontWeight: 600 }}>
                                            +{dayProcs.length - 3}
                                        </div>
                                    )}
                                </div>
                            );
                        })}
                    </div>
                </Card>
            )}

            {/* List view */}
            {viewMode === 'list' && (
                <Card noPadding>
                    {sortedProcedures.length === 0 ? (
                        <p style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '3rem 0' }}>
                            Нет процедур за этот период
                        </p>
                    ) : (
                        <table>
                            <thead>
                                <tr>
                                    {['Дата и время', 'Процедура', 'Пациент', 'Врач', 'Стоимость', 'Статус'].map(h => (
                                        <th key={h}>{h}</th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {sortedProcedures.map(p => {
                                    const pat = patients.find(pa => pa.id === p.patientCardId);
                                    const doc = doctors.find(d => d.id === p.doctorId);
                                    const done = isDone(p.procedureId);
                                    return (
                                        <tr key={p.procedureId}
                                            onClick={() => { setSelectedProcedure(p); setIsDetailsOpen(true); setIsRescheduleMode(false); }}
                                            style={{ cursor: 'pointer' }}
                                        >
                                            <td style={{ whiteSpace: 'nowrap' }}>
                                                {p.scheduledDate ? format(parseISO(p.scheduledDate), 'dd.MM.yyyy HH:mm', { locale: ru }) : '—'}
                                            </td>
                                            <td style={{ fontWeight: 600 }}>{p.title || '—'}</td>
                                            <td>{pat?.fullname ?? '—'}</td>
                                            <td>{doc?.name ?? '—'}</td>
                                            <td style={{ whiteSpace: 'nowrap' }}>{p.price ? `${p.price} ₽` : '—'}</td>
                                            <td>
                                                <span className={done ? 'badge badge-success' : p.isCancelled ? 'badge badge-danger' : 'badge badge-warning'}>
                                                    {done ? 'Завершена' : p.isCancelled ? 'Отменена' : 'Ожидается'}
                                                </span>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    )}
                </Card>
            )}

            {/* Stats */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                {[
                    { label: 'Сегодня', value: todayCount,                        color: 'var(--color-primary)' },
                    { label: 'За период', value: procedures.length,               color: 'var(--color-accent)' },
                    { label: 'Выручка', value: `${totalRev.toLocaleString('ru-RU')} ₽`, color: 'var(--color-success)' },
                    { label: 'Пересечений', value: conflictCnt,                   color: conflictCnt > 0 ? 'var(--color-danger)' : 'var(--text-muted)' },
                ].map(s => (
                    <div key={s.label} className="card" style={{ padding: '1.25rem', textAlign: 'center' }}>
                        <p style={{ fontFamily: "'Cormorant Garamond', serif", fontSize: '1.75rem', fontWeight: 700, color: s.color }}>{s.value}</p>
                        <p style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginTop: '.25rem' }}>{s.label}</p>
                    </div>
                ))}
            </div>

            {/* Details Modal */}
            <Modal
                isOpen={isDetailsOpen}
                onClose={() => { setIsDetailsOpen(false); setIsRescheduleMode(false); }}
                title={isRescheduleMode ? 'Перенести запись' : 'Детали записи'}
                size="sm"
                footer={selectedProcedure && (() => {
                    const done      = isDone(selectedProcedure.procedureId);
                    const cancelled = selectedProcedure.isCancelled === true;

                    if (isRescheduleMode) return (
                        <>
                            <Button variant="outline" onClick={() => setIsRescheduleMode(false)}>Назад</Button>
                            <Button
                                variant="primary"
                                isLoading={rescheduleLoading}
                                onClick={handleReschedule}
                                disabled={!rescheduleDate}
                            >
                                Сохранить перенос
                            </Button>
                        </>
                    );

                    return (
                        <>
                            <Button variant="outline" onClick={() => setIsDetailsOpen(false)}>Закрыть</Button>
                            {!done && !cancelled && (
                                <>
                                    <Button
                                        variant="outline"
                                        onClick={() => {
                                            if (!loadingRef && procedureTypes.length === 0) loadReferenceData();
                                            const iso = selectedProcedure.scheduledDate
                                                ? new Date(selectedProcedure.scheduledDate).toISOString().slice(0, 16)
                                                : '';
                                            setRescheduleDate(iso);
                                            setIsRescheduleMode(true);
                                        }}
                                        style={{ color: 'var(--color-info)' }}
                                    >
                                        ↗ Перенести
                                    </Button>
                                    <Button variant="success" isLoading={proceeding} onClick={handleProceed}>
                                        ✓ Завершить
                                    </Button>
                                    <Button
                                        variant="outline"
                                        onClick={() => setCancelTarget(selectedProcedure.procedureId)}
                                        style={{ color: 'var(--color-warning)' }}
                                    >
                                        ✗ Отменить
                                    </Button>
                                </>
                            )}
                            <Button variant="danger" onClick={() => { setDeleteTarget(selectedProcedure.procedureId); setIsDetailsOpen(false); }}>
                                Удалить
                            </Button>
                        </>
                    );
                })()}
            >
                {selectedProcedure && (() => {
                    const done      = isDone(selectedProcedure.procedureId);
                    const cancelled = selectedProcedure.isCancelled === true;

                    /* ── Reschedule form ── */
                    if (isRescheduleMode) return (
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
                            <div style={{
                                background: 'var(--color-info-light, #EEF4FB)',
                                border: '1px solid var(--color-info, #3A6B9E)',
                                borderRadius: 'var(--radius-md)',
                                padding: '.75rem 1rem',
                                fontSize: '.875rem',
                                color: 'var(--color-info, #3A6B9E)',
                            }}>
                                Текущая дата: <strong>
                                    {selectedProcedure.scheduledDate
                                        ? format(parseISO(selectedProcedure.scheduledDate), 'dd MMMM yyyy, HH:mm', { locale: ru })
                                        : '—'}
                                </strong>
                            </div>
                            <Input
                                label="Новая дата и время"
                                required
                                type="datetime-local"
                                value={rescheduleDate}
                                min={new Date().toISOString().slice(0, 16)}
                                onChange={e => setRescheduleDate(e.target.value)}
                            />
                        </div>
                    );

                    /* ── Detail view ── */
                    const statusLabel = cancelled ? 'Отменена' : done ? 'Завершена' : 'Ожидается';
                    const statusColor = cancelled ? 'var(--color-warning)' : done ? 'var(--color-success)' : 'var(--text-primary)';

                    return (
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                            {/* Status banners */}
                            {done && (
                                <div style={{
                                    background: 'var(--color-success-light)', border: '1px solid var(--color-success)',
                                    borderRadius: 'var(--radius-md)', padding: '.625rem 1rem',
                                    color: 'var(--color-success)', fontWeight: 700, fontSize: '.875rem',
                                    display: 'flex', alignItems: 'center', gap: '.5rem',
                                }}>✓ Процедура завершена</div>
                            )}
                            {cancelled && (
                                <div style={{
                                    background: 'var(--color-warning-light)', border: '1px solid var(--color-warning)',
                                    borderRadius: 'var(--radius-md)', padding: '.625rem 1rem',
                                    color: 'var(--color-warning)', fontWeight: 700, fontSize: '.875rem',
                                    display: 'flex', alignItems: 'center', gap: '.5rem',
                                }}>✗ Запись отменена</div>
                            )}

                            {/* Fields */}
                            {[
                                { label: 'Процедура',   value: selectedProcedure.title },
                                { label: 'Статус',      value: statusLabel, color: statusColor },
                                { label: 'Дата и время', value: selectedProcedure.scheduledDate
                                    ? format(parseISO(selectedProcedure.scheduledDate), 'dd MMMM yyyy, HH:mm', { locale: ru })
                                    : '—' },
                                { label: 'Пациент',     value: patients.find(p => p.id === selectedProcedure.patientCardId)?.fullname ?? '—' },
                                { label: 'Врач',        value: doctors.find(d => d.id === selectedProcedure.doctorId)?.name ?? 'Не назначен' },
                                { label: 'Стоимость',   value: selectedProcedure.price ? `${selectedProcedure.price} ₽` : '—' },
                            ].map(r => (
                                <div key={r.label}>
                                    <p style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginBottom: '.25rem' }}>{r.label}</p>
                                    <p style={{ fontWeight: 600, color: r.color ?? 'var(--text-primary)' }}>{r.value}</p>
                                </div>
                            ))}
                        </div>
                    );
                })()}
            </Modal>

            {/* Cancel confirm */}
            <Modal
                isOpen={!!cancelTarget}
                onClose={() => setCancelTarget(null)}
                title="Отменить запись?"
                size="sm"
                footer={
                    <>
                        <Button variant="outline" onClick={() => setCancelTarget(null)}>Нет, оставить</Button>
                        <Button variant="outline" isLoading={cancelLoading} onClick={handleCancelProcedure}
                            style={{ color: 'var(--color-warning)', borderColor: 'var(--color-warning)' }}>
                            Да, отменить
                        </Button>
                    </>
                }
            >
                <p style={{ color: 'var(--text-secondary)', lineHeight: 1.6 }}>
                    Запись будет отмечена как <strong>отменённая</strong>. Данные останутся в системе, запись можно будет удалить позже.
                </p>
            </Modal>

            {/* Delete confirm */}
            <Modal isOpen={!!deleteTarget} onClose={() => setDeleteTarget(null)} title="Удалить запись?" size="sm"
                footer={<><Button variant="outline" onClick={() => setDeleteTarget(null)}>Отмена</Button><Button variant="danger" onClick={handleDelete}>Удалить</Button></>}>
                <p style={{ color: 'var(--text-secondary)' }}>Эта запись будет удалена безвозвратно.</p>
            </Modal>

            {/* Add Modal */}
            <Modal isOpen={isAddOpen} onClose={() => { setIsAddOpen(false); setTimeConflictWarning(null); }} title="Запись на процедуру" size="lg"
                footer={<>
                    <Button variant="outline" onClick={() => { setIsAddOpen(false); setTimeConflictWarning(null); }} disabled={formLoading}>Отмена</Button>
                    <Button variant="success" onClick={handleAdd} isLoading={formLoading}>Записать</Button>
                </>}>
                <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                    {loadingRef ? <Spinner /> : (
                        <>
                            <SearchDropdown label="Пациент" required placeholder="Иванов Иван..." hint="Начните вводить ФИО для поиска"
                                value={search.patient} onChange={v => setSearch(s => ({ ...s, patient: v }))}
                                items={patientItems} selectedLabel={selPatient?.fullname}
                                onSelect={(id, label) => { setForm(f => ({ ...f, patientCardId: id })); setSearch(s => ({ ...s, patient: label })); }}
                            />
                            <SearchDropdown label="Тип процедуры" required placeholder="Консультация, пилинг..." hint="Стоимость подставится автоматически"
                                value={search.type} onChange={v => setSearch(s => ({ ...s, type: v }))}
                                items={typeItems} selectedLabel={selType?.title}
                                onSelect={(id, label) => {
                                    const t = procedureTypes.find(x => x.id === id);
                                    setForm(f => ({ ...f, typeId: id, price: String(t?.price ?? 0) }));
                                    setSearch(s => ({ ...s, type: label }));
                                }}
                            />
                            <SearchDropdown label="Врач" optional placeholder="Петрова Наталья..." hint="Оставьте пустым, если врач не назначен"
                                value={search.doctor} onChange={v => setSearch(s => ({ ...s, doctor: v }))}
                                items={doctorItems} selectedLabel={selDoctor?.name}
                                onSelect={(id, label) => { setForm(f => ({ ...f, doctorId: id })); setSearch(s => ({ ...s, doctor: label })); }}
                            />
                            <Input label="Дата и время" required type="datetime-local"
                                value={form.scheduledDate} min={new Date().toISOString().slice(0, 16)}
                                onChange={e => setForm(f => ({ ...f, scheduledDate: e.target.value }))}
                            />
                            {timeConflictWarning && (
                                <div style={{
                                    background: 'var(--color-warning-light)',
                                    border: '1px solid var(--color-warning)',
                                    borderRadius: 'var(--radius-sm)',
                                    padding: '.625rem .875rem',
                                    fontSize: '.8125rem',
                                    color: 'var(--color-warning)',
                                    fontWeight: 600,
                                    display: 'flex',
                                    alignItems: 'flex-start',
                                    gap: '.5rem',
                                }}>
                                    <span>⚠</span>
                                    <span>Пересечение с процедурой {timeConflictWarning}. Вы можете создать запись несмотря на это.</span>
                                </div>
                            )}
                            <Input label="Стоимость (₽)" type="number" min="0" placeholder="0"
                                value={form.price} onChange={e => setForm(f => ({ ...f, price: e.target.value }))}
                            />
                        </>
                    )}
                </div>
            </Modal>
        </div>
    );
};

export default SchedulePage;
