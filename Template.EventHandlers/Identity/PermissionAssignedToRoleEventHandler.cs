using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class PermissionAssignedToRoleEventHandler : IEventHandler<PermissionAssignedToRoleEvent>
    {
        public async Task Handle(PermissionAssignedToRoleEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing PermissionAssignedToRoleEvent for RoleId: {RoleId}, PermissionId: {PermissionId}, PermissionName: {PermissionName}",
                    @event.RoleId, @event.PermissionId, @event.PermissionName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate role permissions cache
                // - Notify administrators
                // - Update search indexes

                await Task.Delay(100);

                Log.Information("PermissionAssignedToRoleEvent processed successfully for RoleId: {RoleId}", @event.RoleId);
            }
        }
    }
}
