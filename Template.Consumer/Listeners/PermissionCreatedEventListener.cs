using Serilog;
using Template.Contracts.MessageQueue;
using Template.Events.Identity;

namespace Template.Consumer.Listeners
{
    public class PermissionCreatedEventListener : BaseEventListener<PermissionCreatedEvent>
    {
        public PermissionCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
        {
        }

        protected override async Task HandleEventAsync(PermissionCreatedEvent @event)
        {
            Log.Information("Handling PermissionCreatedEvent: PermissionId={PermissionId}, PermissionName={PermissionName}",
                @event.PermissionId, @event.PermissionName);

            // TODO: Implement business logic for handling permission created events
            // Examples:
            // - Send notification to administrators
            // - Update search indexes
            // - Create audit log
            // - Update external systems

            await Task.Delay(100);

            Log.Information("PermissionCreatedEvent handled successfully for PermissionId: {PermissionId}", @event.PermissionId);
        }
    }
}
