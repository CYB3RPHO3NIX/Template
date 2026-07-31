using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        public async Task Handle(UserCreatedEvent @event)
        {
            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing UserCreatedEvent for UserId: {UserId}, UserName: {UserName}, Email: {Email}",
                    @event.UserId, @event.UserName, @event.Email);

                // Event processing logic here
                // Examples:
                // - Send welcome email notification
                // - Create audit log entry
                // - Update analytics/metrics
                // - Trigger downstream processes
                // - Send to message queue for async processing

                // Simulating async work
                await Task.Delay(100);

                Log.Information("UserCreatedEvent processed successfully for UserId: {UserId}", @event.UserId);
            }
        }
    }
}
