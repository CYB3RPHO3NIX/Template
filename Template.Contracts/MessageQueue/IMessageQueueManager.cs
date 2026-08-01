namespace Template.Contracts.MessageQueue
{
    /// <summary>
    /// Broker-agnostic interface for managing message queue/topic creation.
    /// Enables automatic queue provisioning across different message brokers.
    /// </summary>
    public interface IMessageQueueManager
    {
        /// <summary>
        /// Creates a queue/topic if it doesn't already exist.
        /// Idempotent: safe to call multiple times.
        /// </summary>
        Task CreateQueueIfNotExistsAsync(string queueName);

        /// <summary>
        /// Checks if a queue/topic exists in the broker.
        /// </summary>
        Task<bool> QueueExistsAsync(string queueName);

        /// <summary>
        /// Deletes a queue/topic from the broker.
        /// Use with caution in production.
        /// </summary>
        Task DeleteQueueAsync(string queueName);
    }
}
