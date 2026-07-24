import React, { useEffect } from 'react';

interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    title?: string;
    children: React.ReactNode;
    footer?: React.ReactNode;
    size?: 'sm' | 'md' | 'lg' | 'xl';
}

const sizeMap: Record<string, string> = {
    sm: '420px',
    md: '520px',
    lg: '640px',
    xl: '780px',
};

export const Modal: React.FC<ModalProps> = ({
    isOpen,
    onClose,
    title,
    children,
    footer,
    size = 'md',
}) => {
    // Close on Escape key
    useEffect(() => {
        if (!isOpen) return;
        const handler = (e: KeyboardEvent) => { if (e.key === 'Escape') onClose(); };
        document.addEventListener('keydown', handler);
        // Prevent body scroll when modal is open
        document.body.style.overflow = 'hidden';
        return () => {
            document.removeEventListener('keydown', handler);
            document.body.style.overflow = '';
        };
    }, [isOpen, onClose]);

    if (!isOpen) return null;

    return (
        <div
            className="fixed inset-0 z-[9999] flex items-center justify-center px-4"
            role="dialog"
            aria-modal="true"
        >
            {/* Backdrop — properly dark */}
            <div
                className="fixed inset-0"
                style={{ background: 'rgba(44,26,36,.6)', backdropFilter: 'blur(4px)' }}
                onClick={onClose}
            />

            {/* Panel */}
            <div
                className="relative w-full animate-scale-in"
                style={{
                    maxWidth: sizeMap[size],
                    background: 'var(--bg-surface)',
                    borderRadius: 'var(--radius-xl)',
                    border: '1px solid var(--border-subtle)',
                    boxShadow: 'var(--shadow-xl)',
                    maxHeight: '90vh',
                    display: 'flex',
                    flexDirection: 'column',
                }}
            >
                {/* Header */}
                {title && (
                    <div style={{
                        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                        padding: '1rem 1.5rem', flexShrink: 0,
                        borderBottom: '1px solid var(--border-subtle)',
                    }}>
                        <h3 style={{
                            fontFamily: "'Cormorant Garamond', Georgia, serif",
                            fontSize: '1.25rem',
                            fontWeight: 600,
                            color: 'var(--text-primary)',
                        }}>
                            {title}
                        </h3>
                        <button
                            onClick={onClose}
                            aria-label="Закрыть"
                            style={{
                                width: 32, height: 32,
                                borderRadius: 'var(--radius-full)',
                                background: 'var(--bg-muted)',
                                border: 'none',
                                cursor: 'pointer',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                color: 'var(--text-secondary)',
                                transition: 'background var(--transition-fast)',
                                padding: 0,
                            }}
                            onMouseEnter={e => (e.currentTarget.style.background = 'var(--color-primary-light)')}
                            onMouseLeave={e => (e.currentTarget.style.background = 'var(--bg-muted)')}
                        >
                            <svg width="14" height="14" viewBox="0 0 14 14" fill="none">
                                <path d="M1 1l12 12M13 1L1 13" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
                            </svg>
                        </button>
                    </div>
                )}

                {/* Body */}
                <div style={{ padding: '1.25rem 1.5rem', overflowY: 'auto', flex: '1 1 0%', minHeight: 0 }}>
                    {children}
                </div>

                {/* Footer */}
                {footer && (
                    <div style={{
                        display: 'flex', justifyContent: 'flex-end', gap: '.75rem',
                        padding: '1rem 1.5rem', flexShrink: 0,
                        borderTop: '1px solid var(--border-subtle)',
                        background: 'var(--bg-muted)',
                        borderRadius: '0 0 var(--radius-xl) var(--radius-xl)',
                    }}>
                        {footer}
                    </div>
                )}
            </div>
        </div>
    );
};
