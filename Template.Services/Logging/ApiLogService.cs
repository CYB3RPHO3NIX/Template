using Serilog;
using Serilog.Context;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;

namespace Template.Services.Logging;

public interface IApiLogService
{
    Task LogApiCallAsync(ApiLogEntry entry);
}

public class ApiLogEntry
{
    public string TraceId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? QueryString { get; set; }
    public int? ResponseStatusCode { get; set; }
    public long DurationMs { get; set; }
    public Guid? UserId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestHeaders { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? Exception { get; set; }
}

public class ApiLogService : IApiLogService
{
    private readonly TemplateDbContext _dbContext;

    public ApiLogService(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogApiCallAsync(ApiLogEntry entry)
    {
        try
        {
            var apiLog = new ApiLog
            {
                ApiLogId = Guid.NewGuid(),
                TraceId = entry.TraceId,
                Method = entry.Method,
                Path = entry.Path,
                QueryString = entry.QueryString,
                ResponseStatusCode = entry.ResponseStatusCode,
                DurationMs = entry.DurationMs,
                UserId = entry.UserId,
                UserAgent = entry.UserAgent,
                IpAddress = entry.IpAddress,
                RequestHeaders = entry.RequestHeaders,
                RequestBody = TruncateIfNeeded(entry.RequestBody, 5000),
                ResponseBody = TruncateIfNeeded(entry.ResponseBody, 5000),
                Exception = entry.Exception,
                CreatedOn = DateTime.UtcNow
            };

            _dbContext.ApiLogs.Add(apiLog);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            using (LogContext.PushProperty("TraceId", entry.TraceId))
            {
                Log.Warning(ex, "Failed to log API call to database");
            }
        }
    }

    private static string? TruncateIfNeeded(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Length > maxLength ? value[..maxLength] : value;
    }
}
