using System;
using System.Collections.Generic;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a security role.
    /// </summary>
    public class Role : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the users assigned to this role.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        /// <summary>
        /// Gets or sets the permissions assigned to this role.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    }
}
