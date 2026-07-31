using Template.Contracts.MessageQueue;

namespace Template.Consumer.MessageQueues
{
    /// <summary>
    /// Strategy interface for creating message queue implementations.
    /// Follows Strategy and Factory patterns for loose coupling.
    /// </summary>
    public interface IMessageQueueStrategy
    {
        IMessageQueue CreateMessageQueue(string? connectionString);
    }
}
