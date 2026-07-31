using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionUpdatedEventHandler : IEventHandler<PermissionUpdatedEvent>
    {
        public async Task Handle(PermissionUpdatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionUpdatedEvent for PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate cache
                // - Update search indexes
                // - Notify connected clients

                await Task.Delay(100);

                Log.Information("PermissionUpdatedEvent processed successfully for PermissionId: {PermissionId}", @event.PermissionId);
            }
        }
    }
}
