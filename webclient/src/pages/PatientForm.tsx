import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import type { Patient } from "../TypesFromServer/Patient";
import type { PatientCard } from "../TypesFromServer/PatientCard";

const PatientProfilePage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [patient, setPatient] = useState<Patient>({} as Patient);
    const [patientId, setPatientId] = useState("");
    const [card, setCard] = useState<PatientCard>({} as PatientCard);
    const [loadingPatient, setLoadingPatient] = useState(false);
    const [loadingCard, setLoadingCard] = useState(false);

    // Загружаем данные пациента
    useEffect(() => {
        if (!id) return;

        const fetchPatient = async () => {
            try {
                setLoadingPatient(true);
                const res = await axios.get(`https://localhost:7135/api/Patients/${id}`, { withCredentials: true });
                setPatient(res.data);
            } catch (err) {
                console.error("Ошибка загрузки пациента", err);
                alert("Не удалось загрузить данные пациента");
            } finally {
                setLoadingPatient(false);
            }
        };
        const fetchPatientCard = async () => {
            try {
                const responsePC = await axios.get(`https://localhost:7135/api/PatientCards/${patient.cardId}`, { withCredentials: true });
                setCard(responsePC.data);
            } catch (err) {
                console.error("Ошибка загрузки пациента", err);
                alert("Не удалось загрузить данные пациента");
            } finally {
                setLoadingPatient(false);
            }
        };

        fetchPatient();


    }, [id]);
    
    const handlePatientChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setPatient(prev => ({ ...prev, [name]: value }));
    };

    const handleCardChange = (e: React.ChangeEvent<HTMLInputElement> | React.ChangeEvent<HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setCard(prev => ({
            ...prev,
            id: patient.cardId,
            [name]: name === "age" ? Number(value) : value,
        }));
    };


    // Сохранить данные пациента
    const savePatient = async () => {
        try {
            setLoadingPatient(true);
            const patientId = setPatientId( await axios.post("https://localhost:7135/api/Patients", {
                fullName: patient.fullname,
            }, { withCredentials: true }));
            const res = await axios.get(`https://localhost:7135/api/Patients/${id}`, { withCredentials: true });
            setPatient(res.data);

            const responsePC = await axios.get(`https://localhost:7135/api/PatientCards/${patient.cardId}`, { withCredentials: true });
            setCard(responsePC.data);
            alert("Данные пациента обновлены");
        } catch (err) {
            console.error(err);
            alert("Не удалось сохранить данные пациента");
        } finally {
            setLoadingPatient(false);
        }
    };

    // Сохранить данные карточки
    const saveCard = async () => {
        try {
            setLoadingCard(true);
            await axios.put("https://localhost:7135/api/PatientCards", card, { withCredentials: true });
            alert("Карточка пациента обновлена");
        } catch (err) {
            console.error(err);
            alert("Не удалось сохранить карточку");
        } finally {
            setLoadingCard(false);
        }
    };

    const saveData = async () => {
        
    }

    return (
        <div style={{ padding: "2rem" }}>
            <h1>Профиль пациента</h1>

            <section style={{ marginBottom: "2rem" }}>
                <h2>Информация о пациенте</h2>
                <input
                    type="text"
                    name="fullname"
                    placeholder="ФИО"
                    value={patient.fullname}
                    onChange={handlePatientChange}
                    style={{ display: "block", marginBottom: "10px", width: "300px" }}
                />
                <button onClick={savePatient} disabled={loadingPatient}>
                    {loadingPatient ? "Сохранение..." : "Сохранить пациента"}
                </button>
            </section>

            <section>
                <h2>Карточка пациента</h2>
                <input
                    type="number"
                    name="age"
                    placeholder="Возраст"
                    value={card.age || 0}
                    onChange={handleCardChange}
                    style={{ display: "block", marginBottom: "10px", width: "300px" }}
                />
                <input
                    name="address"
                    placeholder="Адрес"
                    value={card.address || ""}
                    onChange={handleCardChange}
                    style={{ display: "block", marginBottom: "10px", width: "300px" }}
                />
                <textarea
                    name="complaints"
                    placeholder="Жалобы"
                    value={card.complaints || ""}
                    rows={6}
                    onChange={handleCardChange}
                    style={{ display: "block", marginBottom: "10px", width: "300px" }}
                />
                <input
                    name="phoneNumber"
                    placeholder="Телефон карточки"
                    value={card.phoneNumber || ""}
                    onChange={handleCardChange}
                    style={{ display: "block", marginBottom: "10px", width: "300px" }}
                />
                <button onClick={saveCard} disabled={loadingCard}>
                    {loadingCard ? "Сохранение..." : "Сохранить карточку"}
                </button>
            </section>
        </div>
    );
};

export default PatientProfilePage;