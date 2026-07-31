using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionCreatedEventHandler : IEventHandler<PermissionCreatedEvent>
    {
        public async Task Handle(PermissionCreatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionCreatedEvent for PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Update analytics/metrics
                // - Trigger downstream processes
                // - Update search indexes

                await Task.Delay(100);

                Log.Information("PermissionCreatedEvent processed successfully for PermissionId: {PermissionId}", @event.PermissionId);
            }
        }
    }
}
