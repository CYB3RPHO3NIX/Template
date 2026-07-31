using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class RoleCreatedEventHandler : IEventHandler<RoleCreatedEvent>
    {
        public async Task Handle(RoleCreatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing RoleCreatedEvent for RoleId: {RoleId}, RoleName: {RoleName}",
                    @event.RoleId, @event.RoleName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Update analytics/metrics
                // - Trigger downstream processes
                // - Send notifications

                await Task.Delay(100);

                Log.Information("RoleCreatedEvent processed successfully for RoleId: {RoleId}", @event.RoleId);
            }
        }
    }
}
