using InventorySystem.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Data.SqlClient;

namespace InventorySystem.API.Middleware;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Log.Error(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            ValidationException validationException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Detail = "One or more validation errors occurred.",
                Extensions = { { "errors", validationException.Errors } }
            },
            DbUpdateException dbUpdateException when dbUpdateException.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627) => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Detail = "A conflict occurred with the database constraints (e.g. duplicate key)."
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7807",
                Detail = "An unexpected error occurred." 
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
