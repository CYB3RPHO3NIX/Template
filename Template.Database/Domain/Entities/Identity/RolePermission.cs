using System;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a permission assignment for a role.
    /// </summary>
    public class RolePermission : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets the permission identifier.
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Gets or sets the permission.
        /// </summary>
        public Permission Permission { get; set; } = null!;
    }
}
