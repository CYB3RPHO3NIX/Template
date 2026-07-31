using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class PermissionCreatedEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public Guid? ParentPermissionId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
