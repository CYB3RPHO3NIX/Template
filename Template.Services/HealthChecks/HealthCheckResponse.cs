namespace Template.Services.HealthChecks;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public TimeSpan TotalDuration { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, HealthCheckEntryResponse> Entries { get; set; } = new();
}

public class HealthCheckEntryResponse
{
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object>? Data { get; set; }
    public string? Exception { get; set; }
}
