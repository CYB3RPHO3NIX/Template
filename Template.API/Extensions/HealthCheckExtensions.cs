using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Template.Services.HealthChecks;

namespace Template.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(
        this IServiceCollection services,
        IServiceProvider serviceProvider)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                "database",
                HealthStatus.Unhealthy,
                new[] { "ready" });

        return services;
    }

    public static WebApplication MapCustomHealthChecks(this WebApplication app)
    {
        var options = new HealthCheckOptions
        {
            ResponseWriter = WriteCustomResponse,
            AllowCachingResponses = false
        };

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            ResponseWriter = WriteCustomResponse,
            AllowCachingResponses = false,
            Predicate = check => check.Tags.Contains("live") || check.Tags.Count == 0
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResponseWriter = WriteCustomResponse,
            AllowCachingResponses = false,
            Predicate = check => check.Tags.Contains("ready")
        });

        app.MapHealthChecks("/health", options);

        return app;
    }

    private static async Task WriteCustomResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration,
            Timestamp = DateTime.UtcNow,
            Entries = new Dictionary<string, HealthCheckEntryResponse>()
        };

        foreach (var entry in report.Entries)
        {
            response.Entries[entry.Key] = new HealthCheckEntryResponse
            {
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description ?? string.Empty,
                Duration = entry.Value.Duration,
                Data = entry.Value.Data?.Count > 0 ? new Dictionary<string, object>(entry.Value.Data) : null,
                Exception = entry.Value.Exception?.Message
            };
        }

        context.Response.StatusCode = report.Status == HealthStatus.Healthy ? 200 : 503;

        await context.Response.WriteAsJsonAsync(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }
}
