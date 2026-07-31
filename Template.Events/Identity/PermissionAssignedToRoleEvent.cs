using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class PermissionAssignedToRoleEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
