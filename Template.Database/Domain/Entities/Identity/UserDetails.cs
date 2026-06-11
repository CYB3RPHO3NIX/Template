using System;
using Template.Database.Abstractions;

namespace Template.Database.Domain.Entities.Identity
{
    public class UserDetails : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public User User { get; set; } = null!;
    }
}
