import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from 'axios';
import type { Patient } from "../TypesFromServer/Patient"
import type { PatientCard } from "../TypesFromServer/PatientCard"

const PatientDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [patient, setPatient] = useState<Patient>({} as Patient);
    const [patientCard, setPatientCard] = useState<PatientCard>({} as PatientCard);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const [loadingPatient, setLoadingPatient] = useState(false);
    const [loadingCard, setLoadingCard] = useState(false);

    useEffect(() => {
        if (!id) return;

        const fetchPatient = async () => {
            try {
                setLoadingPatient(true);
                const res = await axios.get(`https://localhost:7135/api/Patients/${id}`, { withCredentials: true });
                setPatient(res.data);
                return res.data; // Возвращаем данные пациента
            } catch (err) {
                console.error("Ошибка загрузки пациента", err);
                alert("Не удалось загрузить данные пациента");
                return null;
            } finally {
                setLoadingPatient(false);
            }
        };

        const fetchPatientCard = async (cardId: string) => {
            try {
                setLoadingCard(true);
                const responsePC = await axios.get(`https://localhost:7135/api/PatientCards/${cardId}`, { withCredentials: true });
                setPatientCard(responsePC.data);
            } catch (err) {
                console.error("Ошибка загрузки карты пациента", err);
                alert("Не удалось загрузить данные карты пациента");
            } finally {
                setLoadingCard(false);
            }
        };

        // Сначала загружаем пациента, затем его карту
        const loadData = async () => {
            const patientData = await fetchPatient();
            if (patientData && patientData.cardId) {
                await fetchPatientCard(patientData.cardId);
            }
        };

        loadData();
    }, [id]);

    // Сохранить данные пациента
    const savePatient = async () => {
        try {
            setIsLoading(true);
            setSaving(true);
            await axios.put("https://localhost:7135/api/Patients/Update", {
                id: patient.patientId,
                fullname: patient.fullname,
            }, { withCredentials: true });
            alert("Данные пациента обновлены");
        } catch (err) {
            console.error(err);
            alert("Не удалось сохранить данные пациента");
        } finally {
            setIsLoading(false);
            setSaving(false);
        }
    };

    // Сохранить данные карточки
    const saveCard = async () => {
        try {
            setIsLoading(true);
            setSaving(true);
            await axios.put("https://localhost:7135/api/PatientCards", patientCard, { withCredentials: true });
            alert("Карточка пациента обновлена");
        } catch (err) {
            console.error(err);
            alert("Не удалось сохранить карточку");
        } finally {
            setIsLoading(false);
            setSaving(false);
        }
    };


    const handlePatientChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatient(prev => ({ ...prev, [name]: value }));
    };

    const handleCardChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatientCard(prev => ({
            ...prev,
            id: patient.cardId,
            [name]: name === "age" ? Number(value) : value,
        }));
    };


    if (loadingCard && loadingPatient && isLoading) return <div>Загрузка...</div>;
    if (error) return <div style={{ color: "red" }}>{error}</div>;
    if (!patient) return <div>Пациент не найден</div>;

    return (
        <div style={{ margin: "2rem" }}>
            <h2>Профиль пациента</h2>

            <div style={{ marginBottom: "1rem" }}>
                <label>ФИО:</label>
                <input
                    type="text"
                    value={patient.fullname}
                    name="fullname"
                    onChange={handlePatientChange}
                    style={{ marginLeft: "1rem", width: "300px" }}
                />
                <button onClick={savePatient}>
                    {saving ? "Сохранение..." : "Сохранить ФИО"}
                </button>
            </div>

            <h3>Карта пациента</h3>
            <div
                style={{
                    display: "flex",
                    flexDirection: "column",
                    gap: "10px",
                    width: "400px",
                }}
            >
                <label>
                    Возраст:
                    <input
                        type="number"
                        name="age"
                        value={patientCard.age}
                        onChange={handleCardChange}
                    />
                </label>

                <label>
                    Адрес:
                    <input
                        type="text"
                        name="address"
                        value={patientCard.address}
                        onChange={handleCardChange}
                    />
                </label>

                <label>
                    Жалобы:
                    <textarea
                        rows={6}
                        value={patientCard.complaints}
                        name="complaints"
                        onChange={handleCardChange }
                    />
                </label>

                <label>
                    Телефон:
                    <input
                        type="text"
                        name="phoneNumber"
                        value={patientCard.phoneNumber}
                        onChange={ handleCardChange }
                    />
                </label>

                <button onClick={saveCard} disabled={saving}>
                    {saving ? "Сохранение..." : "Сохранить карту"}
                </button>
            </div>
        </div>
    );
};

export default PatientDetailsPage;
