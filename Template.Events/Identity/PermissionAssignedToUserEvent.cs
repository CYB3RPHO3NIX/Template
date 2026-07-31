using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class PermissionAssignedToUserEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid UserPermissionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
        public string? Reason { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
