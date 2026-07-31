using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class UpdatePermissionRequest : IRequest
    {
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }

        public ValidationResult Validate()
        {
            var validationResult = new ValidationResult();

            if (PermissionId == Guid.Empty)
            {
                validationResult.Errors.Add("PermissionId is required.");
            }

            if (PermissionName != null && string.IsNullOrWhiteSpace(PermissionName))
            {
                validationResult.Errors.Add("PermissionName cannot be empty.");
            }

            if (PermissionName != null && PermissionName.Length > 100)
            {
                validationResult.Errors.Add("PermissionName cannot exceed 100 characters.");
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
