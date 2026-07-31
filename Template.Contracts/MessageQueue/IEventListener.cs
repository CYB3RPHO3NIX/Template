namespace Template.Contracts.MessageQueue
{
    public interface IEventListener
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
