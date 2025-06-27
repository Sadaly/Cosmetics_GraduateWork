using Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    internal static IActionResult ToActionResult<T>(this T value) { return new OkObjectResult(value); }
    internal static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return result switch
        {
            IValidationResult validationResult =>
                new BadRequestObjectResult(
                    CreateProblemDetails(
                        "Validation Error",
                        StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors)),
            _ =>
                new BadRequestObjectResult(
                    CreateProblemDetails(
                        "Bad Request",
                        StatusCodes.Status400BadRequest,
                        result.Error))
        };
    }

    private static ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Type = error.Code,
            Detail = error.Message,
            Status = status,
            Extensions = { ["errors"] = errors }
        };
    }
}