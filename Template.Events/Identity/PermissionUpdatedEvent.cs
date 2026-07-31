using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class PermissionUpdatedEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
