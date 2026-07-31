using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class RoleAssignedToUserEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
