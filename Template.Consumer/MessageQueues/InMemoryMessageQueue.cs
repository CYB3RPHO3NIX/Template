using Serilog;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// In-memory message queue for development and testing.
    /// Single Responsibility: manage in-memory event subscriptions.
    /// Thread-safe for single-process scenarios.
    /// </summary>
    public class InMemoryMessageQueue : IMessageQueue
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private bool _isConnected;

        public Task ConnectAsync()
        {
            _isConnected = true;
            Log.Information("InMemoryMessageQueue connected");
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            _isConnected = false;
            _subscribers.Clear();
            Log.Information("InMemoryMessageQueue disconnected");
            return Task.CompletedTask;
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            EnsureConnected();

            var eventType = typeof(TEvent);
            LogEventPublished(eventType.Name);

            PublishToSubscribers<TEvent>(@event, eventType);

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            LogSubscribed(eventType.Name);

            AddSubscriber(eventType, handler);

            return Task.CompletedTask;
        }

        private void EnsureConnected()
        {
            if (!_isConnected)
                throw new InvalidOperationException("Message queue is not connected");
        }

        private void PublishToSubscribers<TEvent>(TEvent @event, Type eventType) where TEvent : IEvent
        {
            if (_subscribers.TryGetValue(eventType, out var handlers))
            {
                InvokeHandlers<TEvent>(@event, handlers);
            }
        }

        private static void InvokeHandlers<TEvent>(TEvent @event, List<Delegate> handlers) where TEvent : IEvent
        {
            foreach (var handler in handlers.OfType<Func<TEvent, Task>>())
            {
                _ = handler.Invoke(@event);
            }
        }

        private void AddSubscriber<TEvent>(Type eventType, Func<TEvent, Task> handler) where TEvent : IEvent
        {
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            _subscribers[eventType].Add(handler);
        }

        private static void LogEventPublished(string eventTypeName)
            => Log.Information("Publishing event: {EventType}", eventTypeName);

        private static void LogSubscribed(string eventTypeName)
            => Log.Information("Subscribing to event: {EventType}", eventTypeName);
    }
}
