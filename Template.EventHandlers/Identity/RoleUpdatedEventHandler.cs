using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class RoleUpdatedEventHandler : IEventHandler<RoleUpdatedEvent>
    {
        public async Task Handle(RoleUpdatedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing RoleUpdatedEvent for RoleId: {RoleId}, RoleName: {RoleName}",
                    @event.RoleId, @event.RoleName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Invalidate cache
                // - Update search indexes
                // - Notify connected clients

                await Task.Delay(100);

                Log.Information("RoleUpdatedEvent processed successfully for RoleId: {RoleId}", @event.RoleId);
            }
        }
    }
}
