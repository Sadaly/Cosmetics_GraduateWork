import React, { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../api/api';
import type { PatientCard } from '../TypesFromServer/PatientCard';
import { Header } from '../Components/Layout/Header';
import { Card } from '../Components/UI/Card';
import { Button } from '../Components/UI/Button';
import { Input } from '../Components/UI/Input';
import { Badge } from '../Components/UI/Badge';
import { Spinner } from '../Components/UI/Spinner';
import { debounce } from 'lodash';
import { useToast } from '../Hooks/useToast';

// ─── Shared types ─────────────────────────────────────────────────────────────
interface TypedItem { id: string; typeId: string; typeName?: string; patientCardId: string; date?: string; }
interface ItemType  { id: string; title: string; }

// ─── Reusable TagSection — eliminates 4× duplication ─────────────────────────
interface TagSectionProps {
    items: TypedItem[];
    types: ItemType[];
    loading: boolean;
    inputValue: string;
    suggestions: ItemType[];
    onInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    onSelectType: (id: string) => void;
    onCreateNew: (title: string) => void;
    placeholder: string;
    badgeVariant?: 'primary' | 'success' | 'danger' | 'warning' | 'neutral';
    emptyMessage: string;
    showDate?: boolean;
}

const TagSection: React.FC<TagSectionProps> = ({
    items, types, loading, inputValue, suggestions,
    onInputChange, onSelectType, onCreateNew,
    placeholder, badgeVariant = 'primary', emptyMessage, showDate = false,
}) => {
    const getTypeName = (item: TypedItem) => item.typeName ?? types.find(t => t.id === item.typeId)?.title ?? '—';

    return (
        <div>
            {/* Search input */}
            <div style={{ position: 'relative', marginBottom: '1.25rem' }}>
                <input
                    type="text"
                    value={inputValue}
                    onChange={onInputChange}
                    placeholder={placeholder}
                    className="form-input"
                    style={{ maxWidth: 320 }}
                />
                {suggestions.length > 0 && (
                    <div style={{
                        position: 'absolute', top: '100%', left: 0,
                        width: 320, marginTop: 4, zIndex: 20,
                        background: 'var(--bg-surface)',
                        border: '1px solid var(--border-default)',
                        borderRadius: 'var(--radius-md)',
                        boxShadow: 'var(--shadow-md)',
                        maxHeight: 200, overflowY: 'auto',
                    }}>
                        {suggestions.map(type => (
                            <button
                                key={type.id}
                                onClick={() => onSelectType(type.id)}
                                style={{
                                    display: 'block', width: '100%',
                                    textAlign: 'left', padding: '.625rem 1rem',
                                    background: 'none', border: 'none', cursor: 'pointer',
                                    fontSize: '.875rem', color: 'var(--text-primary)',
                                    transition: 'background var(--transition-fast)',
                                }}
                                onMouseEnter={e => (e.currentTarget.style.background = 'var(--bg-muted)')}
                                onMouseLeave={e => (e.currentTarget.style.background = 'none')}
                            >
                                {type.title}
                            </button>
                        ))}
                    </div>
                )}
                {inputValue.trim() && suggestions.length === 0 && (
                    <div style={{
                        position: 'absolute', top: '100%', left: 0,
                        width: 320, marginTop: 4, zIndex: 20,
                        background: 'var(--bg-surface)',
                        border: '1px solid var(--border-default)',
                        borderRadius: 'var(--radius-md)',
                        boxShadow: 'var(--shadow-md)',
                    }}>
                        <button
                            onClick={() => onCreateNew(inputValue.trim())}
                            style={{
                                display: 'block', width: '100%',
                                textAlign: 'left', padding: '.625rem 1rem',
                                background: 'none', border: 'none', cursor: 'pointer',
                                fontSize: '.875rem', fontWeight: 600, color: 'var(--color-success)',
                                transition: 'background var(--transition-fast)',
                            }}
                            onMouseEnter={e => (e.currentTarget.style.background = 'var(--color-success-light)')}
                            onMouseLeave={e => (e.currentTarget.style.background = 'none')}
                        >
                            + Создать: «{inputValue.trim()}»
                        </button>
                    </div>
                )}
            </div>

            {/* Items */}
            {loading ? (
                <Spinner />
            ) : items.length === 0 ? (
                <p style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem 0' }}>
                    {emptyMessage}
                </p>
            ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3">
                    {items.map(item => (
                        <div key={item.id} style={{
                            padding: '.75rem 1rem',
                            background: 'var(--bg-muted)',
                            borderRadius: 'var(--radius-md)',
                            border: '1px solid var(--border-subtle)',
                        }}>
                            <Badge variant={badgeVariant}>{getTypeName(item)}</Badge>
                            {showDate && item.date && (
                                <p style={{ fontSize: '.75rem', color: 'var(--text-muted)', marginTop: '.5rem' }}>
                                    {new Date(item.date).toLocaleDateString('ru-RU')}
                                </p>
                            )}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

// ─── Main Page ────────────────────────────────────────────────────────────────
type TabId = 'info' | 'health' | 'procedures' | 'skin' | 'features';

const PatientDetailsPage: React.FC = () => {
    const { showSuccess, showError } = useToast();
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [patientCard, setPatientCard] = useState<PatientCard>({} as PatientCard);
    const [isLoading, setIsLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [activeTab, setActiveTab] = useState<TabId>('info');

    // ── Per-tab state factory ─────────────────────────────────────────────────
    const makeTabState = () => ({
        items: [] as TypedItem[],
        types: [] as ItemType[],
        input: '',
        suggestions: [] as ItemType[],
        loading: false,
    });

    const [health,     setHealth]     = useState(makeTabState());
    const [procedures, setProcedures] = useState(makeTabState());
    const [skinCare,   setSkinCare]   = useState(makeTabState());
    const [features,   setFeatures]   = useState(makeTabState());

    // ── Load patient card ─────────────────────────────────────────────────────
    useEffect(() => {
        if (!id) return;
        api.get(`/PatientCards/${id}`)
            .then(r => setPatientCard(r.data))
            .catch(() => showError('Не удалось загрузить данные пациента'))
            .finally(() => setIsLoading(false));
    }, [id]);

    // ── Load all type dictionaries ────────────────────────────────────────────
    useEffect(() => {
        Promise.all([
            api.get('/HealthCondTypes/All'),
            api.get('/ExternalProcedureRecordTypes/All'),
            api.get('/SkinCareTypes/All'),
            api.get('/SkinFeatureTypes/All'),
        ]).then(([hc, ep, sc, sf]) => {
            setHealth(s => ({ ...s, types: hc.data }));
            setProcedures(s => ({ ...s, types: ep.data }));
            setSkinCare(s => ({ ...s, types: sc.data }));
            setFeatures(s => ({ ...s, types: sf.data }));
        }).catch(() => {});
    }, []);

    // ── Load items for each tab ───────────────────────────────────────────────
    const loadItems = useCallback(async (cardId: string, fullname: string) => {
        const fetch = async (setter: React.Dispatch<any>, url: string) => {
            setter((s: any) => ({ ...s, loading: true }));
            try {
                const r = await api.get(url, { params: { PatientName: fullname } });
                setter((s: any) => ({ ...s, items: r.data, loading: false }));
            } catch { setter((s: any) => ({ ...s, loading: false })); }
        };
        fetch(setHealth,     '/HealthConds/All');
        fetch(setProcedures, '/ExternalProcedureRecords/All');
        fetch(setSkinCare,   '/SkinCares/All');
        fetch(setFeatures,   '/SkinFeatures/All');
    }, []);

    useEffect(() => {
        if (patientCard.id) loadItems(patientCard.id, patientCard.fullname);
    }, [patientCard.id]);

    // ── Debounced search factories ────────────────────────────────────────────
    const makeSearch = useCallback((searchUrl: string, setter: React.Dispatch<any>) =>
        debounce(async (query: string) => {
            if (!query.trim()) { setter((s: any) => ({ ...s, suggestions: [] })); return; }
            try {
                const r = await api.get(searchUrl, { params: { Typename: query, Count: 10 } });
                setter((s: any) => ({ ...s, suggestions: r.data }));
            } catch {}
        }, 300),
    []);

    // Memoized search functions
    const searchHealth     = useCallback(makeSearch('/HealthCondTypes/Take',              setHealth),     []);
    const searchProcedures = useCallback(makeSearch('/ExternalProcedureRecordTypes/Take', setProcedures), []);
    const searchSkinCare   = useCallback(makeSearch('/SkinCareTypes/Take',                setSkinCare),   []);
    const searchFeatures   = useCallback(makeSearch('/SkinFeatureTypes/Take',             setFeatures),   []);

    // ── Generic add helper ────────────────────────────────────────────────────
    const addItem = async (
        setter: React.Dispatch<any>,
        typesSetter: React.Dispatch<any>,
        postUrl: string,
        typesUrl: string,
        createTypeUrl: string,
        body: (typeId: string) => object,
        successMsg: string,
        typeId: string,
        isNew = false,
        title = '',
    ) => {
        try {
            if (isNew && title) {
                const r = await api.post(createTypeUrl, { title });
                typeId = r.data?.id ?? r.data;
                const updated = await api.get(typesUrl);
                setter((s: any) => ({ ...s, types: updated.data }));
            }
            await api.post(postUrl, body(typeId));
            setter((s: any) => ({ ...s, input: '', suggestions: [] }));
            showSuccess(successMsg);
            // Re-fetch items
            const r = await api.get(postUrl + '/All', { params: { PatientName: patientCard.fullname } });
            setter((s: any) => ({ ...s, items: r.data }));
        } catch { showError('Ошибка при добавлении'); }
    };

    // ── Save handlers ─────────────────────────────────────────────────────────
    const savePatient = async () => {
        setSaving(true);
        try {
            await api.put('/Patients/Update', { id: patientCard.patientId, fullName: patientCard.fullname });
            showSuccess('ФИО пациента обновлено');
        } catch { showError('Не удалось сохранить'); }
        finally { setSaving(false); }
    };

    const saveCard = async () => {
        setSaving(true);
        try {
            await api.put('/PatientCards', {
                id: patientCard.id,
                age: patientCard.age,
                address: patientCard.address,
                complaints: patientCard.complaints,
                phoneNumber: patientCard.phoneNumber,
            });
            showSuccess('Карточка пациента обновлена');
        } catch { showError('Не удалось сохранить карточку'); }
        finally { setSaving(false); }
    };

    const handleCardChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatientCard(prev => ({ ...prev, [name]: name === 'age' ? Number(value) : value }));
    };

    // ── Tab config ────────────────────────────────────────────────────────────
    const tabs: { id: TabId; label: string; count?: number }[] = [
        { id: 'info',       label: 'Информация' },
        { id: 'health',     label: 'Здоровье',    count: health.items.length },
        { id: 'procedures', label: 'Процедуры',   count: procedures.items.length },
        { id: 'skin',       label: 'Уход',        count: skinCare.items.length },
        { id: 'features',   label: 'Особенности', count: features.items.length },
    ];

    if (isLoading) return (
        <div className="flex items-center justify-center" style={{ minHeight: '60vh' }}><Spinner size="lg" /></div>
    );

    return (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <Header
                title={patientCard.fullname ? `Пациент: ${patientCard.fullname}` : 'Карта пациента'}
                subtitle="Детальная информация и медицинская карта"
                actions={
                    <Button onClick={() => navigate('/patients')} variant="outline">← Назад</Button>
                }
            />

            {/* Tabs */}
            <div style={{
                background: 'var(--bg-surface)',
                borderRadius: 'var(--radius-lg)',
                border: '1px solid var(--border-subtle)',
                boxShadow: 'var(--shadow-sm)',
                padding: '0 1.5rem',
            }}>
                <nav style={{ display: 'flex', gap: '0', overflowX: 'auto' }}>
                    {tabs.map(tab => (
                        <button
                            key={tab.id}
                            onClick={() => setActiveTab(tab.id)}
                            style={{
                                padding: '1rem 1.25rem',
                                border: 'none',
                                borderBottom: activeTab === tab.id ? '2px solid var(--color-primary)' : '2px solid transparent',
                                background: 'none',
                                cursor: 'pointer',
                                fontFamily: "'Nunito', sans-serif",
                                fontWeight: activeTab === tab.id ? 700 : 500,
                                fontSize: '.875rem',
                                color: activeTab === tab.id ? 'var(--color-primary)' : 'var(--text-muted)',
                                whiteSpace: 'nowrap',
                                transition: 'all var(--transition-fast)',
                                display: 'flex', alignItems: 'center', gap: '.5rem',
                            }}
                            onMouseEnter={e => {
                                if (activeTab !== tab.id) (e.currentTarget as HTMLElement).style.color = 'var(--text-secondary)';
                            }}
                            onMouseLeave={e => {
                                if (activeTab !== tab.id) (e.currentTarget as HTMLElement).style.color = 'var(--text-muted)';
                            }}
                        >
                            {tab.label}
                            {tab.count !== undefined && (
                                <span style={{
                                    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                                    minWidth: 18, height: 18, borderRadius: 9,
                                    background: activeTab === tab.id ? 'var(--color-primary)' : 'var(--bg-muted)',
                                    color: activeTab === tab.id ? '#fff' : 'var(--text-muted)',
                                    fontSize: '.6875rem', fontWeight: 700, padding: '0 4px',
                                }}>
                                    {tab.count}
                                </span>
                            )}
                        </button>
                    ))}
                </nav>
            </div>

            {/* Tab content */}
            {activeTab === 'info' && (
                <Card title="Основная информация">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Full name with inline save */}
                        <div>
                            <label className="form-label">ФИО пациента</label>
                            <div style={{ display: 'flex', gap: '.5rem' }}>
                                <input
                                    type="text" name="fullname"
                                    value={patientCard.fullname ?? ''}
                                    onChange={handleCardChange}
                                    className="form-input"
                                    style={{ borderRadius: 'var(--radius-md) 0 0 var(--radius-md)', flex: 1 }}
                                />
                                <Button
                                    onClick={savePatient} variant="success" disabled={saving}
                                    style={{ borderRadius: '0 var(--radius-md) var(--radius-md) 0', whiteSpace: 'nowrap' }}
                                >
                                    {saving ? '...' : 'Сохранить'}
                                </Button>
                            </div>
                        </div>

                        <Input label="Возраст" type="number" name="age"
                            value={patientCard.age ?? ''} onChange={handleCardChange} />
                        <Input label="Адрес" type="text" name="address"
                            value={patientCard.address ?? ''} onChange={handleCardChange} />
                        <Input label="Телефон" type="tel" name="phoneNumber"
                            value={patientCard.phoneNumber ?? ''} onChange={handleCardChange} />

                        <div className="md:col-span-2">
                            <label className="form-label">Жалобы</label>
                            <textarea
                                name="complaints"
                                value={patientCard.complaints ?? ''}
                                onChange={handleCardChange}
                                rows={4}
                                className="form-input"
                                style={{ resize: 'vertical' }}
                            />
                        </div>
                    </div>
                    <div style={{ marginTop: '1.5rem' }}>
                        <Button onClick={saveCard} disabled={saving} variant="primary">
                            {saving ? 'Сохранение...' : 'Сохранить все изменения'}
                        </Button>
                    </div>
                </Card>
            )}

            {activeTab === 'health' && (
                <Card title="Состояния здоровья">
                    <TagSection
                        items={health.items} types={health.types} loading={health.loading}
                        inputValue={health.input}
                        suggestions={health.suggestions}
                        onInputChange={e => { setHealth(s => ({ ...s, input: e.target.value })); searchHealth(e.target.value); }}
                        onSelectType={async id => {
                            await api.post('/HealthConds', { patientCardId: patientCard.id, typeId: id });
                            setHealth(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        onCreateNew={async title => {
                            const r = await api.post('/HealthCondTypes', { title });
                            const typeId = r.data?.id ?? r.data;
                            await api.post('/HealthConds', { patientCardId: patientCard.id, typeId });
                            setHealth(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        placeholder="Поиск или добавление состояния..."
                        badgeVariant="danger"
                        emptyMessage="Нет данных о состоянии здоровья"
                    />
                </Card>
            )}

            {activeTab === 'procedures' && (
                <Card title="Внешние процедуры">
                    <TagSection
                        items={procedures.items} types={procedures.types} loading={procedures.loading}
                        inputValue={procedures.input}
                        suggestions={procedures.suggestions}
                        onInputChange={e => { setProcedures(s => ({ ...s, input: e.target.value })); searchProcedures(e.target.value); }}
                        onSelectType={async id => {
                            await api.post('/ExternalProcedureRecords', { patientCardId: patientCard.id, typeId: id, date: new Date().toISOString() });
                            setProcedures(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        onCreateNew={async title => {
                            const r = await api.post('/ExternalProcedureRecordTypes', { title });
                            const typeId = r.data?.id ?? r.data;
                            await api.post('/ExternalProcedureRecords', { patientCardId: patientCard.id, typeId, date: new Date().toISOString() });
                            setProcedures(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        placeholder="Поиск или добавление процедуры..."
                        badgeVariant="primary"
                        emptyMessage="Нет записей о внешних процедурах"
                        showDate
                    />
                </Card>
            )}

            {activeTab === 'skin' && (
                <Card title="Уход за кожей">
                    <TagSection
                        items={skinCare.items} types={skinCare.types} loading={skinCare.loading}
                        inputValue={skinCare.input}
                        suggestions={skinCare.suggestions}
                        onInputChange={e => { setSkinCare(s => ({ ...s, input: e.target.value })); searchSkinCare(e.target.value); }}
                        onSelectType={async id => {
                            await api.post('/SkinCares', { patientCardId: patientCard.id, typeId: id });
                            setSkinCare(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        onCreateNew={async title => {
                            const r = await api.post('/SkinCareTypes', { title });
                            const typeId = r.data?.id ?? r.data;
                            await api.post('/SkinCares', { patientCardId: patientCard.id, typeId });
                            setSkinCare(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        placeholder="Поиск или добавление ухода..."
                        badgeVariant="success"
                        emptyMessage="Нет назначений по уходу за кожей"
                    />
                </Card>
            )}

            {activeTab === 'features' && (
                <Card title="Особенности кожи">
                    <TagSection
                        items={features.items} types={features.types} loading={features.loading}
                        inputValue={features.input}
                        suggestions={features.suggestions}
                        onInputChange={e => { setFeatures(s => ({ ...s, input: e.target.value })); searchFeatures(e.target.value); }}
                        onSelectType={async id => {
                            await api.post('/SkinFeatures', { patientCardId: patientCard.id, typeId: id });
                            setFeatures(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        onCreateNew={async title => {
                            const r = await api.post('/SkinFeatureTypes/Create', { title });
                            const typeId = r.data?.id ?? r.data;
                            await api.post('/SkinFeatures', { patientCardId: patientCard.id, typeId });
                            setFeatures(s => ({ ...s, input: '', suggestions: [] }));
                            showSuccess('Добавлено'); loadItems(patientCard.id, patientCard.fullname);
                        }}
                        placeholder="Поиск или добавление особенности..."
                        badgeVariant="warning"
                        emptyMessage="Нет данных об особенностях кожи"
                    />
                </Card>
            )}
        </div>
    );
};

export default PatientDetailsPage;
