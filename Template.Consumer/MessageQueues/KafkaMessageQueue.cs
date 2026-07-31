using Serilog;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// Apache Kafka Message Queue Implementation
    /// TODO: Install NuGet: Confluent.Kafka
    /// </summary>
    public class KafkaMessageQueue : IMessageQueue
    {
        private readonly string _brokerConnectionString;
        // private IProducer<string, string> _producer;
        // private IConsumer<string, string> _consumer;

        public KafkaMessageQueue(string brokerConnectionString)
        {
            _brokerConnectionString = brokerConnectionString ?? throw new ArgumentNullException(nameof(brokerConnectionString));
        }

        public Task ConnectAsync()
        {
            Log.Information("Connecting to Kafka: {BrokerConnectionString}", _brokerConnectionString);

            // TODO: Implement Kafka connection logic
            // var producerConfig = new ProducerConfig { BootstrapServers = _brokerConnectionString };
            // _producer = new ProducerBuilder<string, string>(producerConfig).Build();

            Log.Information("Connected to Kafka");
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            Log.Information("Disconnecting from Kafka");

            // TODO: Implement Kafka disconnection logic
            // _producer?.Dispose();
            // _consumer?.Dispose();

            Log.Information("Disconnected from Kafka");
            return Task.CompletedTask;
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Log.Information("Publishing to Kafka: {EventType}", typeof(TEvent).Name);

            // TODO: Serialize @event and publish to Kafka topic

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Log.Information("Subscribing to Kafka: {EventType}", typeof(TEvent).Name);

            // TODO: Subscribe to Kafka topic
            // Deserialize messages and call handler

            return Task.CompletedTask;
        }
    }
}
