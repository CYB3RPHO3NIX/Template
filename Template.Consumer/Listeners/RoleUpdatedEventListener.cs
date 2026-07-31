using Serilog;
using Template.Contracts.MessageQueue;
using Template.Events.Identity;

namespace Template.Consumer.Listeners
{
    public class RoleUpdatedEventListener : BaseEventListener<RoleUpdatedEvent>
    {
        public RoleUpdatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
        {
        }

        protected override async Task HandleEventAsync(RoleUpdatedEvent @event)
        {
            Log.Information("Handling RoleUpdatedEvent: RoleId={RoleId}, RoleName={RoleName}",
                @event.RoleId, @event.RoleName);

            // TODO: Implement business logic for handling role updated events
            // Examples:
            // - Send notification to administrators
            // - Update external systems
            // - Clear related caches
            // - Audit trail updates

            await Task.Delay(100);

            Log.Information("RoleUpdatedEvent handled successfully for RoleId: {RoleId}", @event.RoleId);
        }
    }
}
