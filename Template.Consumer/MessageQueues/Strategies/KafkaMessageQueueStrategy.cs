using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues.Strategies
{
    public class KafkaMessageQueueStrategy : IMessageQueueStrategy
    {
        public IMessageQueue CreateMessageQueue(string? connectionString)
        {
            ValidateConnectionString(connectionString);
            return new KafkaMessageQueue(connectionString!);
        }

        private static void ValidateConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Kafka connection string is required", nameof(connectionString));
        }
    }
}
