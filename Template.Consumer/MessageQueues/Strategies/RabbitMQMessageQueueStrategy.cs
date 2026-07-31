using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues.Strategies
{
    public class RabbitMQMessageQueueStrategy : IMessageQueueStrategy
    {
        public IMessageQueue CreateMessageQueue(string? connectionString)
        {
            ValidateConnectionString(connectionString);
            return new RabbitMQMessageQueue(connectionString!);
        }

        private static void ValidateConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("RabbitMQ connection string is required", nameof(connectionString));
        }
    }
}
