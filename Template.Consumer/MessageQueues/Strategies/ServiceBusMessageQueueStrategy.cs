using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues.Strategies
{
    public class ServiceBusMessageQueueStrategy : IMessageQueueStrategy
    {
        public IMessageQueue CreateMessageQueue(string? connectionString)
        {
            ValidateConnectionString(connectionString);
            return new ServiceBusMessageQueue(connectionString!);
        }

        private static void ValidateConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Service Bus connection string is required", nameof(connectionString));
        }
    }
}
