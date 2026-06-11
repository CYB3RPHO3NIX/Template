using System;
using System.Collections.Generic;
using System.Linq;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    public class User : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public UserDetails? UserDetails { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        public ICollection<UserPermission> UserPermissions { get; set; } = new HashSet<UserPermission>();
    }
}
