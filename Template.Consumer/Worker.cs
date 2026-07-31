using Serilog;
using Template.Contracts.MessageQueue;

namespace Template.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IEnumerable<IEventListener> _eventListeners;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IMessageQueue messageQueue,
            IEnumerable<IEventListener> eventListeners,
            ILogger<Worker> logger)
        {
            _messageQueue = messageQueue;
            _eventListeners = eventListeners;
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer service starting...");

            try
            {
                // Connect to message queue
                await _messageQueue.ConnectAsync();
                Log.Information("Message queue connected");

                // Start all event listeners
                foreach (var listener in _eventListeners)
                {
                    await listener.StartAsync(cancellationToken);
                }
                Log.Information("All event listeners started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting consumer service");
                throw;
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Consumer service running...");
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when stopping
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in consumer service");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consumer service stopping...");

            try
            {
                // Stop all event listeners
                foreach (var listener in _eventListeners)
                {
                    await listener.StopAsync(cancellationToken);
                }
                Log.Information("All event listeners stopped");

                // Disconnect from message queue
                await _messageQueue.DisconnectAsync();
                Log.Information("Message queue disconnected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping consumer service");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
