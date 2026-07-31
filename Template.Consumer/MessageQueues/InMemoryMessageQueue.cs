using Serilog;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    public class InMemoryMessageQueue : IMessageQueue
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private bool _isConnected = false;

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
            if (!_isConnected)
            {
                throw new InvalidOperationException("Message queue is not connected");
            }

            var eventType = typeof(TEvent);
            Log.Information("Publishing event: {EventType}", eventType.Name);

            if (_subscribers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    if (handler is Func<TEvent, Task> typedHandler)
                    {
                        _ = typedHandler.Invoke(@event);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            Log.Information("Subscribing to event: {EventType}", eventType.Name);

            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            _subscribers[eventType].Add(handler);
            return Task.CompletedTask;
        }
    }
}
