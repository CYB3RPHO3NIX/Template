namespace Template.Shared.Models.Exceptions;

public class ApplicationException : Exception
{
    public int StatusCode { get; set; } = 500;
    public string ErrorCode { get; set; } = "INTERNAL_ERROR";
    public Dictionary<string, List<string>>? Details { get; set; }

    public ApplicationException(string message, int statusCode = 500, string errorCode = "INTERNAL_ERROR")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
