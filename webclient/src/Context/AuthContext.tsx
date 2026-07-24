import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import api from '../api/api';

interface User {
    id: string;
    email: string;
    fullname?: string;
    role?: string;
}

interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    token: string | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

interface AuthProviderProps {
    children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [token, setToken] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const storedToken = localStorage.getItem('authToken');
        const storedUser = localStorage.getItem('user');

        if (storedToken && storedUser) {
            setToken(storedToken);
            setUser(JSON.parse(storedUser));
            api.defaults.headers.common['Authorization'] = `Bearer ${storedToken}`;
        }
        setIsLoading(false);
    }, []);

    const login = async (email: string, password: string) => {
        try {
            const response = await api.post('/Users/Login', { email, password });
            const { token: newToken, user: userData } = response.data;

            if (!newToken) {
                throw new Error('Token not received');
            }

            setToken(newToken);
            setUser(userData);
            localStorage.setItem('authToken', newToken);
            localStorage.setItem('user', JSON.stringify(userData));
            api.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
        } catch (error) {
            localStorage.removeItem('authToken');
            localStorage.removeItem('user');
            throw error;
        }
    };

    const logout = () => {
        // Call server to clear the HttpOnly JWT cookie
        api.post('/Users/Logout').catch(() => {});
        setToken(null);
        setUser(null);
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
        delete api.defaults.headers.common['Authorization'];
    };

    const value: AuthContextType = {
        user,
        isAuthenticated: !!token,
        isLoading,
        login,
        logout,
        token,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
