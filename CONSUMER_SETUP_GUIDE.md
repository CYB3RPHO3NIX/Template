# Consumer Service Setup Guide

## Overview

The `Template.Consumer` is a flexible background service that listens to events published by the API and processes them asynchronously. It supports multiple message queue implementations (RabbitMQ, Kafka, Azure Service Bus, or in-memory for development).

## Architecture

```
API publishes events
         ↓
IMessageQueue abstraction
         ↓
Message Queue (RabbitMQ/Kafka/Service Bus/InMemory)
         ↓
Consumer Worker Service
         ↓
Event Listeners (UserCreatedEventListener, etc.)
         ↓
Process events (send emails, update external systems, etc.)
```

## Message Queue Implementations

### 1. **InMemory** (Development/Testing)
- No external dependencies
- Events stored in memory
- Perfect for local development
- Not for production

**Configuration:**
```json
{
  "MessageQueue": {
    "Type": "InMemory",
    "ConnectionString": ""
  }
}
```

### 2. **RabbitMQ** (Production)
- Robust message queuing
- Install: `dotnet add package RabbitMQ.Client`

**Configuration:**
```json
{
  "MessageQueue": {
    "Type": "RabbitMQ",
    "ConnectionString": "amqp://guest:guest@localhost:5672/"
  }
}
```

**Docker:**
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```

### 3. **Kafka** (Production - High Throughput)
- Distributed event streaming
- Install: `dotnet add package Confluent.Kafka`

**Configuration:**
```json
{
  "MessageQueue": {
    "Type": "Kafka",
    "ConnectionString": "localhost:9092"
  }
}
```

### 4. **Azure Service Bus** (Production - Cloud)
- Azure native messaging
- Install: `dotnet add package Azure.Messaging.ServiceBus`

**Configuration:**
```json
{
  "MessageQueue": {
    "Type": "ServiceBus",
    "ConnectionString": "Endpoint=sb://yournamespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR_KEY"
  }
}
```

## How It Works

### 1. Start the Consumer
```bash
cd Template.Consumer
dotnet run
```

### 2. Create Event Listeners
Create a new listener by inheriting from `BaseEventListener<TEvent>`:

```csharp
public class ProductCreatedEventListener : BaseEventListener<ProductCreatedEvent>
{
    public ProductCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
    {
    }

    protected override async Task HandleEventAsync(ProductCreatedEvent @event)
    {
        // TODO: Handle product created event
        // Example: Send notification, update external system
        
        Log.Information("Processing ProductCreatedEvent: ProductId={ProductId}", @event.ProductId);
        await Task.Delay(100);
    }
}
```

### 3. Register the Listener
In `ServiceCollectionExtensions.cs`, add:

```csharp
services.AddScoped<IEventListener, ProductCreatedEventListener>();
```

### 4. Events Flow Automatically
```
API Command (CreateProduct)
    ↓
Command Handler executes
    ↓
SaveChangesAsync()
    ↓
Publishes ProductCreatedEvent via _bus.Publish()
    ↓
Message Queue receives event
    ↓
Consumer picks it up
    ↓
ProductCreatedEventListener.HandleEventAsync() called
    ↓
Event processed
```

## File Structure

```
Template.Consumer/
├── Listeners/
│   ├── BaseEventListener.cs              (base class for all listeners)
│   └── UserCreatedEventListener.cs       (example listener)
├── MessageQueues/
│   ├── InMemoryMessageQueue.cs           (development/testing)
│   ├── RabbitMQMessageQueue.cs           (production - TODO: implement)
│   ├── KafkaMessageQueue.cs              (production - TODO: implement)
│   └── ServiceBusMessageQueue.cs         (production - TODO: implement)
├── Worker.cs                             (background service)
├── ServiceCollectionExtensions.cs        (dependency injection setup)
├── Program.cs                            (entry point)
├── appsettings.json                      (default config)
├── appsettings.Development.json          (dev config)
└── appsettings.Production.json           (production config)
```

## Configuration Examples

### Development (InMemory)
```json
{
  "MessageQueue": {
    "Type": "InMemory",
    "ConnectionString": ""
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

### Production (RabbitMQ)
```json
{
  "MessageQueue": {
    "Type": "RabbitMQ",
    "ConnectionString": "amqp://user:password@rabbitmq-host:5672/"
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### Production (Kafka)
```json
{
  "MessageQueue": {
    "Type": "Kafka",
    "ConnectionString": "kafka-host:9092"
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### Production (Service Bus)
```json
{
  "MessageQueue": {
    "Type": "ServiceBus",
    "ConnectionString": "Endpoint=sb://namespace.servicebus.windows.net/;..."
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

## Implementation Roadmap

### ✅ Completed
- [x] Message queue abstraction (IMessageQueue)
- [x] Event listener base class (BaseEventListener<T>)
- [x] Worker service (background service runner)
- [x] InMemory implementation (for testing)
- [x] RabbitMQ stub (ready for implementation)
- [x] Kafka stub (ready for implementation)
- [x] Service Bus stub (ready for implementation)
- [x] Example listener (UserCreatedEventListener)
- [x] Configuration support

### 📋 To Implement When Needed
1. **RabbitMQ Implementation**
   - Install RabbitMQ.Client NuGet
   - Implement PublishAsync, SubscribeAsync, ConnectAsync, DisconnectAsync
   - Set up exchanges, queues, and bindings

2. **Kafka Implementation**
   - Install Confluent.Kafka NuGet
   - Implement producer for publishing
   - Implement consumer group for subscribing
   - Handle deserialization

3. **Service Bus Implementation**
   - Install Azure.Messaging.ServiceBus NuGet
   - Implement topic sender for publishing
   - Implement subscription receiver for subscribing

## Running Multiple Instances

You can run multiple consumer instances for scalability:

```bash
# Terminal 1
dotnet run --configuration Production

# Terminal 2
dotnet run --configuration Production

# Terminal 3
dotnet run --configuration Production
```

All instances will consume from the same message queue without duplication (assuming proper consumer group configuration in RabbitMQ/Kafka/Service Bus).

## Error Handling

All listeners include:
- Exception logging with TraceId
- Structured logging for diagnostics
- Automatic retry support (when implemented in message queue)

## Monitoring

Monitor the consumer service:

```bash
# Check if service is running
ps aux | grep "Template.Consumer"

# View logs (if using file sink)
tail -f logs/consumer.log
```

## Deployment

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Template.Consumer.dll"]
```

### Kubernetes
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: template-consumer
spec:
  replicas: 3
  selector:
    matchLabels:
      app: template-consumer
  template:
    metadata:
      labels:
        app: template-consumer
    spec:
      containers:
      - name: consumer
        image: template-consumer:latest
        env:
        - name: MessageQueue__Type
          value: "RabbitMQ"
        - name: MessageQueue__ConnectionString
          valueFrom:
            secretKeyRef:
              name: message-queue
              key: connection-string
```

## Next Steps

1. Choose your message queue implementation
2. Install required NuGet package
3. Implement the message queue class
4. Configure connection string in appsettings
5. Create event listeners for your domain events
6. Test with the API
7. Deploy to production
