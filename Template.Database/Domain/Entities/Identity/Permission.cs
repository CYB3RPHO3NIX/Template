using System;
using System.Collections.Generic;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents an access permission.
    /// </summary>
    public class Permission : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the permission description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the roles associated with this permission.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();

        /// <summary>
        /// Gets or sets the users associated with this permission.
        /// </summary>
        public ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();
    }
}
