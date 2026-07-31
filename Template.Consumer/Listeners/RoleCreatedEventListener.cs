using Serilog;
using Template.Contracts.MessageQueue;
using Template.Events.Identity;

namespace Template.Consumer.Listeners
{
    public class RoleCreatedEventListener : BaseEventListener<RoleCreatedEvent>
    {
        public RoleCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
        {
        }

        protected override async Task HandleEventAsync(RoleCreatedEvent @event)
        {
            Log.Information("Handling RoleCreatedEvent: RoleId={RoleId}, RoleName={RoleName}",
                @event.RoleId, @event.RoleName);

            // TODO: Implement business logic for handling role created events
            // Examples:
            // - Send notification to administrators
            // - Create audit log
            // - Update external systems
            // - Send to third-party services

            await Task.Delay(100);

            Log.Information("RoleCreatedEvent handled successfully for RoleId: {RoleId}", @event.RoleId);
        }
    }
}
