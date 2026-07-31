namespace Template.Shared.Models.Pagination;

public class PaginatedResponse<T>
{
    public bool Success { get; set; } = true;
    public List<T> Items { get; set; } = new();
    public PaginationMetadata Pagination { get; set; } = new();
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginationMetadata
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => TotalCount == 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
