using Template.Contracts.Event;

namespace Template.Events.Identity
{
    public class UserCreatedEvent : IEvent
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}
