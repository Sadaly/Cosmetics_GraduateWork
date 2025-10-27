// ErrorService.ts
import axios from "axios";
import type { ErrorResponse } from "../TypesFromServer/ErrorResponse";

class ErrorService {
    static showError(error: unknown) {
        if (!error) return;

        // Handle Axios errors
        if (axios.isAxiosError(error) && error.response?.data) {
            const data = error.response.data as ErrorResponse;
            alert(`Ошибка: ${data.message} (код: ${data.code})`);
            return;
        }

        // Handle generic Error objects
        if (error instanceof Error) {
            alert(`Ошибка: ${error.message}`);
            return;
        }

        // Fallback for unknown errors
        alert(`Произошла неизвестная ошибка: ${JSON.stringify(error)}`);
    }
}

export { ErrorService };
