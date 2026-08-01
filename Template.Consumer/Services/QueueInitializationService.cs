using Serilog;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.Services
{
    /// <summary>
    /// Service for initializing message queues on application startup.
    /// Automatically creates queues/topics for all registered event listeners.
    /// Works broker-agnostically across InMemory, RabbitMQ, Kafka, and Service Bus.
    /// </summary>
    public interface IQueueInitializationService
    {
        /// <summary>
        /// Creates all registered event queues if they don't exist.
        /// Called during Consumer service startup.
        /// </summary>
        Task InitializeQueuesAsync(IEnumerable<string> queueNames);

        /// <summary>
        /// Creates a single queue by name.
        /// </summary>
        Task CreateQueueAsync(string queueName);
    }

    public class QueueInitializationService : IQueueInitializationService
    {
        private readonly IMessageQueueManager _queueManager;

        public QueueInitializationService(IMessageQueueManager queueManager)
        {
            _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
        }

        public async Task InitializeQueuesAsync(IEnumerable<string> queueNames)
        {
            if (queueNames == null || !queueNames.Any())
            {
                Log.Warning("No queues to initialize");
                return;
            }

            Log.Information("Initializing {QueueCount} message queues", queueNames.Count());

            var tasks = queueNames.Select(queueName => CreateQueueWithErrorHandlingAsync(queueName));
            await Task.WhenAll(tasks);

            Log.Information("Queue initialization completed successfully");
        }

        public async Task CreateQueueAsync(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            await _queueManager.CreateQueueIfNotExistsAsync(queueName);
        }

        private async Task CreateQueueWithErrorHandlingAsync(string queueName)
        {
            try
            {
                Log.Debug("Creating queue: {QueueName}", queueName);
                await _queueManager.CreateQueueIfNotExistsAsync(queueName);
                Log.Information("Queue created/verified: {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create queue: {QueueName}. This may cause event processing to fail.", queueName);
                throw;
            }
        }
    }
}
