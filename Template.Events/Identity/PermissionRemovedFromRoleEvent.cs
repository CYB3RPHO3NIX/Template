using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class PermissionRemovedFromRoleEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public DateTime RemovedAt { get; set; }
    }
}
