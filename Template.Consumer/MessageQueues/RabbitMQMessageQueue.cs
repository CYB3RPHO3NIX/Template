using Serilog;
using Template.Contracts.Event;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// RabbitMQ Message Queue Implementation
    /// TODO: Install NuGet: RabbitMQ.Client
    /// </summary>
    public class RabbitMQMessageQueue : IMessageQueue
    {
        private readonly string _connectionString;
        // private IConnection _connection;
        // private IModel _channel;

        public RabbitMQMessageQueue(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public Task ConnectAsync()
        {
            Log.Information("Connecting to RabbitMQ: {ConnectionString}", _connectionString);

            // TODO: Implement RabbitMQ connection logic
            // var factory = new ConnectionFactory() { Uri = new Uri(_connectionString) };
            // _connection = factory.CreateConnection();
            // _channel = _connection.CreateModel();

            Log.Information("Connected to RabbitMQ");
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            Log.Information("Disconnecting from RabbitMQ");

            // TODO: Implement RabbitMQ disconnection logic
            // _channel?.Close();
            // _connection?.Close();

            Log.Information("Disconnected from RabbitMQ");
            return Task.CompletedTask;
        }

        public Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Log.Information("Publishing to RabbitMQ: {EventType}", typeof(TEvent).Name);

            // TODO: Serialize @event and publish to RabbitMQ exchange

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            Log.Information("Subscribing to RabbitMQ: {EventType}", typeof(TEvent).Name);

            // TODO: Create queue, bind to exchange, and set up consumer
            // Deserialize messages and call handler

            return Task.CompletedTask;
        }
    }
}
