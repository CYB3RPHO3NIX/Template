# Message Queue Auto-Initialization Guide

## Overview

The Consumer service now automatically creates message queues (or topics) when it starts up, across all supported brokers. This eliminates manual queue provisioning and ensures consistency across environments.

**Key Benefits:**
- ✅ Broker-agnostic (works with InMemory, RabbitMQ, Kafka, Service Bus)
- ✅ Automatic queue creation on startup (idempotent)
- ✅ Supports multiple event listeners with different queues
- ✅ Consistent queue naming across all brokers
- ✅ Fails fast with clear error messages

## Architecture

### Components

1. **IMessageQueueManager** - Broker-agnostic abstraction for queue operations
   - `CreateQueueIfNotExistsAsync(queueName)` - Create or verify queue
   - `QueueExistsAsync(queueName)` - Check if queue exists
   - `DeleteQueueAsync(queueName)` - Delete a queue

2. **Broker Implementations**
   - `InMemoryMessageQueueManager` - In-memory registry (dev/test)
   - `RabbitMQMessageQueueManager` - RabbitMQ broker
   - `KafkaMessageQueueManager` - Apache Kafka
   - `ServiceBusMessageQueueManager` - Azure Service Bus

3. **IQueueInitializationService** - Orchestrates queue creation
   - `InitializeQueuesAsync(queueNames)` - Bulk create queues
   - `CreateQueueAsync(queueName)` - Single queue creation

4. **QueueNamingUtility** - Consistent naming conventions
   - Converts event listener class names to queue names
   - Examples: `UserCreatedEventListener` → `user-created-events`

## How It Works

### Startup Flow

```
Consumer Service Starts
    ↓
Connect to Message Broker
    ↓
Initialize Queues (IQueueInitializationService)
    ├─ Extract queue names from registered event listeners
    ├─ Create each queue if it doesn't exist
    └─ Log creation status
    ↓
Start All Event Listeners
```

### Queue Naming Convention

Event listeners are automatically converted to queue names using kebab-case:

| Listener Class | Queue Name |
|---|---|
| `UserCreatedEventListener` | `user-created-events` |
| `OrderProcessedEventListener` | `order-processed-events` |
| `ProductInventoryEventListener` | `product-inventory-events` |

You can also use `QueueNamingUtility` for custom naming:

```csharp
// From listener type
var queueName = QueueNamingUtility.GetQueueNameFromListenerType(typeof(UserCreatedEventListener));
// Result: "user-created-events"

// From event type
var queueName = QueueNamingUtility.GetQueueNameFromEventType(typeof(UserCreatedEvent));
// Result: "user-created"

// Custom feature name
var queueName = QueueNamingUtility.GetQueueNameForFeature("Payments");
// Result: "payments-events"
```

## Configuration

Configuration is already wired in `ServiceCollectionExtensions.cs`. The system uses:

1. **MessageQueueConfiguration** - Loaded from `appsettings.json`
   ```json
   {
     "MessageQueue": {
       "Type": "RabbitMQ",
       "ConnectionString": "amqp://user:pass@localhost/"
     }
   }
   ```

2. **MessageQueueStrategyFactory** - Creates both queue and manager instances
   - Polymorphic strategy pattern handles all broker types
   - Single configuration drives both publishing and initialization

3. **Dependency Injection** - Registered in `AddEventConsumer()`
   ```csharp
   services.AddSingleton<IMessageQueueManager>(queueManager);
   services.AddSingleton<IQueueInitializationService, QueueInitializationService>();
   services.AddHostedService<Worker>();
   ```

## Adding a New Event Listener

When you add a new event listener:

1. Create your listener (e.g., `ProductCreatedEventListener`)
2. Register it in `ServiceCollectionExtensions.cs`:
   ```csharp
   services.AddScoped<IEventListener, ProductCreatedEventListener>();
   ```
3. Restart the Consumer service
4. **That's it!** The queue is automatically created before the listener starts

## Usage Examples

### Example 1: Development with InMemory

No configuration needed - queues are created in-memory automatically.

```json
{
  "MessageQueue": {
    "Type": "InMemory"
  }
}
```

### Example 2: Production with RabbitMQ

```json
{
  "MessageQueue": {
    "Type": "RabbitMQ",
    "ConnectionString": "amqp://guest:guest@rabbitmq-prod:5672/"
  }
}
```

