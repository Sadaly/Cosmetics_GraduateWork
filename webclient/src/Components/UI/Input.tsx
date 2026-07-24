import React from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    hint?: string;
    icon?: React.ReactNode;
}

export const Input: React.FC<InputProps> = ({
    label,
    error,
    hint,
    icon,
    className = '',
    id,
    ...props
}) => {
    const inputId = id || label?.toLowerCase().replace(/\s+/g, '-');

    return (
        <div className="form-group">
            {label && (
                <label htmlFor={inputId} className="form-label">
                    {label}
                </label>
            )}
            <div className="relative">
                {icon && (
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
                        style={{ color: 'var(--text-muted)' }}>
                        {icon}
                    </div>
                )}
                <input
                    id={inputId}
                    className={`form-input ${icon ? 'pl-10' : ''} ${error ? 'border-red-400 focus:border-red-400' : ''} ${className}`}
                    {...props}
                />
            </div>
            {hint && !error && (
                <p className="mt-1 text-xs" style={{ color: 'var(--text-muted)' }}>{hint}</p>
            )}
            {error && (
                <p className="mt-1 text-xs font-medium" style={{ color: 'var(--color-danger)' }}>{error}</p>
            )}
        </div>
    );
};
