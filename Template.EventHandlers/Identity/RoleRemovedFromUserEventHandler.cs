using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class RoleRemovedFromUserEventHandler : IEventHandler<RoleRemovedFromUserEvent>
    {
        public async Task Handle(RoleRemovedFromUserEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing RoleRemovedFromUserEvent for UserId: {UserId}, RoleId: {RoleId}, RoleName: {RoleName}",
                    @event.UserId, @event.RoleId, @event.RoleName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Send notification to user
                // - Update user permissions cache
                // - Log security event
                // - Revoke access tokens

                await Task.Delay(100);

                Log.Information("RoleRemovedFromUserEvent processed successfully for UserId: {UserId}", @event.UserId);
            }
        }
    }
}
