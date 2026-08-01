using Serilog;
using Template.Contracts.MessageQueue;
using Template.Consumer.MessageQueues.Strategies;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// Factory for creating message queue implementations and managers using Strategy pattern.
    /// Eliminates switch/if-else statements through polymorphism.
    /// Follows: Strategy, Factory, and Dependency Inversion principles.
    /// </summary>
    public class MessageQueueStrategyFactory
    {
        private readonly Dictionary<string, IMessageQueueStrategy> _strategies;
        private readonly Dictionary<string, IMessageQueueManager> _managers;

        public MessageQueueStrategyFactory()
        {
            _strategies = new Dictionary<string, IMessageQueueStrategy>(StringComparer.OrdinalIgnoreCase)
            {
                { "InMemory", new InMemoryMessageQueueStrategy() },
                { "RabbitMQ", new RabbitMQMessageQueueStrategy() },
                { "Kafka", new KafkaMessageQueueStrategy() },
                { "ServiceBus", new ServiceBusMessageQueueStrategy() }
            };

            _managers = new Dictionary<string, IMessageQueueManager>(StringComparer.OrdinalIgnoreCase);
        }

        public IMessageQueue CreateMessageQueue(string messageQueueType, string? connectionString)
        {
            ValidateInput(messageQueueType);

            var strategy = RetrieveStrategy(messageQueueType);
            LogMessageQueueCreation(messageQueueType);

            return strategy.CreateMessageQueue(connectionString);
        }

        public IMessageQueueManager CreateMessageQueueManager(string messageQueueType, string? connectionString)
        {
            ValidateInput(messageQueueType);

            var cacheKey = $"{messageQueueType}:{connectionString}";

            if (_managers.TryGetValue(cacheKey, out var cachedManager))
                return cachedManager;

            var manager = messageQueueType.ToUpperInvariant() switch
            {
                "INMEMORY" => new InMemoryMessageQueueManager(),
                "RABBITMQ" => new RabbitMQMessageQueueManager(connectionString ?? throw new ArgumentException("RabbitMQ connection string is required")),
                "KAFKA" => new KafkaMessageQueueManager(connectionString ?? throw new ArgumentException("Kafka connection string is required")),
                "SERVICEBUS" => new ServiceBusMessageQueueManager(connectionString ?? throw new ArgumentException("Service Bus connection string is required")),
                _ => throw new InvalidOperationException(
                    $"Unsupported message queue type: '{messageQueueType}'. " +
                    $"Supported types: {string.Join(", ", GetSupportedMessageQueueTypes())}")
            };

            _managers[cacheKey] = manager;
            LogMessageQueueManagerCreation(messageQueueType);

            return manager;
        }

        public bool IsValidMessageQueueType(string messageQueueType)
        {
            return _strategies.ContainsKey(messageQueueType);
        }

        public IEnumerable<string> GetSupportedMessageQueueTypes()
        {
            return _strategies.Keys;
        }

        private void ValidateInput(string messageQueueType)
        {
            if (string.IsNullOrWhiteSpace(messageQueueType))
                throw new ArgumentException("Message queue type cannot be null or empty", nameof(messageQueueType));
        }

        private IMessageQueueStrategy RetrieveStrategy(string messageQueueType)
        {
            return _strategies.TryGetValue(messageQueueType, out var strategy)
                ? strategy
                : throw new InvalidOperationException(
                    $"Unsupported message queue type: '{messageQueueType}'. " +
                    $"Supported types: {string.Join(", ", GetSupportedMessageQueueTypes())}");
        }

        private static void LogMessageQueueCreation(string messageQueueType)
        {
            Log.Information("Creating message queue of type: {MessageQueueType}", messageQueueType);
        }

        private static void LogMessageQueueManagerCreation(string messageQueueType)
        {
            Log.Information("Creating message queue manager of type: {MessageQueueType}", messageQueueType);
        }
    }
}
