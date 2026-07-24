// ErrorService.ts
import axios from "axios";
import type { ErrorResponse } from "../TypesFromServer/ErrorResponse";

type ToastHandler = (message: string) => void;

class ErrorService {
    private static toastHandler: ToastHandler | null = null;

    static setToastHandler(handler: ToastHandler) {
        ErrorService.toastHandler = handler;
    }

    static showError(error: unknown, customMessage?: string) {
        if (!error) return;

        let errorMessage = customMessage || 'Произошла неизвестная ошибка';

        // Handle Axios errors
        if (axios.isAxiosError(error) && error.response?.data) {
            const data = error.response.data as ErrorResponse;
            // Handle both flat {message} and ASP.NET ProblemDetails {detail} formats
            errorMessage = data.message || data.detail || errorMessage;

            ErrorService.toastHandler?.(errorMessage);
            console.error(`API Error [${data.code || data.type || error.response?.status}]:`, errorMessage);
            return;
        }

        // Handle Axios errors without response data
        if (axios.isAxiosError(error)) {
            if (error.code === 'ECONNABORTED') {
                errorMessage = 'Превышено время ожидания ответа от сервера';
            } else if (error.code === 'ERR_NETWORK') {
                errorMessage = 'Ошибка сети. Проверьте подключение к серверу';
            } else {
                errorMessage = error.message || 'Ошибка соединения с сервером';
            }
            ErrorService.toastHandler?.(errorMessage);
            console.error('Network Error:', errorMessage);
            return;
        }

        // Handle generic Error objects
        if (error instanceof Error) {
            errorMessage = error.message;
            ErrorService.toastHandler?.(errorMessage);
            console.error('Error:', errorMessage);
            return;
        }

        // Fallback for unknown errors
        ErrorService.toastHandler?.(errorMessage);
        console.error('Unknown Error:', error);
    }
}

export { ErrorService };
