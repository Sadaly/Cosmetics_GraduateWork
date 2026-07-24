export interface ErrorResponse {
    // Legacy flat format
    code?: string;
    message?: string;
    // ASP.NET Core ProblemDetails format
    title?: string;
    type?: string;
    detail?: string;
    status?: number;
}