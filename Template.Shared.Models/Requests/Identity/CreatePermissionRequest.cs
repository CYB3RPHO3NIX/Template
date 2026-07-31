using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class CreatePermissionRequest : IRequest
    {
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public Guid? ParentPermissionId { get; set; }

        public ValidationResult Validate()
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(PermissionName))
            {
                validationResult.Errors.Add("PermissionName is required.");
            }

            if (PermissionName.Length > 100)
            {
                validationResult.Errors.Add("PermissionName cannot exceed 100 characters.");
            }

            if (string.IsNullOrWhiteSpace(PermissionCode))
            {
                validationResult.Errors.Add("PermissionCode is required.");
            }

            if (PermissionCode.Length > 50)
            {
                validationResult.Errors.Add("PermissionCode cannot exceed 50 characters.");
            }

            if (Description != null && Description.Length > 500)
            {
                validationResult.Errors.Add("Description cannot exceed 500 characters.");
            }

            if (Category != null && Category.Length > 100)
            {
                validationResult.Errors.Add("Category cannot exceed 100 characters.");
            }

            return validationResult;
        }
    }
}
