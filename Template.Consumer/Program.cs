using Serilog;
using Template.Consumer;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Consumer service starting...");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            // Configure which message queue to use
            // Change this to use different implementations:
            // - MessageQueueType.InMemory (for development/testing)
            // - MessageQueueType.RabbitMQ (production with RabbitMQ)
            // - MessageQueueType.Kafka (production with Kafka)
            // - MessageQueueType.ServiceBus (production with Azure Service Bus)

            var messageQueueType = Enum.Parse<MessageQueueType>(
                context.Configuration["MessageQueue:Type"] ?? "InMemory");

            var connectionString = context.Configuration["MessageQueue:ConnectionString"];

            services.AddEventConsumer(messageQueueType, connectionString);
        })
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Consumer service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
