namespace Template.Shared.Models.Exceptions;

public class ResourceNotFoundException : ApplicationException
{
    public ResourceNotFoundException(string resourceName, object? id = null)
        : base(
            id != null ? $"{resourceName} with id {id} not found" : $"{resourceName} not found",
            statusCode: 404,
            errorCode: "RESOURCE_NOT_FOUND")
    {
    }
}
