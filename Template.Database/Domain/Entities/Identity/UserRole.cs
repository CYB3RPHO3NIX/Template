using System;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents a role assignment for a user.
    /// </summary>
    public class UserRole : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public Role Role { get; set; } = null!;
    }
}
