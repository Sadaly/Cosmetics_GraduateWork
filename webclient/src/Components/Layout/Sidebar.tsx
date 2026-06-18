import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../Context/AuthContext';

const navigation = [
    { name: 'Главная',       href: '/home',      icon: '⌂'  },
    { name: 'Расписание',    href: '/schedule',  icon: '◷'  },
    { name: 'Пациенты',      href: '/patients',  icon: '♥'  },
    { name: 'Врачи',         href: '/doctors',   icon: '🩺' },
    { name: 'Аналитика',     href: '/analitics', icon: '◈'  },
    { name: 'Справочники',   href: '/reference', icon: '📚' },
    { name: 'Пользователи',  href: '/users',     icon: '⚙'  },
];

interface SidebarProps {
    mobileOpen?: boolean;
    onMobileClose?: () => void;
}

export const Sidebar: React.FC<SidebarProps> = ({ mobileOpen = false, onMobileClose }) => {
    const location = useLocation();
    const navigate = useNavigate();
    const { logout, user } = useAuth();
    const [isCollapsed, setIsCollapsed] = useState(false);

    const handleLogout = () => { logout(); navigate('/'); };

    const initial = (user?.email ?? 'U')[0].toUpperCase();

    const SidebarContent = (
        <aside style={{
            display: 'flex', flexDirection: 'column', height: '100%',
            background: 'var(--sidebar-bg)',
            width: isCollapsed ? 'var(--sidebar-collapsed)' : 'var(--sidebar-width)',
            transition: 'width var(--transition-normal)',
            overflow: 'hidden',
        }}>
            {/* Logo */}
            <div style={{
                display: 'flex', alignItems: 'center',
                justifyContent: isCollapsed ? 'center' : 'space-between',
                height: 64, padding: isCollapsed ? '0 16px' : '0 20px',
                borderBottom: '1px solid rgba(255,255,255,.07)', flexShrink: 0,
            }}>
                {!isCollapsed && (
                    <div>
                        <span style={{
                            fontFamily: "'Cormorant Garamond', Georgia, serif",
                            fontSize: '1.25rem', fontWeight: 600, color: '#F9F0F5', letterSpacing: '-.01em',
                        }}>Cosmetics</span>
                        <div style={{
                            width: 32, height: 2,
                            background: 'linear-gradient(90deg, var(--color-accent), transparent)',
                            marginTop: 4, borderRadius: 2,
                        }} />
                    </div>
                )}
                <button
                    onClick={() => { setIsCollapsed(c => !c); onMobileClose?.(); }}
                    style={{
                        width: 32, height: 32, borderRadius: 8,
                        background: 'rgba(255,255,255,.06)', border: 'none',
                        cursor: 'pointer', display: 'flex', alignItems: 'center', justifyContent: 'center',
                        color: 'rgba(255,255,255,.6)', transition: 'background var(--transition-fast)', flexShrink: 0,
                        fontSize: '1.1rem',
                    }}
                    onMouseEnter={e => (e.currentTarget.style.background = 'rgba(255,255,255,.12)')}
                    onMouseLeave={e => (e.currentTarget.style.background = 'rgba(255,255,255,.06)')}
                >
                    {isCollapsed ? '›' : '‹'}
                </button>
            </div>

            {/* Nav */}
            <nav style={{ flex: 1, padding: '12px 10px', overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: 4 }}>
                {navigation.map(item => {
                    const isActive = location.pathname === item.href ||
                        (item.href !== '/home' && location.pathname.startsWith(item.href));
                    return (
                        <Link
                            key={item.href}
                            to={item.href}
                            onClick={onMobileClose}
                            title={isCollapsed ? item.name : undefined}
                            style={{
                                display: 'flex', alignItems: 'center', gap: 12,
                                padding: isCollapsed ? '10px 0' : '10px 14px',
                                justifyContent: isCollapsed ? 'center' : 'flex-start',
                                borderRadius: 10, textDecoration: 'none',
                                background: isActive ? 'var(--sidebar-active)' : 'transparent',
                                transition: 'background var(--transition-fast)', position: 'relative',
                            }}
                            onMouseEnter={e => { if (!isActive) e.currentTarget.style.background = 'var(--sidebar-hover)'; }}
                            onMouseLeave={e => { if (!isActive) e.currentTarget.style.background = 'transparent'; }}
                        >
                            {isActive && (
                                <span style={{
                                    position: 'absolute', left: 0, top: '50%', transform: 'translateY(-50%)',
                                    width: 3, height: 20, borderRadius: '0 2px 2px 0',
                                    background: 'var(--color-accent)',
                                }} />
                            )}
                            <span style={{
                                fontSize: '1.125rem',
                                color: isActive ? 'var(--color-accent)' : 'rgba(255,255,255,.55)',
                                flexShrink: 0,
                            }}>{item.icon}</span>
                            {!isCollapsed && (
                                <span style={{
                                    fontSize: '.875rem',
                                    fontWeight: isActive ? 700 : 500,
                                    color: isActive ? '#F9F0F5' : 'rgba(255,255,255,.65)',
                                    whiteSpace: 'nowrap',
                                }}>{item.name}</span>
                            )}
                        </Link>
                    );
                })}
            </nav>

            {/* User + logout */}
            <div style={{ padding: '12px 10px', borderTop: '1px solid rgba(255,255,255,.07)', flexShrink: 0 }}>
                {!isCollapsed && (
                    <div style={{
                        display: 'flex', alignItems: 'center', gap: 10,
                        padding: '10px 14px', marginBottom: 8,
                        background: 'rgba(255,255,255,.04)', borderRadius: 10,
                    }}>
                        <div style={{
                            width: 34, height: 34, borderRadius: '50%',
                            background: 'linear-gradient(135deg, var(--color-primary), var(--color-accent))',
                            display: 'flex', alignItems: 'center', justifyContent: 'center',
                            color: '#fff', fontWeight: 700, fontSize: '.875rem', flexShrink: 0,
                        }}>{initial}</div>
                        <div style={{ overflow: 'hidden', flex: 1 }}>
                            <p style={{ fontSize: '.8125rem', fontWeight: 600, color: 'rgba(255,255,255,.85)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                {user?.fullname ?? user?.email ?? 'Пользователь'}
                            </p>
                            <p style={{ fontSize: '.6875rem', color: 'rgba(255,255,255,.4)', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                                {user?.email}
                            </p>
                        </div>
                    </div>
                )}
                <button
                    onClick={handleLogout}
                    title={isCollapsed ? 'Выйти' : undefined}
                    style={{
                        width: '100%', display: 'flex', alignItems: 'center',
                        justifyContent: isCollapsed ? 'center' : 'flex-start',
                        gap: 8, padding: '8px 14px',
                        background: 'rgba(184,64,84,.12)', border: 'none', borderRadius: 10,
                        cursor: 'pointer', color: '#F87A8A', fontSize: '.8125rem', fontWeight: 600,
                        transition: 'background var(--transition-fast)',
                    }}
                    onMouseEnter={e => (e.currentTarget.style.background = 'rgba(184,64,84,.22)')}
                    onMouseLeave={e => (e.currentTarget.style.background = 'rgba(184,64,84,.12)')}
                >
                    <span>⎋</span>
                    {!isCollapsed && <span>Выйти</span>}
                </button>
            </div>
        </aside>
    );

    return (
        <>
            <div className="hidden lg:flex flex-col h-screen sticky top-0">{SidebarContent}</div>
            {mobileOpen && (
                <div
                    className="fixed inset-0 z-40 lg:hidden flex"
                    style={{ backdropFilter: 'blur(2px)', background: 'rgba(44,26,36,.5)' }}
                    onClick={onMobileClose}
                >
                    <div className="animate-fade-in" onClick={e => e.stopPropagation()}>
                        {SidebarContent}
                    </div>
                </div>
            )}
        </>
    );
};
