using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Template.Database.Domain.Contexts;

namespace Template.Services.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly TemplateDbContext _dbContext;

    public DatabaseHealthCheck(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy("Cannot connect to database");
            }

            var startTime = DateTime.UtcNow;
            _ = await _dbContext.Users.CountAsync(cancellationToken);
            var duration = DateTime.UtcNow - startTime;

            var data = new Dictionary<string, object>
            {
                { "duration_ms", duration.TotalMilliseconds },
                { "timestamp", DateTime.UtcNow }
            };

            return HealthCheckResult.Healthy("Database is healthy", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database health check failed: {ex.Message}", ex);
        }
    }
}
