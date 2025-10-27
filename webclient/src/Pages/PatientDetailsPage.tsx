import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../api/api";
import type { PatientCard } from "../TypesFromServer/PatientCard"

const PatientDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [patientCard, setPatientCard] = useState<PatientCard>({} as PatientCard);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const [loadingPatient, setLoadingPatient] = useState(false);
    const [loadingCard, setLoadingCard] = useState(false);

    useEffect(() => {
        if (!id) return;

        const fetchPatientCard = async () => {
            try {
                setLoadingCard(true);
                const responsePC = await api.get(`/PatientCards/${id}`);
                setPatientCard(responsePC.data);
            } catch (err) {
                console.error("Ошибка загрузки карты пациента", err);
                alert("Не удалось загрузить данные карты пациента");
            } finally {
                setLoadingCard(false);
            }
        };
        fetchPatientCard();
    }, [id]);

    // Сохранить данные пациента
    const savePatient = async () => {
        try {
            setIsLoading(true);
            setSaving(true);
            await axios.put(`https://localhost:7135/api/Patients/Update`, {
                id: patientCard.patientId,
                fullname: patientCard.fullname,
            }, { withCredentials: true });
            alert(patientCard.fullname);
            alert(patientCard.patientId);
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

    const handleCardChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatientCard(prev => ({
            ...prev,
            id: patientCard.id,
            [name]: name === "age" ? Number(value) : value,
        }));
    };


    if (loadingCard && loadingPatient && isLoading) return <div>Загрузка...</div>;
    if (error) return <div style={{ color: "red" }}>{error}</div>;
    if (!patientCard) return <div>Пациент не найден</div>;

    return (
        <div style={{ margin: "2rem" }}>
            <h2>Профиль пациента</h2>

            <div style={{ marginBottom: "1rem" }}>
                <label>ФИО:</label>
                <input
                    type="text"
                    value={patientCard.fullname}
                    name="fullname"
                    onChange={handleCardChange}
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
