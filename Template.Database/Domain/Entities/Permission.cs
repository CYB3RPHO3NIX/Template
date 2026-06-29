using System;
using System.Collections.Generic;

namespace Template.Database.Domain.Entities;

public partial class Permission
{
    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string PermissionCode { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public Guid? ParentPermissionId { get; set; }

    public bool IsActive { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Permission> InverseParentPermission { get; set; } = new List<Permission>();

    public virtual Permission? ParentPermission { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
