using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.MessageQueue;
using Template.Consumer.Listeners;
using Template.Consumer.MessageQueues;

namespace Template.Consumer
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add consumer services with specified message queue implementation
        /// </summary>
        public static IServiceCollection AddEventConsumer(
            this IServiceCollection services,
            MessageQueueType messageQueueType,
            string? connectionString = null)
        {
            // Register message queue based on type
            switch (messageQueueType)
            {
                case MessageQueueType.InMemory:
                    services.AddSingleton<IMessageQueue, InMemoryMessageQueue>();
                    break;

                case MessageQueueType.RabbitMQ:
                    if (string.IsNullOrEmpty(connectionString))
                        throw new ArgumentException("RabbitMQ connection string is required");
                    services.AddSingleton<IMessageQueue>(new RabbitMQMessageQueue(connectionString));
                    break;

                case MessageQueueType.Kafka:
                    if (string.IsNullOrEmpty(connectionString))
                        throw new ArgumentException("Kafka connection string is required");
                    services.AddSingleton<IMessageQueue>(new KafkaMessageQueue(connectionString));
                    break;

                case MessageQueueType.ServiceBus:
                    if (string.IsNullOrEmpty(connectionString))
                        throw new ArgumentException("Service Bus connection string is required");
                    services.AddSingleton<IMessageQueue>(new ServiceBusMessageQueue(connectionString));
                    break;

                default:
                    throw new ArgumentException($"Unsupported message queue type: {messageQueueType}");
            }

            // Register event listeners
            services.AddScoped<IEventListener, UserCreatedEventListener>();

            // TODO: Add more event listeners here as you create new events
            // services.AddScoped<IEventListener, ProductCreatedEventListener>();
            // services.AddScoped<IEventListener, OrderProcessedEventListener>();

            // Register worker service
            services.AddHostedService<Worker>();

            return services;
        }
    }

    public enum MessageQueueType
    {
        InMemory,
        RabbitMQ,
        Kafka,
        ServiceBus
    }
}
