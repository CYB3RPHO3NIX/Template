using Serilog;
using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// In-memory message queue manager for development and testing.
    /// Maintains in-memory queue registry without actual broker operations.
    /// </summary>
    public class InMemoryMessageQueueManager : IMessageQueueManager
    {
        private static readonly HashSet<string> _queues = new();

        public Task CreateQueueIfNotExistsAsync(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            if (_queues.Add(queueName))
                Log.Information("Created in-memory queue: {QueueName}", queueName);
            else
                Log.Debug("Queue already exists: {QueueName}", queueName);

            return Task.CompletedTask;
        }

        public Task<bool> QueueExistsAsync(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            return Task.FromResult(_queues.Contains(queueName));
        }

        public Task DeleteQueueAsync(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

            if (_queues.Remove(queueName))
                Log.Information("Deleted in-memory queue: {QueueName}", queueName);
            else
                Log.Warning("Queue not found for deletion: {QueueName}", queueName);

            return Task.CompletedTask;
        }
    }
}
