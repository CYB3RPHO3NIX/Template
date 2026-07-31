using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity;

public class LoginRequest : IRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ValidationResult Validate()
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(Email))
        {
            validationResult.Errors.Add("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            validationResult.Errors.Add("Password is required.");
        }

        return validationResult;
    }
}
