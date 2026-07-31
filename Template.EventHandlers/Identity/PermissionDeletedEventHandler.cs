using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionDeletedEventHandler : IEventHandler<PermissionDeletedEvent>
    {
        public async Task Handle(PermissionDeletedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionDeletedEvent for PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Archive related data
                // - Update analytics
                // - Notify administrators

                await Task.Delay(100);

                Log.Information("PermissionDeletedEvent processed successfully for PermissionId: {PermissionId}", @event.PermissionId);
            }
        }
    }
}
