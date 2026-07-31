using Serilog;
using Template.Contracts.MessageQueue;

namespace Template.Consumer
{
    /// <summary>
    /// Background service that manages event processing lifecycle.
    /// Single Responsibility: orchestrate message queue and event listeners.
    /// Dependency Inversion: depends on abstractions (IMessageQueue, IEventListener).
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IEnumerable<IEventListener> _eventListeners;
        private readonly ILogger<Worker> _logger;
        private const int HealthCheckIntervalMs = 5000;

        public Worker(
            IMessageQueue messageQueue,
            IEnumerable<IEventListener> eventListeners,
            ILogger<Worker> logger)
        {
            _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
            _eventListeners = eventListeners ?? throw new ArgumentNullException(nameof(eventListeners));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            LogServiceStarting();

            await ConnectMessageQueue();
            await StartAllEventListeners(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunHealthCheckLoop(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogServiceStopping();

            await StopAllEventListeners(cancellationToken);
            await DisconnectMessageQueue();

            await base.StopAsync(cancellationToken);
        }

        private async Task ConnectMessageQueue()
        {
            await _messageQueue.ConnectAsync();
            Log.Information("Message queue connected");
        }

        private async Task StartAllEventListeners(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_eventListeners
                .Select(listener => listener.StartAsync(cancellationToken)));

            Log.Information("All event listeners started");
        }

        private async Task DisconnectMessageQueue()
        {
            await _messageQueue.DisconnectAsync();
            Log.Information("Message queue disconnected");
        }

        private async Task StopAllEventListeners(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_eventListeners
                .Select(listener => listener.StopAsync(cancellationToken)));

            Log.Information("All event listeners stopped");
        }

        private async Task RunHealthCheckLoop(CancellationToken stoppingToken)
        {
            while (stoppingToken.IsCancellationRequested == false)
            {
                try
                {
                    _logger.LogDebug("Consumer service running...");
                    await Task.Delay(HealthCheckIntervalMs, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in consumer service");
                }
            }
        }

        private void LogServiceStarting()
            => _logger.LogInformation("Consumer service starting...");

        private void LogServiceStopping()
            => _logger.LogInformation("Consumer service stopping...");
    }
}
