import React, { useEffect, useState, useCallback } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../api/api";
import type { PatientCard } from "../TypesFromServer/PatientCard";
import type { ErrorResponse } from "../TypesFromServer/ErrorResponse";
import { debounce } from 'lodash';

// Define minimal types needed for this page
interface HealthCond {
    id: string;
    patientCardId: string;
    typeId: string;
    healthCondType?: { title: string };
}

interface HealthCondType {
    id: string;
    title: string;
}

interface ExternalProcedureRecord {
    id: string;
    patientCardId: string;
    typeId: string;
    date: string; // ISO 8601
    externalProcedureRecordType?: { title: string };
}

interface ExternalProcedureRecordType {
    id: string;
    title: string;
}

interface SkinCare {
    id: string;
    patientCardId: string;
    typeId: string;
    skinCareType?: { title: string };
}

interface SkinCareType {
    id: string;
    title: string;
}

interface SkinFeature {
    id: string;
    patientCardId: string;
    typeId: string;
    skinFeatureType?: { title: string };
}

interface SkinFeatureType {
    id: string;
    title: string;
}

const PatientDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [patientCard, setPatientCard] = useState<PatientCard>({} as PatientCard);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    // Health Conditions state
    const [healthConds, setHealthConds] = useState<HealthCond[]>([]);
    const [healthCondInput, setHealthCondInput] = useState("");
    const [healthCondSuggestions, setHealthCondSuggestions] = useState<HealthCondType[]>([]);
    const [loadingHealthConds, setLoadingHealthConds] = useState(false);
    const [addingHealthCond, setAddingHealthCond] = useState(false);
    const [healthCondTypes, setHealthCondTypes] = useState<HealthCondType[]>([]);

    // External Procedures state
    const [externalProcedures, setExternalProcedures] = useState<ExternalProcedureRecord[]>([]);
    const [externalProcedureInput, setExternalProcedureInput] = useState("");
    const [externalProcedureSuggestions, setExternalProcedureSuggestions] = useState<ExternalProcedureRecordType[]>([]);
    const [loadingExternalProcedures, setLoadingExternalProcedures] = useState(false);
    const [addingExternalProcedure, setAddingExternalProcedure] = useState(false);
    const [externalProcedureTypes, setExternalProcedureTypes] = useState<ExternalProcedureRecordType[]>([]);

    // Skin Cares state
    const [skinCares, setSkinCares] = useState<SkinCare[]>([]);
    const [skinCareInput, setSkinCareInput] = useState("");
    const [skinCareSuggestions, setSkinCareSuggestions] = useState<SkinCareType[]>([]);
    const [loadingSkinCares, setLoadingSkinCares] = useState(false);
    const [addingSkinCare, setAddingSkinCare] = useState(false);
    const [skinCareTypes, setSkinCareTypes] = useState<SkinCareType[]>([]);

    // Skin Features state
    const [skinFeatures, setSkinFeatures] = useState<SkinFeature[]>([]);
    const [skinFeatureInput, setSkinFeatureInput] = useState("");
    const [skinFeatureSuggestions, setSkinFeatureSuggestions] = useState<SkinFeatureType[]>([]);
    const [loadingSkinFeatures, setLoadingSkinFeatures] = useState(false);
    const [addingSkinFeature, setAddingSkinFeature] = useState(false);
    const [skinFeatureTypes, setSkinFeatureTypes] = useState<SkinFeatureType[]>([]);

    // Fetch patient card data
    useEffect(() => {
        if (!id) return;

        const fetchPatientCard = async () => {
            try {
                const responsePC = await api.get(`/PatientCards/${id}`);
                setPatientCard(responsePC.data);
            } catch (err) {
                console.error("Ошибка загрузки карты пациента", err);
                setError("Не удалось загрузить данные пациента");
            } finally {
                setIsLoading(false);
            }
        };
        fetchPatientCard();
    }, [id]);

    // Fetch related data types
    useEffect(() => {
        const fetchDataTypes = async () => {
            try {
                // Fetch all types in parallel
                const [
                    healthCondTypesRes,
                    externalProcedureTypesRes,
                    skinCareTypesRes,
                    skinFeatureTypesRes
                ] = await Promise.all([
                    api.get('/HealthCondTypes/All'),
                    api.get('/ExternalProcedureRecordTypes/All'),
                    api.get('/SkinCareTypes/All'),
                    api.get('/SkinFeatureTypes/All')
                ]);

                setHealthCondTypes(healthCondTypesRes.data);
                setExternalProcedureTypes(externalProcedureTypesRes.data);
                setSkinCareTypes(skinCareTypesRes.data);
                setSkinFeatureTypes(skinFeatureTypesRes.data);
            } catch (err) {
                console.error("Ошибка загрузки типов данных", err);
            }
        };

        fetchDataTypes();
    }, []);

    // Fetch related data when patientCard is available
    useEffect(() => {
        if (!patientCard.id) return;

        const fetchData = async () => {
            await Promise.all([
                fetchHealthConds(),
                fetchExternalProcedures(),
                fetchSkinCares(),
                fetchSkinFeatures()
            ]);
        };

        fetchData();
    }, [patientCard.id, healthCondTypes, externalProcedureTypes, skinCareTypes, skinFeatureTypes]);

    // Get type title by ID
    const getHealthCondTypeName = (typeId: string) => {
        const type = healthCondTypes.find(t => t.id === typeId);
        return type ? type.title : 'Неизвестное состояние';
    };

    const getExternalProcedureTypeName = (typeId: string) => {
        const type = externalProcedureTypes.find(t => t.id === typeId);
        return type ? type.title : 'Неизвестная процедура';
    };

    const getSkinCareTypeName = (typeId: string) => {
        const type = skinCareTypes.find(t => t.id === typeId);
        return type ? type.title : 'Неизвестный уход';
    };

    const getSkinFeatureTypeName = (typeId: string) => {
        const type = skinFeatureTypes.find(t => t.id === typeId);
        return type ? type.title : 'Неизвестная особенность';
    };

    // Health Conditions functions
    const fetchHealthConds = async () => {
        setLoadingHealthConds(true);
        try {
            const response = await api.get(`/HealthConds/All`, {
                params: { PatientName: patientCard.fullname }
            });
            setHealthConds(response.data);
        } catch (err) {
            console.error("Ошибка загрузки состояний здоровья", err);
        } finally {
            setLoadingHealthConds(false);
        }
    };

    const searchHealthCondTypes = useCallback(
        debounce(async (query: string) => {
            if (!query.trim()) {
                setHealthCondSuggestions([]);
                return;
            }

            try {
                const response = await api.get(`/HealthCondTypes/Take`, {
                    params: {
                        Typename: query,
                        Count: 10
                    }
                });
                setHealthCondSuggestions(response.data);
            } catch (err) {
                console.error("Ошибка поиска типов состояний здоровья", err);
            }
        }, 300),
        []
    );

    const handleHealthCondInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setHealthCondInput(value);
        searchHealthCondTypes(value);
    };

    const addHealthCond = async (typeId: string, isNewType = false, title = "") => {
        setAddingHealthCond(true);
        try {
            if (isNewType && title) {
                const createResponse = await api.post(`/HealthCondTypes`, { title });
                typeId = createResponse.data?.id || createResponse.data;
                // Update types list after creating new type
                const updatedTypes = await api.get('/HealthCondTypes/All');
                setHealthCondTypes(updatedTypes.data);
            }

            await api.post(`/HealthConds`, {
                patientCardId: patientCard.id,
                typeId
            });

            setHealthCondInput("");
            setHealthCondSuggestions([]);
            await fetchHealthConds();
        } catch (err) {
            console.error("Ошибка добавления состояния здоровья", err);
            if (err.response) {
                console.error("Response ", err.response.data);
                setError(`Ошибка добавления: ${JSON.stringify(err.response.data)}`);
            }
        } finally {
            setAddingHealthCond(false);
        }
    };

    // External Procedures functions
    const fetchExternalProcedures = async () => {
        setLoadingExternalProcedures(true);
        try {
            const response = await api.get(`/ExternalProcedureRecords/All`, {
                params: { PatientName: patientCard.fullname }
            });
            setExternalProcedures(response.data);
        } catch (err) {
            console.error("Ошибка загрузки внешних процедур", err);
        } finally {
            setLoadingExternalProcedures(false);
        }
    };

    const searchExternalProcedureTypes = useCallback(
        debounce(async (query: string) => {
            if (!query.trim()) {
                setExternalProcedureSuggestions([]);
                return;
            }

            try {
                const response = await api.get(`/ExternalProcedureRecordTypes/Take`, {
                    params: {
                        Typename: query,
                        Count: 10
                    }
                });
                setExternalProcedureSuggestions(response.data);
            } catch (err) {
                console.error("Ошибка поиска типов внешних процедур", err);
            }
        }, 300),
        []
    );

    const handleExternalProcedureInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setExternalProcedureInput(value);
        searchExternalProcedureTypes(value);
    };

    const addExternalProcedure = async (typeId: string, isNewType = false, title = "") => {
        setAddingExternalProcedure(true);
        try {
            if (isNewType && title) {
                const createResponse = await api.post(`/ExternalProcedureRecordTypes`, { title });
                typeId = createResponse.data?.id || createResponse.data;
                // Update types list
                const updatedTypes = await api.get('/ExternalProcedureRecordTypes/All');
                setExternalProcedureTypes(updatedTypes.data);
            }

            await api.post(`/ExternalProcedureRecords`, {
                patientCardId: patientCard.id,
                typeId,
                date: new Date().toISOString()
            });

            setExternalProcedureInput("");
            setExternalProcedureSuggestions([]);
            await fetchExternalProcedures();
        } catch (err) {
            console.error("Ошибка добавления внешней процедуры", err);
            if (err.response) {
                console.error("Response ", err.response.data);
                setError(`Ошибка добавления: ${JSON.stringify(err.response.data)}`);
            }
        } finally {
            setAddingExternalProcedure(false);
        }
    };

    // Skin Cares functions
    const fetchSkinCares = async () => {
        setLoadingSkinCares(true);
        try {
            const response = await api.get(`/SkinCares/All`, {
                params: { PatientName: patientCard.fullname }
            });
            setSkinCares(response.data);
        } catch (err) {
            console.error("Ошибка загрузки ухода за кожей", err);
        } finally {
            setLoadingSkinCares(false);
        }
    };

    const searchSkinCareTypes = useCallback(
        debounce(async (query: string) => {
            if (!query.trim()) {
                setSkinCareSuggestions([]);
                return;
            }

            try {
                const response = await api.get(`/SkinCareTypes/Take`, {
                    params: {
                        Typename: query,
                        Count: 10
                    }
                });
                setSkinCareSuggestions(response.data);
            } catch (err) {
                console.error("Ошибка поиска типов ухода за кожей", err);
            }
        }, 300),
        []
    );

    const handleSkinCareInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setSkinCareInput(value);
        searchSkinCareTypes(value);
    };

    const addSkinCare = async (typeId: string, isNewType = false, title = "") => {
        setAddingSkinCare(true);
        try {
            if (isNewType && title) {
                const createResponse = await api.post(`/SkinCareTypes`, { title });
                typeId = createResponse.data?.id || createResponse.data;
                // Update types list
                const updatedTypes = await api.get('/SkinCareTypes/All');
                setSkinCareTypes(updatedTypes.data);
            }

            await api.post(`/SkinCares`, {
                patientCardId: patientCard.id,
                typeId
            });

            setSkinCareInput("");
            setSkinCareSuggestions([]);
            await fetchSkinCares();
        } catch (err) {
            console.error("Ошибка добавления ухода за кожей", err);
            if (err.response) {
                console.error("Response ", err.response.data);
                setError(`Ошибка добавления: ${JSON.stringify(err.response.data)}`);
            }
        } finally {
            setAddingSkinCare(false);
        }
    };

    // Skin Features functions
    const fetchSkinFeatures = async () => {
        setLoadingSkinFeatures(true);
        try {
            const response = await api.get(`/SkinFeatures/All`, {
                params: { PatientName: patientCard.fullname }
            });
            setSkinFeatures(response.data);
        } catch (err) {
            console.error("Ошибка загрузки особенностей кожи", err);
        } finally {
            setLoadingSkinFeatures(false);
        }
    };

    const searchSkinFeatureTypes = useCallback(
        debounce(async (query: string) => {
            if (!query.trim()) {
                setSkinFeatureSuggestions([]);
                return;
            }

            try {
                const response = await api.get(`/SkinFeatureTypes/Take`, {
                    params: {
                        Typename: query,
                        Count: 10
                    }
                });
                setSkinFeatureSuggestions(response.data);
            } catch (err) {
                console.error("Ошибка поиска типов особенностей кожи", err);
            }
        }, 300),
        []
    );

    const handleSkinFeatureInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        setSkinFeatureInput(value);
        searchSkinFeatureTypes(value);
    };

    const addSkinFeature = async (typeId: string, isNewType = false, title = "") => {
        setAddingSkinFeature(true);
        try {
            if (isNewType && title) {
                const createResponse = await api.post(`/SkinFeatureTypes/Create`, { title });
                typeId = createResponse.data?.id || createResponse.data;
                // Update types list
                const updatedTypes = await api.get('/SkinFeatureTypes/All');
                setSkinFeatureTypes(updatedTypes.data);
            }

            await api.post(`/SkinFeatures`, {
                patientCardId: patientCard.id,
                typeId
            });

            setSkinFeatureInput("");
            setSkinFeatureSuggestions([]);
            await fetchSkinFeatures();
        } catch (err) {
            console.error("Ошибка добавления особенности кожи", err);
            if (err.response) {
                console.error("Response ", err.response.data);
                setError(`Ошибка добавления: ${JSON.stringify(err.response.data)}`);
            }
        } finally {
            setAddingSkinFeature(false);
        }
    };

    // Save patient data
    const savePatient = async () => {
        try {
            setSaving(true);
            await api.put(`/Patients/Update`, {
                id: patientCard.patientId,
                fullName: patientCard.fullname,
            });
            alert("Данные пациента успешно обновлены");
        } catch (err: any) {
            const errorResponse = err.response?.data as ErrorResponse[];
            setError(errorResponse?.map(e => e.message).join('\n') || "Не удалось сохранить данные пациента");
            console.error(err);
        } finally {
            setSaving(false);
        }
    };

    // Save patient card data
    const saveCard = async () => {
        try {
            setSaving(true);
            await api.put(`/PatientCards`, {
                id: patientCard.id,
                age: patientCard.age,
                address: patientCard.address,
                complaints: patientCard.complaints,
                phoneNumber: patientCard.phoneNumber
            });
            alert("Карточка пациента успешно обновлена");
        } catch (err: any) {
            const errorResponse = err.response?.data as ErrorResponse[];
            setError(errorResponse?.map(e => e.message).join('\n') || "Не удалось сохранить карточку пациента");
            console.error(err);
        } finally {
            setSaving(false);
        }
    };

    const handleCardChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatientCard(prev => ({
            ...prev,
            [name]: name === "age" ? Number(value) : value,
        }));
        setError("");
    };

    if (isLoading) return (
        <div className="flex justify-center items-center h-screen">
            <div className="animate-spin rounded-full h-24 w-24 border-b-2 border-blue-600"></div>
        </div>
    );

    if (error) return (
        <div className="p-6 max-w-4xl mx-auto">
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                {error}
            </div>
            <button
                onClick={() => navigate(-1)}
                className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            >
                Вернуться назад
            </button>
        </div>
    );

    if (!patientCard.id) return (
        <div className="p-6 max-w-4xl mx-auto text-center">
            <h2 className="text-2xl font-bold mb-4">Пациент не найден</h2>
            <button
                onClick={() => navigate('/patients')}
                className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            >
                Вернуться к списку пациентов
            </button>
        </div>
    );

    return (
        <div className="p-6 max-w-6xl mx-auto bg-gray-50 min-h-screen">
            <div className="mb-8 flex justify-between items-center">
                <h1 className="text-3xl font-bold text-gray-800">Профиль пациента: {patientCard.fullname}</h1>
                <button
                    onClick={() => navigate('/patients')}
                    className="px-4 py-2 bg-gray-200 hover:bg-gray-300 rounded-lg transition-colors"
                >
                    ← Назад к списку пациентов
                </button>
            </div>

            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-6">
                    {error}
                </div>
            )}

            {/* Patient Profile Section */}
            <div className="bg-white rounded-xl shadow-md p-6 mb-8 border border-gray-200">
                <h2 className="text-2xl font-semibold mb-4 text-blue-700 flex items-center">
                    <span>Основная информация</span>
                </h2>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div>
                        <label className="block text-gray-700 font-medium mb-2">ФИО пациента</label>
                        <div className="flex">
                            <input
                                type="text"
                                name="fullname"
                                value={patientCard.fullname || ""}
                                onChange={handleCardChange}
                                className="flex-1 px-4 py-2 border border-gray-300 rounded-l-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <button
                                onClick={savePatient}
                                disabled={saving}
                                className={`px-4 py-2 rounded-r-lg font-medium transition-colors ${saving
                                        ? 'bg-gray-400 cursor-not-allowed'
                                        : 'bg-green-500 hover:bg-green-600 text-white'
                                    }`}
                            >
                                {saving ? 'Сохранение...' : 'Сохранить ФИО'}
                            </button>
                        </div>
                    </div>

                    <div>
                        <label className="block text-gray-700 font-medium mb-2">Возраст</label>
                        <input
                            type="number"
                            name="age"
                            value={patientCard.age || ""}
                            onChange={handleCardChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div>
                        <label className="block text-gray-700 font-medium mb-2">Адрес</label>
                        <input
                            type="text"
                            name="address"
                            value={patientCard.address || ""}
                            onChange={handleCardChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div>
                        <label className="block text-gray-700 font-medium mb-2">Телефон</label>
                        <input
                            type="text"
                            name="phoneNumber"
                            value={patientCard.phoneNumber || ""}
                            onChange={handleCardChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                </div>

                <div className="mb-6">
                    <label className="block text-gray-700 font-medium mb-2">Жалобы</label>
                    <textarea
                        name="complaints"
                        value={patientCard.complaints || ""}
                        onChange={handleCardChange}
                        rows={4}
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                </div>

                <button
                    onClick={saveCard}
                    disabled={saving}
                    className={`px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors ${saving ? 'opacity-75 cursor-not-allowed' : ''
                        }`}
                >
                    {saving ? 'Сохранение данных...' : 'Сохранить все изменения'}
                </button>
            </div>

            {/* Health Conditions Section */}
            <div className="bg-white rounded-xl shadow-md p-6 mb-8 border border-gray-200">
                <h2 className="text-2xl font-semibold mb-4 text-green-700 flex items-center">
                    <span>Состояния здоровья</span>
                </h2>

                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">Добавить состояние здоровья</label>
                    <div className="relative">
                        <input
                            type="text"
                            value={healthCondInput}
                            onChange={handleHealthCondInputChange}
                            placeholder="Начните вводить название состояния..."
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
                        />

                        {healthCondInput && healthCondSuggestions.length === 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
                                <button
                                    onClick={() => addHealthCond("", true, healthCondInput)}
                                    disabled={addingHealthCond}
                                    className="block w-full text-left px-4 py-2 hover:bg-green-50 text-green-700 font-medium"
                                >
                                    Создать новый тип: "{healthCondInput}"
                                </button>
                            </div>
                        )}

                        {healthCondSuggestions.length > 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
                                {healthCondSuggestions.map((type) => (
                                    <button
                                        key={type.id}
                                        onClick={() => addHealthCond(type.id)}
                                        disabled={addingHealthCond}
                                        className="block w-full text-left px-4 py-2 hover:bg-gray-100 transition-colors"
                                    >
                                        {type.title}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </div>

                {loadingHealthConds ? (
                    <div className="flex justify-center py-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-green-500"></div>
                    </div>
                ) : healthConds.length === 0 ? (
                    <p className="text-gray-500 italic">Нет записей о состояниях здоровья</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {healthConds.map((cond) => (
                            <div
                                key={cond.id}
                                className="border border-green-200 bg-green-50 rounded-lg p-4 flex justify-between items-start"
                            >
                                <div>
                                    <h4 className="font-medium text-green-800">
                                        {getHealthCondTypeName(cond.typeId)}
                                    </h4>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* External Procedures Section */}
            <div className="bg-white rounded-xl shadow-md p-6 mb-8 border border-gray-200">
                <h2 className="text-2xl font-semibold mb-4 text-purple-700 flex items-center">
                    <span>Внешние процедуры</span>
                </h2>

                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">Добавить внешнюю процедуру</label>
                    <div className="relative">
                        <input
                            type="text"
                            value={externalProcedureInput}
                            onChange={handleExternalProcedureInputChange}
                            placeholder="Начните вводить название процедуры..."
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-purple-500"
                        />

                        {externalProcedureInput && externalProcedureSuggestions.length === 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
                                <button
                                    onClick={() => addExternalProcedure("", true, externalProcedureInput)}
                                    disabled={addingExternalProcedure}
                                    className="block w-full text-left px-4 py-2 hover:bg-purple-50 text-purple-700 font-medium"
                                >
                                    Создать новый тип: "{externalProcedureInput}"
                                </button>
                            </div>
                        )}

                        {externalProcedureSuggestions.length > 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
                                {externalProcedureSuggestions.map((type) => (
                                    <button
                                        key={type.id}
                                        onClick={() => addExternalProcedure(type.id)}
                                        disabled={addingExternalProcedure}
                                        className="block w-full text-left px-4 py-2 hover:bg-gray-100 transition-colors"
                                    >
                                        {type.title}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </div>

                {loadingExternalProcedures ? (
                    <div className="flex justify-center py-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-purple-500"></div>
                    </div>
                ) : externalProcedures.length === 0 ? (
                    <p className="text-gray-500 italic">Нет записей о внешних процедурах</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {externalProcedures.map((proc) => (
                            <div
                                key={proc.id}
                                className="border border-purple-200 bg-purple-50 rounded-lg p-4 flex justify-between items-start"
                            >
                                <div>
                                    <h4 className="font-medium text-purple-800">
                                        {getExternalProcedureTypeName(proc.typeId)}
                                    </h4>
                                    <p className="text-sm text-purple-600 mt-1">
                                        Дата: {new Date(proc.date).toLocaleDateString()}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Skin Cares Section */}
            <div className="bg-white rounded-xl shadow-md p-6 mb-8 border border-gray-200">
                <h2 className="text-2xl font-semibold mb-4 text-amber-700 flex items-center">
                    <span>Уход за кожей</span>
                </h2>

                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">Добавить уход за кожей</label>
                    <div className="relative">
                        <input
                            type="text"
                            value={skinCareInput}
                            onChange={handleSkinCareInputChange}
                            placeholder="Начните вводить название ухода..."
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500"
                        />

                        {skinCareInput && skinCareSuggestions.length === 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
                                <button
                                    onClick={() => addSkinCare("", true, skinCareInput)}
                                    disabled={addingSkinCare}
                                    className="block w-full text-left px-4 py-2 hover:bg-amber-50 text-amber-700 font-medium"
                                >
                                    Создать новый тип: "{skinCareInput}"
                                </button>
                            </div>
                        )}

                        {skinCareSuggestions.length > 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
                                {skinCareSuggestions.map((type) => (
                                    <button
                                        key={type.id}
                                        onClick={() => addSkinCare(type.id)}
                                        disabled={addingSkinCare}
                                        className="block w-full text-left px-4 py-2 hover:bg-gray-100 transition-colors"
                                    >
                                        {type.title}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </div>

                {loadingSkinCares ? (
                    <div className="flex justify-center py-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-amber-500"></div>
                    </div>
                ) : skinCares.length === 0 ? (
                    <p className="text-gray-500 italic">Нет записей об уходе за кожей</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {skinCares.map((care) => (
                            <div
                                key={care.id}
                                className="border border-amber-200 bg-amber-50 rounded-lg p-4 flex justify-between items-start"
                            >
                                <div>
                                    <h4 className="font-medium text-amber-800">
                                        {getSkinCareTypeName(care.typeId)}
                                    </h4>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Skin Features Section */}
            <div className="bg-white rounded-xl shadow-md p-6 mb-8 border border-gray-200">
                <h2 className="text-2xl font-semibold mb-4 text-cyan-700 flex items-center">
                    <span>Особенности кожи</span>
                </h2>

                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">Добавить особенность кожи</label>
                    <div className="relative">
                        <input
                            type="text"
                            value={skinFeatureInput}
                            onChange={handleSkinFeatureInputChange}
                            placeholder="Начните вводить название особенности..."
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-500"
                        />

                        {skinFeatureInput && skinFeatureSuggestions.length === 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg">
                                <button
                                    onClick={() => addSkinFeature("", true, skinFeatureInput)}
                                    disabled={addingSkinFeature}
                                    className="block w-full text-left px-4 py-2 hover:bg-cyan-50 text-cyan-700 font-medium"
                                >
                                    Создать новый тип: "{skinFeatureInput}"
                                </button>
                            </div>
                        )}

                        {skinFeatureSuggestions.length > 0 && (
                            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-y-auto">
                                {skinFeatureSuggestions.map((type) => (
                                    <button
                                        key={type.id}
                                        onClick={() => addSkinFeature(type.id)}
                                        disabled={addingSkinFeature}
                                        className="block w-full text-left px-4 py-2 hover:bg-gray-100 transition-colors"
                                    >
                                        {type.title}
                                    </button>
                                ))}
                            </div>
                        )}
                    </div>
                </div>

                {loadingSkinFeatures ? (
                    <div className="flex justify-center py-4">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-500"></div>
                    </div>
                ) : skinFeatures.length === 0 ? (
                    <p className="text-gray-500 italic">Нет записей об особенностях кожи</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {skinFeatures.map((feature) => (
                            <div
                                key={feature.id}
                                className="border border-cyan-200 bg-cyan-50 rounded-lg p-4 flex justify-between items-start"
                            >
                                <div>
                                    <h4 className="font-medium text-cyan-800">
                                        {getSkinFeatureTypeName(feature.typeId)}
                                    </h4>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default PatientDetailsPage;
