using Serilog;
using Template.Contracts.MessageQueue;
using Template.Events.Identity;

namespace Template.Consumer.Listeners
{
    public class PermissionUpdatedEventListener : BaseEventListener<PermissionUpdatedEvent>
    {
        public PermissionUpdatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
        {
        }

        protected override async Task HandleEventAsync(PermissionUpdatedEvent @event)
        {
            Log.Information("Handling PermissionUpdatedEvent: PermissionId={PermissionId}, PermissionName={PermissionName}",
                @event.PermissionId, @event.PermissionName);

            // TODO: Implement business logic for handling permission updated events
            // Examples:
            // - Send notification to administrators
            // - Update search indexes
            // - Clear related caches
            // - Notify users with this permission

            await Task.Delay(100);

            Log.Information("PermissionUpdatedEvent handled successfully for PermissionId: {PermissionId}", @event.PermissionId);
        }
    }
}
