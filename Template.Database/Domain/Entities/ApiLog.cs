namespace Template.Database.Domain.Entities;

public partial class ApiLog
{
    public Guid ApiLogId { get; set; }
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
    public DateTime CreatedOn { get; set; }
}
