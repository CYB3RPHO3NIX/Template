using Serilog;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// Azure Service Bus Message Queue Implementation
    /// TODO: Install NuGet: Azure.Messaging.ServiceBus
    /// </summary>
    public class ServiceBusMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;
        // private ServiceBusClient _client;

        public ServiceBusMessageQueue(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task ConnectAsync()
        {
            Log.Information("Connecting to Azure Service Bus");

            // TODO: Implement Service Bus connection logic
            // _client = new ServiceBusClient(_connectionString);

            Log.Information("Connected to Azure Service Bus");
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            Log.Information("Disconnecting from Azure Service Bus");

            // TODO: Implement Service Bus disconnection logic
            // await _client?.DisposeAsync();

            Log.Information("Disconnected from Azure Service Bus");
            return Task.CompletedTask;
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Log.Information("Publishing to Service Bus: {EventType}", typeof(TEvent).Name);

            // TODO: Serialize @event and send to Service Bus topic

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Log.Information("Subscribing to Service Bus: {EventType}", typeof(TEvent).Name);

            // TODO: Subscribe to Service Bus topic
            // Deserialize messages and call handler

            return Task.CompletedTask;
        }
    }
}
