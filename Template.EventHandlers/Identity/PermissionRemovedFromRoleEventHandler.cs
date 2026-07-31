using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionRemovedFromRoleEventHandler : IEventHandler<PermissionRemovedFromRoleEvent>
    {
        public async Task Handle(PermissionRemovedFromRoleEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionRemovedFromRoleEvent for RoleId: {RoleId}, PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.RoleId, @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate role permissions cache
                // - Notify administrators
                // - Revoke access for users with this role

                await Task.Delay(100);

                Log.Information("PermissionRemovedFromRoleEvent processed successfully for RoleId: {RoleId}", @event.RoleId);
            }
        }
    }
}
