using System;
using Template.Database.Abstractions;
using Template.Shared.Models.Common;

namespace Template.Database.Domain.Entities.Identity
{
    public class UserPermission : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PermissionId { get; set; }

        public string Access { get; set; } = AccessTypes.Allow;

        public User User { get; set; } = null!;

        public Permission Permission { get; set; } = null!;
    }
}
