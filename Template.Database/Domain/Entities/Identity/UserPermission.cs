using System;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a direct permission assignment or override for a user.
    /// </summary>
    public class UserPermission : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the permission identifier.
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the access behavior for the permission.
        /// </summary>
        public PermissionAccess Access { get; set; } = PermissionAccess.Allow;

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the permission.
        /// </summary>
        public Permission Permission { get; set; } = null!;
    }
}
