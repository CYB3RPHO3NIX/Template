namespace Template.Consumer.Configuration
{
    /// <summary>
    /// Configuration for message queue settings.
    /// Single Responsibility: holds message queue configuration.
    /// </summary>
    public class MessageQueueConfiguration
    {
        public string Type { get; set; } = "InMemory";
        public string? ConnectionString { get; set; }
    }
}
