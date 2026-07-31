using Serilog;
using Serilog.Context;
using Template.Contracts.EventHandler;
using Template.Events.Identity;

namespace Template.EventHandlers.Identity
{
    public class RoleDeletedEventHandler : IEventHandler<RoleDeletedEvent>
    {
        public async Task Handle(RoleDeletedEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                Log.Information("Processing RoleDeletedEvent for RoleId: {RoleId}, RoleName: {RoleName}",
                    @event.RoleId, @event.RoleName);

                // Event processing logic here
                // Examples:
                // - Create audit log entry
                // - Archive related data
                // - Update analytics
                // - Notify administrators

                await Task.Delay(100);

                Log.Information("RoleDeletedEvent processed successfully for RoleId: {RoleId}", @event.RoleId);
            }
        }
    }
}
