using Serilog;
using Serilog.Context;
using System.Security.Claims;
using System.Text;
using Template.Services.Logging;

namespace Template.API.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public RequestResponseLoggingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var traceId = context.TraceIdentifier;

        // Skip logging for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        using (LogContext.PushProperty("TraceId", traceId))
        {
            try
            {
                // Capture request body
                var requestBody = await ReadRequestBodyAsync(context.Request);

                // Capture response body
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    var responseContent = await ReadResponseBodyAsync(responseBody);

                    // Log the API call
                    var duration = DateTime.UtcNow - startTime;
                    await LogApiCallAsync(context, requestBody, responseContent, duration, null);

                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                await LogApiCallAsync(context, null, null, duration, ex);

                Log.Error(ex, "Request processing failed");
                throw;
            }
        }
    }

    private async Task LogApiCallAsync(
        HttpContext context,
        string? requestBody,
        string? responseBody,
        TimeSpan duration,
        Exception? exception)
    {
        using var scope = _serviceProvider.CreateScope();
        var logService = scope.ServiceProvider.GetRequiredService<IApiLogService>();

        var entry = new ApiLogEntry
        {
            TraceId = context.TraceIdentifier,
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
            ResponseStatusCode = context.Response.StatusCode,
            DurationMs = (long)duration.TotalMilliseconds,
            UserId = GetUserIdFromContext(context),
            UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault(),
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            RequestHeaders = SerializeHeaders(context.Request.Headers),
            RequestBody = ShouldLogBody(context.Request.Method) ? requestBody : null,
            ResponseBody = ShouldLogResponseBody(context.Response.StatusCode) ? responseBody : null,
            Exception = exception?.ToString()
        };

        await logService.LogApiCallAsync(entry);
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        if (!ShouldLogBody(request.Method))
            return null;

        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        request.Body.Position = 0;

        return string.IsNullOrEmpty(body) ? null : body;
    }

    private static async Task<string?> ReadResponseBodyAsync(MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);

        return string.IsNullOrEmpty(body) ? null : body;
    }

    private static bool ShouldLogBody(string method)
    {
        return method is "POST" or "PATCH" or "PUT";
    }

    private static bool ShouldLogResponseBody(int statusCode)
    {
        return statusCode >= 400; // Log error responses
    }

    private static Guid? GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim?.Value is not null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    private static string SerializeHeaders(IHeaderDictionary headers)
    {
        var sensitiveHeaders = new[] { "Authorization", "Cookie", "X-API-Key", "Password" };

        var safeHeaders = headers
            .Where(h => !sensitiveHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .Select(h => $"{h.Key}: {h.Value}")
            .Take(10); // Limit to first 10 headers

        return string.Join(", ", safeHeaders);
    }
}
