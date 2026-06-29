using System;
using System.Collections.Generic;

namespace Template.Database.Domain.Entities;

public partial class RolePermission
{
    public Guid RolePermissionId { get; set; }

    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsAllowed { get; set; }

    public DateTime AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public virtual User? AssignedByNavigation { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
