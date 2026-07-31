namespace Template.Shared.Models.DTOs.Identity
{
    public class PermissionDTO
    {
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public Guid? ParentPermissionId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
