using HMS.Api.Contracts;
using HMS.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, code) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Domain rule violation", "domain_rule_violation"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", "internal_error")
        };

        logger.LogError(exception, "Unhandled exception for trace {TraceId}", context.TraceIdentifier);
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Extensions = { ["error"] = new ApiError(code, title, context.TraceIdentifier) }
        }, cancellationToken);
        return true;
    }
}