**Queues created automatically:**
- `user-created-events`
- `order-processed-events`
- (any other registered listener)

### Example 3: Kafka Cluster

```json
{
  "MessageQueue": {
    "Type": "Kafka",
    "ConnectionString": "broker1:9092,broker2:9092,broker3:9092"
  }
}
```

**Topics created with:**
- 3 partitions (default)
- Replication factor: 1 (configurable in implementation)

### Example 4: Azure Service Bus

```json
{
  "MessageQueue": {
    "Type": "ServiceBus",
    "ConnectionString": "Endpoint=sb://myns.servicebus.windows.net/;..."
  }
}
```

## Extending the System

### Adding a New Broker

To support a new broker:

1. Create `YourBrokerMessageQueueManager : IMessageQueueManager`
   ```csharp
   public class YourBrokerMessageQueueManager : IMessageQueueManager
   {
       public async Task CreateQueueIfNotExistsAsync(string queueName) { }
       public async Task<bool> QueueExistsAsync(string queueName) { }
       public async Task DeleteQueueAsync(string queueName) { }
   }
   ```

2. Register in `MessageQueueStrategyFactory.CreateMessageQueueManager()`
   ```csharp
   var manager = messageQueueType.ToUpperInvariant() switch
   {
       // ... existing cases
       "YOURBROKER" => new YourBrokerMessageQueueManager(connectionString),
       // ...
   };
   ```

3. Add appsettings entry to configuration docs

### Custom Queue Names

Override `InitializeQueues()` in `Worker` for custom queue names:

```csharp
private async Task InitializeQueues()
{
    var customQueueNames = new List<string>
    {
        "custom-queue-1",
        "custom-queue-2",
        "legacy-queue-name"
    };

    await _queueInitializationService.InitializeQueuesAsync(customQueueNames);
}
```

### Conditional Queue Creation

Skip initialization for certain environments:

```csharp
private async Task InitializeQueues()
{
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    
    if (env == "Development")
    {
        Log.Information("Skipping queue initialization in Development");
        return;
    }

    // ... proceed with initialization
}
```

## Error Handling

### Scenarios

1. **Queue already exists** → Logged as debug, safe to proceed
2. **Connection failure** → Logged as error, consumer startup fails
3. **Broker doesn't support operation** → Exception with clear message
4. **Invalid configuration** → ArgumentException during DI registration

### Troubleshooting

If queues aren't being created:

1. Check `appsettings.json` - verify `MessageQueue.Type` and `ConnectionString`
2. Check logs for `IQueueInitializationService` entries
3. Verify broker connectivity manually
4. Check event listener registrations in `ServiceCollectionExtensions.cs`

## Testing

### Unit Testing

```csharp
[TestMethod]
public async Task InitializeQueues_CreatesQueueForEachListener()
{
    // Arrange
    var mockManager = new Mock<IMessageQueueManager>();
    var service = new QueueInitializationService(mockManager.Object);
    
    var queueNames = new[] { "queue-1", "queue-2" };

    // Act
    await service.InitializeQueuesAsync(queueNames);

    // Assert
    mockManager.Verify(
        m => m.CreateQueueIfNotExistsAsync(It.IsAny<string>()),
        Times.Exactly(2));
}
```

### Integration Testing

Use `InMemoryMessageQueueManager` in test configuration:

```json
{
  "MessageQueue": {
    "Type": "InMemory"
  }
}
```

No broker setup needed - queues are managed in-memory.

## Performance Considerations

- **InMemory**: O(1) per queue
- **RabbitMQ**: ~100ms per queue (network dependent)
- **Kafka**: ~500ms per topic (topic creation overhead)
- **Service Bus**: ~2-3s per queue (Azure API latency)

All operations run in parallel where possible (`Task.WhenAll`).

## Migration Path

If you have existing queues:

1. Deploy consumer with auto-initialization enabled
2. New listeners will create their queues automatically
3. Existing queues remain untouched (idempotent operation)
4. No downtime required

## Security

- Connection strings are read from configuration (not hardcoded)
- Queue operations are logged (check your logs for queue names in production)
- `DeleteQueueAsync` is intentionally not called automatically (safe by default)
- Use environment-specific `appsettings.*.json` for different brokers per environment
