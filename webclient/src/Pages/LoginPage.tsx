import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../Context/AuthContext';
import { Button } from '../Components/UI/Button';
import { Input } from '../Components/UI/Input';
import { Spinner } from '../Components/UI/Spinner';
import { useToast } from '../Hooks/useToast';

const LoginPage: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const { login, isAuthenticated, isLoading: authLoading } = useAuth();
    const { showError } = useToast();
    const navigate = useNavigate();

    useEffect(() => {
        if (isAuthenticated && !authLoading) navigate('/home');
    }, [isAuthenticated, authLoading, navigate]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!email.trim() || !password.trim()) {
            setError('Заполните все поля');
            return;
        }
        setIsLoading(true);
        setError('');
        try {
            await login(email, password);
            navigate('/home');
        } catch (err: any) {
            const data = err.response?.data;
            const msg = data?.detail || data?.message || 'Неверный логин или пароль. Попробуйте снова.';
            setError(msg);
            showError(msg);
        } finally {
            setIsLoading(false);
        }
    };

    if (authLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center" style={{ background: 'var(--bg-base)' }}>
                <Spinner size="lg" />
            </div>
        );
    }

    return (
        <div className="min-h-screen flex" style={{ background: 'var(--bg-base)' }}>
            {/* Left decorative panel (hidden on mobile) */}
            <div className="hidden lg:flex flex-col justify-between p-12"
                style={{
                    width: 420,
                    flexShrink: 0,
                    background: 'var(--sidebar-bg)',
                    position: 'relative',
                    overflow: 'hidden',
                }}>
                {/* Decorative circles */}
                <div style={{
                    position: 'absolute', top: -80, right: -80,
                    width: 300, height: 300, borderRadius: '50%',
                    background: 'rgba(200,134,90,.08)',
                }} />
                <div style={{
                    position: 'absolute', bottom: -60, left: -60,
                    width: 250, height: 250, borderRadius: '50%',
                    background: 'rgba(123,63,98,.15)',
                }} />

                {/* Brand */}
                <div>
                    <span style={{
                        fontFamily: "'Cormorant Garamond', Georgia, serif",
                        fontSize: '2rem', fontWeight: 600, color: '#F9F0F5',
                        letterSpacing: '-.01em',
                    }}>
                        Cosmetics
                    </span>
                    <div style={{
                        width: 40, height: 2,
                        background: 'var(--color-accent)',
                        borderRadius: 2, marginTop: 8,
                    }} />
                </div>

                {/* Tagline */}
                <div>
                    <p style={{
                        fontFamily: "'Cormorant Garamond', Georgia, serif",
                        fontSize: '1.75rem', fontWeight: 400,
                        color: 'rgba(255,255,255,.75)', lineHeight: 1.4,
                        fontStyle: 'italic',
                    }}>
                        «Красота — это наука.
                        <br />Забота — это искусство.»
                    </p>
                    <p style={{ marginTop: 16, color: 'rgba(255,255,255,.35)', fontSize: '.875rem' }}>
                        Система управления клиникой косметологии
                    </p>
                </div>

                {/* Footer */}
                <p style={{ color: 'rgba(255,255,255,.2)', fontSize: '.75rem' }}>
                    © 2025 Cosmetics Clinic
                </p>
            </div>

            {/* Right — login form */}
            <div className="flex-1 flex items-center justify-center p-6">
                <div className="w-full animate-fade-in" style={{ maxWidth: 420 }}>
                    {/* Mobile brand */}
                    <div className="lg:hidden text-center mb-8">
                        <span style={{
                            fontFamily: "'Cormorant Garamond', Georgia, serif",
                            fontSize: '2rem', fontWeight: 600, color: 'var(--color-primary)',
                        }}>
                            Cosmetics
                        </span>
                    </div>

                    <div style={{
                        background: 'var(--bg-surface)',
                        borderRadius: 'var(--radius-xl)',
                        border: '1px solid var(--border-subtle)',
                        boxShadow: 'var(--shadow-lg)',
                        padding: '2.5rem',
                    }}>
                        <div style={{ marginBottom: '2rem' }}>
                            <h2 style={{
                                fontFamily: "'Cormorant Garamond', Georgia, serif",
                                fontSize: '1.75rem', fontWeight: 600,
                                color: 'var(--text-primary)',
                            }}>
                                Вход в систему
                            </h2>
                            <p style={{ marginTop: '.5rem', fontSize: '.875rem', color: 'var(--text-muted)' }}>
                                Введите данные вашего аккаунта
                            </p>
                        </div>

                        <form onSubmit={handleSubmit} className="space-y-4">
                            <Input
                                type="email"
                                label="Email"
                                placeholder="doctor@clinic.com"
                                value={email}
                                onChange={e => setEmail(e.target.value)}
                                required
                                autoComplete="email"
                                icon={
                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                                        <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/>
                                        <polyline points="22,6 12,13 2,6"/>
                                    </svg>
                                }
                            />

                            <div className="form-group">
                                <label className="form-label">Пароль</label>
                                <div className="relative">
                                    <input
                                        type={showPassword ? 'text' : 'password'}
                                        placeholder="••••••••"
                                        value={password}
                                        onChange={e => setPassword(e.target.value)}
                                        required
                                        autoComplete="current-password"
                                        className="form-input pr-10"
                                    />
                                    <button
                                        type="button"
                                        onClick={() => setShowPassword(s => !s)}
                                        style={{
                                            position: 'absolute', right: 12, top: '50%',
                                            transform: 'translateY(-50%)',
                                            background: 'none', border: 'none', cursor: 'pointer',
                                            color: 'var(--text-muted)', padding: 0, fontSize: '1rem',
                                        }}
                                        title={showPassword ? 'Скрыть' : 'Показать'}
                                    >
                                        {showPassword ? '○' : '●'}
                                    </button>
                                </div>
                            </div>

                            {error && (
                                <div style={{
                                    padding: '0.75rem 1rem',
                                    borderRadius: 'var(--radius-md)',
                                    background: 'var(--color-danger-light)',
                                    border: '1px solid rgba(184,64,84,.2)',
                                    fontSize: '.875rem',
                                    color: 'var(--color-danger)',
                                    fontWeight: 500,
                                }}>
                                    {error}
                                </div>
                            )}

                            <Button
                                type="submit"
                                variant="primary"
                                isLoading={isLoading}
                                className="w-full"
                                style={{ marginTop: '0.5rem', padding: '0.75rem' }}
                            >
                                {isLoading ? 'Вход...' : 'Войти'}
                            </Button>
                        </form>

                        {/* Demo credentials */}
                        <div style={{
                            marginTop: '1.5rem', padding: '1rem',
                            background: 'var(--bg-muted)',
                            borderRadius: 'var(--radius-md)',
                            border: '1px dashed var(--border-default)',
                        }}>
                            <p style={{ fontSize: '.75rem', fontWeight: 700, color: 'var(--text-muted)', marginBottom: '.5rem', textTransform: 'uppercase', letterSpacing: '.06em' }}>
                                Демо доступ
                            </p>
                            <p style={{ fontSize: '.8125rem', color: 'var(--text-secondary)' }}>
                                <strong>Email:</strong> demo@cosmetics.com
                            </p>
                            <p style={{ fontSize: '.8125rem', color: 'var(--text-secondary)' }}>
                                <strong>Пароль:</strong> Demo123!
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default LoginPage;
