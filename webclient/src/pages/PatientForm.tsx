import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";

type Patient = {
    id?: string;
    fullName: string;
    phone?: string;
    email?: string;
};

const PatientForm: React.FC = () => {
    const { id } = useParams(); // если id есть → редактирование
    const navigate = useNavigate();

    const [form, setForm] = useState<Patient>({
        fullName: "",
        phone: "",
        email: "",
    });

    const [loading, setLoading] = useState(false);

    // Загружаем данные, если редактируем
    useEffect(() => {
        if (!id) return;

        const fetchPatient = async () => {
            try {
                setLoading(true);
                const response = await axios.get(`https://localhost:7135/api/Patients/${id}`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("authToken")}`,
                    },
                        withCredentials: true
                });
                setForm(response.data);
            } catch (err) {
                console.error("Ошибка загрузки пациента", err);
                alert("Не удалось загрузить данные пациента");
            } finally {
                setLoading(false);
            }
        };

        fetchPatient();
    }, [id]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({
            ...form,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        try {
            if (id) {
                // обновление
                await axios.put(
                    "https://localhost:7135/api/Patients/Update",
                    form,
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
                        },
                        withCredentials: true
                    }
                );
                alert("Пациент обновлён");
            } else {
                // создание
                await axios.post(
                    "https://localhost:7135/api/Patients",
                    form,
                    {
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
                        },
                        withCredentials: true
                    }
                );
                alert("Пациент создан");
            }

            navigate("/patients");
        } catch (err) {
            console.error("Ошибка сохранения пациента", err);
            alert("Не удалось сохранить данные");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ padding: "2rem" }}>
            <h1>{id ? "Редактировать пациента" : "Создать пациента"}</h1>

            <form
                onSubmit={handleSubmit}
                style={{ display: "flex", flexDirection: "column", width: "300px", gap: "10px" }}
            >
                <input
                    type="text"
                    name="fullName"
                    placeholder="ФИО"
                    value={form.fullName}
                    onChange={handleChange}
                    required
                />

                <input
                    type="tel"
                    name="phone"
                    placeholder="Телефон"
                    value={form.phone}
                    onChange={handleChange}
                />

                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={form.email}
                    onChange={handleChange}
                />

                <button
                    type="submit"
                    disabled={loading}
                    style={{
                        padding: "8px",
                        backgroundColor: "#0275d8",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                        cursor: loading ? "wait" : "pointer",
                    }}
                >
                    {loading ? "Сохранение..." : "Сохранить"}
                </button>
            </form>
        </div>
    );
};

export default PatientForm;
