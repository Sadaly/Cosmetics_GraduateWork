import React, { useEffect, useState } from 'react';

interface ToastProps {
    message: string;
    type?: 'success' | 'error' | 'warning' | 'info';
    onClose: () => void;
    duration?: number;
}

const config = {
    success: { bg: 'var(--color-success)',  icon: '✓', light: 'var(--color-success-light)' },
    error:   { bg: 'var(--color-danger)',   icon: '✕', light: 'var(--color-danger-light)' },
    warning: { bg: 'var(--color-warning)',  icon: '!', light: 'var(--color-warning-light)' },
    info:    { bg: 'var(--color-info)',     icon: 'i', light: 'var(--color-info-light)' },
};

export const Toast: React.FC<ToastProps> = ({ message, type = 'info', onClose, duration = 5000 }) => {
    const [visible, setVisible] = useState(false);
    const { bg, icon } = config[type];

    useEffect(() => {
        // Trigger entrance animation
        const show = requestAnimationFrame(() => setVisible(true));
        const hide = setTimeout(() => {
            setVisible(false);
            setTimeout(onClose, 350);
        }, duration);
        return () => {
            cancelAnimationFrame(show);
            clearTimeout(hide);
        };
    }, [duration, onClose]);

    return (
        <div
            style={{
                display: 'flex',
                alignItems: 'flex-start',
                gap: '0.75rem',
                padding: '0.875rem 1rem',
                background: 'var(--bg-surface)',
                border: '1px solid var(--border-subtle)',
                borderLeft: `4px solid ${bg}`,
                borderRadius: 'var(--radius-lg)',
                boxShadow: 'var(--shadow-lg)',
                maxWidth: '400px',
                minWidth: '280px',
                transition: 'opacity 350ms ease, transform 350ms cubic-bezier(.4,0,.2,1)',
                opacity: visible ? 1 : 0,
                transform: visible ? 'translateX(0)' : 'translateX(24px)',
            }}
        >
            {/* Icon dot */}
            <div style={{
                width: 28, height: 28, borderRadius: '50%',
                background: bg,
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                color: '#fff', fontWeight: 700, fontSize: '0.875rem',
                shrink: 0,
            }}>
                {icon}
            </div>
            <p style={{
                flex: 1,
                fontSize: '0.875rem',
                fontWeight: 500,
                color: 'var(--text-primary)',
                lineHeight: 1.5,
            }}>
                {message}
            </p>
            <button
                onClick={() => { setVisible(false); setTimeout(onClose, 350); }}
                style={{
                    background: 'none', border: 'none', cursor: 'pointer',
                    color: 'var(--text-muted)', padding: '2px',
                    fontSize: '1rem', lineHeight: 1,
                    transition: 'color var(--transition-fast)',
                }}
                onMouseEnter={e => (e.currentTarget.style.color = 'var(--text-primary)')}
                onMouseLeave={e => (e.currentTarget.style.color = 'var(--text-muted)')}
            >
                ✕
            </button>
        </div>
    );
};
