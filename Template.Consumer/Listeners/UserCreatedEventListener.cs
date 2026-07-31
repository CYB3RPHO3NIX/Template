using Serilog;
using Template.Contracts.MessageQueue;
using Template.Events.Identity;

namespace Template.Consumer.Listeners
{
    public class UserCreatedEventListener : BaseEventListener<UserCreatedEvent>
    {
        public UserCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
        {
        }

        protected override async Task HandleEventAsync(UserCreatedEvent @event)
        {
            // TODO: Implement business logic for handling user created events
            // Examples:
            // - Send welcome email
            // - Create user in external system
            // - Update analytics
            // - Send notification

            Log.Information("Handling UserCreatedEvent: UserId={UserId}, Email={Email}",
                @event.UserId, @event.Email);

            // Simulate async work
            await Task.Delay(100);

            Log.Information("UserCreatedEvent handled successfully for UserId: {UserId}", @event.UserId);
        }
    }
}
