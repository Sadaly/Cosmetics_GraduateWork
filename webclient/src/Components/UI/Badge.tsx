import React from 'react';

interface BadgeProps {
    children: React.ReactNode;
    variant?: 'primary' | 'success' | 'warning' | 'danger' | 'neutral';
    className?: string;
}

export const Badge: React.FC<BadgeProps> = ({
    children,
    variant = 'neutral',
    className = '',
}) => (
    <span className={`badge badge-${variant} ${className}`}>
        {children}
    </span>
);
