import React, { createContext, useContext, useState, useCallback } from 'react';
import { Toast } from '../Components/UI/Toast';

type ToastType = 'success' | 'error' | 'warning' | 'info';

interface ToastContextType {
    showToast: (message: string, type?: ToastType) => void;
    showSuccess: (message: string) => void;
    showError: (message: string) => void;
    showWarning: (message: string) => void;
    showInfo: (message: string) => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

export const useToast = () => {
    const context = useContext(ToastContext);
    if (!context) throw new Error('useToast must be used within a ToastProvider');
    return context;
};

interface ToastMessage {
    id: string;
    message: string;
    type: ToastType;
}

export const ToastProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [toasts, setToasts] = useState<ToastMessage[]>([]);

    const removeToast = useCallback((id: string) => {
        setToasts(prev => prev.filter(t => t.id !== id));
    }, []);

    const showToast = useCallback((message: string, type: ToastType = 'info') => {
        const id = crypto.randomUUID?.() ?? Math.random().toString(36).slice(2);
        // Max 4 simultaneous toasts
        setToasts(prev => [...prev.slice(-3), { id, message, type }]);
    }, []);

    const showSuccess = useCallback((msg: string) => showToast(msg, 'success'), [showToast]);
    const showError   = useCallback((msg: string) => showToast(msg, 'error'),   [showToast]);
    const showWarning = useCallback((msg: string) => showToast(msg, 'warning'), [showToast]);
    const showInfo    = useCallback((msg: string) => showToast(msg, 'info'),    [showToast]);

    return (
        <ToastContext.Provider value={{ showToast, showSuccess, showError, showWarning, showInfo }}>
            {children}
            {/* Toast container — stacked in top-right */}
            <div
                style={{
                    position: 'fixed',
                    top: 20, right: 20,
                    zIndex: 10000,
                    display: 'flex',
                    flexDirection: 'column',
                    gap: 10,
                    pointerEvents: 'none',
                    maxWidth: 'calc(100vw - 40px)',
                }}
            >
                {toasts.map(toast => (
                    <div key={toast.id} style={{ pointerEvents: 'auto' }}>
                        <Toast
                            message={toast.message}
                            type={toast.type}
                            onClose={() => removeToast(toast.id)}
                        />
                    </div>
                ))}
            </div>
        </ToastContext.Provider>
    );
};
