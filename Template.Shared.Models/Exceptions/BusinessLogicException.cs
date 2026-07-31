namespace Template.Shared.Models.Exceptions;

public class BusinessLogicException : ApplicationException
{
    public BusinessLogicException(string message, string errorCode = "BUSINESS_ERROR")
        : base(message, statusCode: 400, errorCode: errorCode)
    {
    }
}
