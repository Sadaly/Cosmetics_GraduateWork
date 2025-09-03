import React, { useState, useEffect } from 'react';
import axios from 'axios';
import type { ErrorResponse } from "../TypesFromServer/ErrorResponse";
import { Link, useNavigate } from 'react-router-dom';

const LoginPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorDetail, setErrorDetail] = useState('');
    const [errorResponse, setErrorResponse] = useState<ErrorResponse[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [dots, setDots] = useState(3);
    const navigate = useNavigate();

    // Анимация точек во время загрузки
    useEffect(() => {
        if (!isLoading) return;

        const interval = setInterval(() => {
            setDots(prev => prev > 1 ? prev - 1 : 3);
        }, 500);

        return () => clearInterval(interval);
    }, [isLoading]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setErrorDetail('');
        setErrorResponse([]);

        const command = { email, password };

        try {
            const response = await axios.post('https://localhost:7135/api/Users/Login', command, {
                headers: {
                    'Content-Type': 'application/json',
                },
                withCredentials: true,
            });

            console.log('User logged in successfully:', response.data.message);
            navigate('/home');
        } catch (error: any) {
            const responseData = error.response?.data;
            setErrorDetail(responseData?.detail || 'Произошла ошибка входа');

            if (Array.isArray(responseData?.errors)) {
                setErrorResponse(responseData.errors);
            }
            console.error('Login failed', responseData);
        } finally {
            setIsLoading(false);
        }
    };

    const renderButtonText = () => {
        if (!isLoading) return 'Войти';

        const dotsText = '.'.repeat(dots);
        return `Загрузка${dotsText}`;
    };

    return (
        <div style={{
            marginLeft: "40vw",
            marginTop: "20vh",
            alignContent: 'center',
            padding: "2rem",
            boxSizing: "border-box"
        }}>
            <h1>Вход</h1>
            <form
                onSubmit={handleSubmit}
                style={{ display: "flex", flexDirection: "column", width: "300px", gap: "10px" }}
            >
                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Пароль"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button
                    type="submit"
                    disabled={isLoading}
                    style={{
                        padding: "10px",
                        color: "white",
                        borderRadius: "5px",
                        cursor: isLoading ? "wait" : "pointer",
                    }}
                >
                    {renderButtonText()}
                </button>

                <p style={{ marginTop: "20px" }}>
                    Еще нет аккаутна? <Link to="/register">Создать</Link>
                </p>
            </form>

            {/* Общая ошибка */}
            {errorDetail && (
                <div style={{ color: 'red', marginTop: '10px' }}>
                    {errorDetail}
                </div>
            )}

            {/* Ошибки валидации */}
            {errorResponse.length > 0 && (
                <ul style={{ color: 'red', marginTop: '10px' }}>
                    {errorResponse.map((err, idx) => (
                        <li key={idx}>{err.message}</li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default LoginPage;