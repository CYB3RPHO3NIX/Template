using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionRemovedFromUserEventHandler : IEventHandler<PermissionRemovedFromUserEvent>
    {
        public async Task Handle(PermissionRemovedFromUserEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionRemovedFromUserEvent for UserId: {UserId}, PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.UserId, @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate user permissions cache
                // - Send notification to user
                // - Log security event
                // - Revoke access tokens

                await Task.Delay(100);

                Log.Information("PermissionRemovedFromUserEvent processed successfully for UserId: {UserId}", @event.UserId);
            }
        }
    }
}
