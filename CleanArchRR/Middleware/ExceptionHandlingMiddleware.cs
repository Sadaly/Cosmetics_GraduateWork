using Domain.Shared;
using System.Net;
using System.Text.Json;

namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, error) = MapExceptionToResponse(exception);
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                Code = error.Code,
                Message = error.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static (HttpStatusCode statusCode, Error error) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                NullReferenceException _ => (HttpStatusCode.InternalServerError,
                    new Error("NullReference", "Object reference not set")),

                ArgumentException _ => (HttpStatusCode.BadRequest,
                    new Error("InvalidArgument", exception.Message)),

                InvalidOperationException _ => (HttpStatusCode.BadRequest,
                    new Error("InvalidOperation", exception.Message)),

                _ => (HttpStatusCode.InternalServerError,
                    new Error("InvalidException", exception.Message))
            };
        }
    }
}