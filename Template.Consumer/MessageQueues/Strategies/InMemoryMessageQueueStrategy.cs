using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues.Strategies
{
    public class InMemoryMessageQueueStrategy : IMessageQueueStrategy
    {
        public IMessageQueue CreateMessageQueue(string? connectionString)
        {
            return new InMemoryMessageQueue();
        }
    }
}
