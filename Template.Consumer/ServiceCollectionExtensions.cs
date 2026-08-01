using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Template.Contracts.MessageQueue;
using Template.Consumer.Configuration;
using Template.Consumer.Listeners;
using Template.Consumer.MessageQueues;
using Template.Consumer.Services;

namespace Template.Consumer
{
    /// <summary>
    /// Dependency Injection configuration for consumer services.
    /// Follows: Dependency Inversion, Strategy, and Factory patterns.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register all consumer services using configuration-driven approach.
        /// No switch/if-else statements - uses Strategy pattern polymorphism.
        /// </summary>
        public static IServiceCollection AddEventConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            var messageQueueConfig = BindMessageQueueConfiguration(configuration);

            RegisterMessageQueue(services, messageQueueConfig);
            RegisterMessageQueueManager(services, messageQueueConfig);
            RegisterQueueInitializationService(services);
            RegisterEventListeners(services);
            RegisterWorkerService(services);

            return services;
        }

        private static MessageQueueConfiguration BindMessageQueueConfiguration(IConfiguration configuration)
        {
            var config = new MessageQueueConfiguration();
            configuration.GetSection("MessageQueue").Bind(config);
            LogConfigurationLoaded(config);
            return config;
        }

        private static void RegisterMessageQueue(
            IServiceCollection services,
            MessageQueueConfiguration config)
        {
            var factory = new MessageQueueStrategyFactory();
            var messageQueue = factory.CreateMessageQueue(config.Type, config.ConnectionString);

            services.AddSingleton<IMessageQueue>(messageQueue);
        }

        private static void RegisterMessageQueueManager(
            IServiceCollection services,
            MessageQueueConfiguration config)
        {
            var factory = new MessageQueueStrategyFactory();
            var queueManager = factory.CreateMessageQueueManager(config.Type, config.ConnectionString);

            services.AddSingleton<IMessageQueueManager>(queueManager);
        }

        private static void RegisterQueueInitializationService(IServiceCollection services)
        {
            services.AddSingleton<IQueueInitializationService, QueueInitializationService>();
        }

        private static void RegisterEventListeners(IServiceCollection services)
        {
            services.AddScoped<IEventListener, UserCreatedEventListener>();

            // TODO: Add more event listeners here as you create new events
            // services.AddScoped<IEventListener, ProductCreatedEventListener>();
            // services.AddScoped<IEventListener, OrderProcessedEventListener>();
        }

        private static void RegisterWorkerService(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }

        private static void LogConfigurationLoaded(MessageQueueConfiguration config)
        {
            Log.Information("Message Queue Configuration Loaded - Type: {Type}", config.Type);
        }
    }
}
