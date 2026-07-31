using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class RoleDeletedEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime DeletedOn { get; set; }
    }
}
