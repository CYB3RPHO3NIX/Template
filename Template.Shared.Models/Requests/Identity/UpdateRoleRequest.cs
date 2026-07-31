using Template.Shared.Models.Common;

namespace Template.Shared.Models.Requests.Identity
{
    public class UpdateRoleRequest : IRequest
    {
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }

        public ValidationResult Validate()
        {
            var validationResult = new ValidationResult();

            if (RoleId == Guid.Empty)
            {
                validationResult.Errors.Add("RoleId is required.");
            }

            if (RoleName != null && string.IsNullOrWhiteSpace(RoleName))
            {
                validationResult.Errors.Add("RoleName cannot be empty.");
            }

            if (RoleName != null && RoleName.Length > 100)
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
