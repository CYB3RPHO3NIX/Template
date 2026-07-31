using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class RoleUpdatedEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
