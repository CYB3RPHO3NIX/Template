using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Shared.Models.Exceptions;

namespace Template.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
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
        var traceId = context.TraceIdentifier;

        var (statusCode, errorCode, message, details) = GetErrorInfo(exception);

        using (LogContext.PushProperty("TraceId", traceId))
        {
            if (statusCode == 500)
            {
                Log.Error(exception, "Unhandled exception: {ErrorCode}", errorCode);
            }
            else
            {
                Log.Warning(exception, "Business exception: {ErrorCode} - {Message}", errorCode, message);
            }
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var response = new
        {
            type = $"https://api.template.local/errors/{errorCode}",
            title = GetErrorTitle(errorCode),
            status = statusCode,
            detail = message,
            traceId = traceId,
            timestamp = DateTime.UtcNow,
            errors = details
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private static (int StatusCode, string ErrorCode, string Message, Dictionary<string, List<string>>? Details) GetErrorInfo(Exception exception)
    {
        return exception switch
        {
            Template.Shared.Models.Exceptions.ApplicationException ae =>
                (ae.StatusCode, ae.ErrorCode, ae.Message, ae.Details),

            ArgumentNullException ane =>
                (400, "INVALID_REQUEST", ane.Message ?? "Invalid request", null),

            DbUpdateConcurrencyException =>
                (409, "CONFLICT", "Resource was modified by another user", null),

            _ => (500, "INTERNAL_ERROR", "An unexpected error occurred", null)
        };
    }

    private static string GetErrorTitle(string errorCode) =>
        errorCode switch
        {
            "VALIDATION_ERROR" => "Validation Failed",
            "RESOURCE_NOT_FOUND" => "Resource Not Found",
            "BUSINESS_ERROR" => "Business Logic Error",
            "INVALID_REQUEST" => "Invalid Request",
            "CONFLICT" => "Conflict",
            _ => "Internal Server Error"
        };
}
