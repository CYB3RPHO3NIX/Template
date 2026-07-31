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
            // Configuration-driven approach
            // Set MessageQueue:Type and MessageQueue:ConnectionString in appsettings.json
            // Supported types: InMemory, RabbitMQ, Kafka, ServiceBus
            services.AddEventConsumer(context.Configuration);
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
