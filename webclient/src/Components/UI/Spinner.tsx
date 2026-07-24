import React from 'react';

interface SpinnerProps {
    size?: 'sm' | 'md' | 'lg';
    className?: string;
}

const sizeMap = { sm: 16, md: 32, lg: 48 };

export const Spinner: React.FC<SpinnerProps> = ({ size = 'md', className = '' }) => {
    const px = sizeMap[size];
    return (
        <div className={`flex justify-center items-center ${className}`}>
            <div
                className="animate-spin"
                style={{
                    width: px, height: px,
                    borderRadius: '50%',
                    border: `${size === 'sm' ? 2 : 3}px solid var(--border-subtle)`,
                    borderTopColor: 'var(--color-primary)',
                }}
            />
        </div>
    );
};

export const LoadingOverlay: React.FC<{ message?: string }> = ({ message = 'Загрузка...' }) => (
    <div className="fixed inset-0 flex items-center justify-center z-50"
        style={{ background: 'rgba(44,26,36,.5)', backdropFilter: 'blur(4px)' }}>
        <div className="flex flex-col items-center gap-4 p-8"
            style={{ background: 'var(--bg-surface)', borderRadius: 'var(--radius-xl)', boxShadow: 'var(--shadow-xl)' }}>
            <Spinner size="lg" />
            {message && <p style={{ color: 'var(--text-secondary)', fontWeight: 500 }}>{message}</p>}
        </div>
    </div>
);
