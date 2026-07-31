using Serilog;
using Serilog.Context;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.Listeners
{
    /// <summary>
    /// Base class for event listeners.
    /// Template Method pattern: subclasses implement HandleEventAsync only.
    /// Encapsulates logging, error handling, and subscription logic.
    /// Single Responsibility: manages event processing lifecycle.
    /// </summary>
    public abstract class BaseEventListener<TEvent> : IEventListener where TEvent : IEvent
    {
        protected readonly IMessageQueue _messageQueue;
        private readonly string _eventTypeName = typeof(TEvent).Name;

        protected BaseEventListener(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            LogListenerStarting();

            await _messageQueue.SubscribeAsync<TEvent>(ProcessEventAsync);

            LogListenerStarted();
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            LogListenerStopping();
            await Task.CompletedTask;
            LogListenerStopped();
        }

        protected abstract Task HandleEventAsync(TEvent @event);

        private async Task ProcessEventAsync(TEvent @event)
        {
            using (LogContext.PushProperty("TraceId", @event.TraceId))
            {
                LogEventProcessingStarted();

                await ExecuteWithErrorHandling(() => HandleEventAsync(@event));

                LogEventProcessingCompleted();
            }
        }

        private async Task ExecuteWithErrorHandling(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                LogEventProcessingError(ex);
                throw;
            }
        }

        private void LogListenerStarting()
            => Log.Information("Starting listener for event type: {EventType}", _eventTypeName);

        private void LogListenerStarted()
            => Log.Information("Listener started for event type: {EventType}", _eventTypeName);

        private void LogListenerStopping()
            => Log.Information("Stopping listener for event type: {EventType}", _eventTypeName);

        private void LogListenerStopped()
            => Log.Information("Listener stopped for event type: {EventType}", _eventTypeName);

        private void LogEventProcessingStarted()
            => Log.Information("Processing event of type: {EventType}", _eventTypeName);

        private void LogEventProcessingCompleted()
            => Log.Information("Event processed successfully: {EventType}", _eventTypeName);

        private void LogEventProcessingError(Exception ex)
            => Log.Error(ex, "Error processing event of type: {EventType}", _eventTypeName);
    }
}
