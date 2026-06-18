import React from 'react';

interface CardProps {
    children: React.ReactNode;
    className?: string;
    title?: string;
    subtitle?: string;
    action?: React.ReactNode;
    noPadding?: boolean;
}

export const Card: React.FC<CardProps> = ({
    children,
    className = '',
    title,
    subtitle,
    action,
    noPadding = false,
}) => (
    <div className={`card ${className}`}>
        {(title || subtitle || action) && (
            <div style={{
                display: 'flex', alignItems: 'center', justifyContent: 'space-between',
                gap: '1rem', padding: '1.25rem 2rem',
                borderBottom: '1px solid var(--border-subtle)',
            }}>
                <div>
                    {title && (
                        <h3 style={{
                            fontFamily: "'Cormorant Garamond', Georgia, serif",
                            fontSize: '1.125rem', fontWeight: 600,
                            color: 'var(--text-primary)',
                        }}>
                            {title}
                        </h3>
                    )}
                    {subtitle && (
                        <p style={{ fontSize: '.8125rem', color: 'var(--text-muted)', marginTop: '.2rem' }}>
                            {subtitle}
                        </p>
                    )}
                </div>
                {action && <div style={{ flexShrink: 0 }}>{action}</div>}
            </div>
        )}
        {!noPadding && <div style={{ padding: '1.75rem 2rem' }}>{children}</div>}
        {noPadding && children}
    </div>
);
