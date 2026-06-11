using System;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{

    public class UserRole : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public User User { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }
}
