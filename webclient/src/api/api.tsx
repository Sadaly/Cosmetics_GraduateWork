// api.ts
import axios from "axios";
import { ErrorService } from "../ErrorHandlingMiddleware/ErrorService";

const api = axios.create({
    baseURL: "https://localhost:7135/api",
    withCredentials: true,
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        ErrorService.showError(error); // show error toast automatically
        return Promise.reject(error);
    }
);

export default api;
