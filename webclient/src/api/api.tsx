// api.ts
import axios from "axios";
import { ErrorService } from "../ErrorHandlingMiddleware/ErrorService";

const api = axios.create({
    baseURL: "/api",
    withCredentials: true,
});

// Request interceptor to add auth token
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('authToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor for error handling
api.interceptors.response.use(
    (response) => response,
    (error) => {
        // Handle 401 Unauthorized - redirect to login
        if (error.response?.status === 401) {
            localStorage.removeItem('authToken');
            localStorage.removeItem('user');
            window.location.href = '/';
            return Promise.reject(error);
        }
        
        // Handle 403 Forbidden
        if (error.response?.status === 403) {
            ErrorService.showError(error, 'Доступ запрещен. Недостаточно прав для выполнения операции');
            return Promise.reject(error);
        }
        
        // Handle 404 Not Found
        if (error.response?.status === 404) {
            ErrorService.showError(error, 'Запрашиваемый ресурс не найден');
            return Promise.reject(error);
        }
        
        // Handle 500 Server Error
        if (error.response?.status === 500) {
            ErrorService.showError(error, 'Ошибка сервера. Попробуйте позже');
            return Promise.reject(error);
        }
        
        // Handle other errors
        ErrorService.showError(error);
        return Promise.reject(error);
    }
);

export default api;
