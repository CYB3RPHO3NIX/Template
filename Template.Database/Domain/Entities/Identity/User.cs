using System;
using System.Collections.Generic;
using System.Linq;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    /// <summary>
    /// Represents an application user.
    /// </summary>
    public class User : IEntity<Guid>
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username for the user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password hash for the user.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password salt for the user.
        /// </summary>
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the user's profile details.
        /// </summary>
        public UserDetails? UserDetails { get; set; }

        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        /// <summary>
        /// Gets or sets the permissions assigned directly to the user.
        /// </summary>
        public ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();

        /// <summary>
        /// Assigns a role to the user when it is not already assigned.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        public void AssignRole(Guid roleId)
        {
            if (UserRoles.Any(userRole => userRole.RoleId == roleId))
            {
                return;
            }

            UserRoles.Add(new UserRole
            {
                UserId = Id,
                RoleId = roleId
            });
        }

        /// <summary>
        /// Revokes a role from the user when assigned.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        public void RevokeRole(Guid roleId)
        {
            var userRole = UserRoles.FirstOrDefault(existingUserRole => existingUserRole.RoleId == roleId);

            if (userRole is null)
            {
                return;
            }

            UserRoles.Remove(userRole);
        }

        /// <summary>
        /// Assigns or updates a direct permission override for the user.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        /// <param name="access">The access override to apply.</param>
        public void AssignPermission(Guid permissionId, PermissionAccess access)
        {
            var userPermission = UserPermissions.FirstOrDefault(existingUserPermission => existingUserPermission.PermissionId == permissionId);

            if (userPermission is null)
            {
                UserPermissions.Add(new UserPermission
                {
                    UserId = Id,
                    PermissionId = permissionId,
                    Access = access
                });

                return;
            }

            userPermission.Access = access;
        }

        /// <summary>
        /// Revokes a direct permission assignment from the user.
        /// </summary>
        /// <param name="permissionId">The permission identifier.</param>
        public void RevokePermission(Guid permissionId)
        {
            var userPermission = UserPermissions.FirstOrDefault(existingUserPermission => existingUserPermission.PermissionId == permissionId);

            if (userPermission is null)
            {
                return;
            }

            UserPermissions.Remove(userPermission);
        }
    }
}
