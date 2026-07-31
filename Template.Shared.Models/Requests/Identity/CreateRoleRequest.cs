using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class CreateRoleRequest : IRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }

        public ValidationResult Validate()
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(RoleName))
            {
                validationResult.Errors.Add("RoleName is required.");
            }

            if (RoleName.Length > 100)
            {
                validationResult.Errors.Add("RoleName cannot exceed 100 characters.");
            }

            if (Description != null && Description.Length > 500)
            {
                validationResult.Errors.Add("Description cannot exceed 500 characters.");
            }

            return validationResult;
        }
    }
}
