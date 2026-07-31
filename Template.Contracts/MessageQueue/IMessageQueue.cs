using Template.Contracts.Event;

namespace Template.Contracts.MessageQueue
{
    public interface IMessageQueue
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
        Task ConnectAsync();
        Task DisconnectAsync();
    }
}
