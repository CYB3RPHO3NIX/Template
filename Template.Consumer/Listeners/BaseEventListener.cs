using Serilog;
using Serilog.Context;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.Listeners
{
    public abstract class BaseEventListener<TEvent> : IEventListener where TEvent : IEvent
    {
        protected readonly IMessageQueue _messageQueue;

        public BaseEventListener(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Starting listener for event type: {EventType}", typeof(TEvent).Name);
                await _messageQueue.SubscribeAsync<TEvent>(HandleEventAsync);
                Log.Information("Listener started for event type: {EventType}", typeof(TEvent).Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error starting listener for event type: {EventType}", typeof(TEvent).Name);
                throw;
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Stopping listener for event type: {EventType}", typeof(TEvent).Name);
                await Task.CompletedTask;
                Log.Information("Listener stopped for event type: {EventType}", typeof(TEvent).Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error stopping listener for event type: {EventType}", typeof(TEvent).Name);
                throw;
            }
        }

        protected abstract Task HandleEventAsync(TEvent @event);

        protected async Task ProcessEventAsync(TEvent @event)
        {
            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                try
                {
                    Log.Information("Processing event of type: {EventType}", typeof(TEvent).Name);
                    await HandleEventAsync(@event);
                    Log.Information("Event processed successfully: {EventType}", typeof(TEvent).Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing event of type: {EventType}", typeof(TEvent).Name);
                    throw;
                }
            }
        }
    }
}
