using System;
using System.Collections.Generic;

namespace Template.Database.Domain.Entities;

public partial class UserPermission
{
    public Guid UserPermissionId { get; set; }

    public Guid UserId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsAllowed { get; set; }

    public string? Reason { get; set; }

    public DateTime AssignedAt { get; set; }

    public Guid? AssignedBy { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public virtual User? AssignedByNavigation { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
