using System;
using System.Collections.Generic;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    public class Permission : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();

        public ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();
    }
}
