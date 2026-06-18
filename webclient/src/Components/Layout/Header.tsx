import React from 'react';

interface HeaderProps {
    title?: string;
    subtitle?: string;
    actions?: React.ReactNode;
}

export const Header: React.FC<HeaderProps> = ({ title, subtitle, actions }) => (
    <header className="animate-fade-in" style={{
        background: 'var(--bg-surface)',
        borderRadius: 'var(--radius-lg)',
        border: '1px solid var(--border-subtle)',
        boxShadow: 'var(--shadow-sm)',
        padding: '1.25rem 1.5rem',
    }}>
        <div className="flex items-center justify-between flex-wrap gap-4">
            <div>
                {title && (
                    <h1 style={{
                        fontFamily: "'Cormorant Garamond', Georgia, serif",
                        fontSize: 'clamp(1.375rem, 3vw, 1.75rem)',
                        fontWeight: 600,
                        color: 'var(--text-primary)',
                        lineHeight: 1.2,
                    }}>
                        {title}
                    </h1>
                )}
                {subtitle && (
                    <p style={{
                        marginTop: '.25rem',
                        fontSize: '.875rem',
                        color: 'var(--text-muted)',
                    }}>
                        {subtitle}
                    </p>
                )}
            </div>
            {actions && (
                <div className="flex items-center gap-3 shrink-0">
                    {actions}
                </div>
            )}
        </div>
    </header>
);
