namespace Template.Shared.Models.Exceptions;

public class ValidationException : ApplicationException
{
    public ValidationException(Dictionary<string, List<string>> errors)
        : base("Validation failed", statusCode: 400, errorCode: "VALIDATION_ERROR")
    {
        Details = errors;
    }

    public ValidationException(string fieldName, string errorMessage)
        : base("Validation failed", statusCode: 400, errorCode: "VALIDATION_ERROR")
    {
        Details = new Dictionary<string, List<string>>
        {
            { fieldName, new List<string> { errorMessage } }
        };
    }
}
