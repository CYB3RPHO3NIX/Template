using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionAssignedToUserEventHandler : IEventHandler<PermissionAssignedToUserEvent>
    {
        public async Task Handle(PermissionAssignedToUserEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionAssignedToUserEvent for UserId: {UserId}, PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.UserId, @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate user permissions cache
                // - Send notification to user
                // - Log security event

                await Task.Delay(100);

                Log.Information("PermissionAssignedToUserEvent processed successfully for UserId: {UserId}", @event.UserId);
            }
        }
    }
}
