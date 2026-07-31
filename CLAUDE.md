# Template Project - Development Guidelines

## Project Overview

This is a .NET 10.0 enterprise application using **CQRS (Command Query Responsibility Segregation)** pattern with event-driven architecture and background processing.

### Projects:
1. **Template.API** - ASP.NET Core Web API (REST endpoints)
2. **Template.Consumer** - Background service (event processing with pluggable message queues)
3. **Core Libraries** - Commands, Queries, Events, Handlers, Database, Services, etc.

### Technologies:
- **Database**: SQL Server with Entity Framework Core
- **API**: ASP.NET Core 10.0 with Swagger/OpenAPI
- **Events**: Pub/Sub with message queue abstraction
- **Logging**: Serilog (SQL Server sink for API, Console for Consumer)
- **Messaging**: Pluggable (InMemory, RabbitMQ, Kafka, Azure Service Bus)
- **Mapping**: Mapster
- **Authentication**: Password hashing with SHA256 + salt

## Architecture Layers

### 1. **Controllers** (`Template.API/Controllers/`)
- REST API endpoints
- Route: `[Route("api/[feature]")]`
- Inject `IServiceBus` for sending commands/queries
- Validate requests using `request.Validate()` before sending
- Return appropriate HTTP status codes (Ok, NotFound, BadRequest)

### 2. **Commands** (`Template.Commands/[Feature]/[FeatureName]Commands/`)
- Define command intentions (Create, Update, Delete actions)
- All commands: inherit `ICommand<TResult>`
- Must include `Guid TraceId { get; set; }` property
- Namespace pattern: `Template.Commands.{Feature}.{FeatureName}Commands`

**Example Command:**
```csharp
public class CreateUserCommand : ICommand<Guid?>
{
    public Guid TraceId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

### 3. **Command Handlers** (`Template.CommandHandlers/[Feature]/[FeatureName]CommandHandlers/`)
- Implement `ICommandHandler<TCommand, TResult>`
- Inject `IServiceBus` and `TemplateDbContext`
- Use Serilog with `LogContext.PushProperty("TraceId", command.TraceId)`
- Log at Information level for key operations
- Can send queries via `_bus.Send<T>(query)` for checking state
- Return `null` for failure cases, actual value for success
- Always `await _dbContext.SaveChangesAsync()` after adding entities
- Publish events after SaveChangesAsync()

**Example Handler:**
```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid?>
{
    private readonly IServiceBus _bus;
    private readonly TemplateDbContext _dbContext;
    
    public CreateUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
    {
        _bus = bus;
        _dbContext = dbContext;
    }
    
    public async Task<Guid?> Handle(CreateUserCommand command)
    {
        using (LogContext.PushProperty("TraceId", command.TraceId))
        {
            Log.Information("Handling CreateUserCommand");
            
            // Business logic, database operations
            await _dbContext.SaveChangesAsync();
            
            // Publish event for async processing
            await _bus.Publish(new UserCreatedEvent { /* ... */ });
            
            return result;
        }
    }
}
```

### 4. **Queries** (`Template.Queries/[Feature]/[FeatureName]Queries/`)
- Define read operations (Get, List, Exists checks)
- All queries: inherit `IQuery<TResult>`
- Must include `Guid TraceId { get; set; }` property
- Namespace pattern: `Template.Queries.{Feature}.{FeatureName}Queries`

**Example Query:**
```csharp
public class GetUserByIdQuery : IQuery<UserDTO?>
{
    public Guid TraceId { get; set; }
    public Guid UserId { get; set; }
}
```

### 5. **Query Handlers** (`Template.QueryHandlers/[Feature]/[FeatureName]QueryHandlers/`)
- Implement `IQueryHandler<TQuery, TResult>`
- Inject `TemplateDbContext`
- Read-only operations (no SaveChangesAsync)
- Use Serilog with TraceId logging
- Return null if entity not found
- Use LINQ to fetch and filter data

**Example Handler:**
```csharp
public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDTO?>
{
    private readonly TemplateDbContext _dbContext;
    
    public GetUserByIdQueryHandler(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<UserDTO?> Handle(GetUserByIdQuery query)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserId == query.UserId);
        
        return user == null ? null : new UserDTO
        {
            Id = user.UserId,
            UserName = user.Username,
            Email = user.Email,
            IsActive = user.IsActive
        };
    }
}
```

### 6. **DTOs** (`Template.Shared.Models/DTOs/[Feature]/`)
- Used for returning data from queries
- Namespace: `Template.Shared.Models.DTOs.{Feature}`
- Naming: `{Entity}DTO`
- Properties: public get/set with initialization

### 7. **Request Models** (`Template.Shared.Models/Requests/[Feature]/`)
- Used for API request bodies
- Inherit `IRequest`
- Implement `ValidationResult Validate()` method
- Namespace: `Template.Shared.Models.Requests.{Feature}`
- Naming: `{Action}{Entity}Request`

**Example Request:**
```csharp
public class CreateUserRequest : IRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ValidationResult Validate()
    {
        var validationResult = new ValidationResult();
        
        if (string.IsNullOrWhiteSpace(UserName))
            validationResult.Errors.Add("UserName is required.");
        
        if (string.IsNullOrWhiteSpace(Email))
            validationResult.Errors.Add("Email is required.");
        
        return validationResult;
    }
}
```

### 8. **Events** (`Template.Events/[Feature]/`)
- Represent domain events that occur as a result of commands
- All events: inherit `IEvent`
- Must include `Guid TraceId { get; set; }` property
- Namespace pattern: `Template.Events.{Feature}`
- Naming: `{Entity}{Action}Event` (e.g., `UserCreatedEvent`)
- Published from command handlers after state changes

**Example Event:**
```csharp
public class UserCreatedEvent : IEvent
{
    public Guid TraceId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}
```

### 9. **Event Handlers** (`Template.EventHandlers/[Feature]/`)
- Handle domain events asynchronously in the API process
- Implement `IEventHandler<TEvent>`
- Inject `IServiceBus` and any needed services
- Use Serilog with `LogContext.PushProperty("TraceId", @event.TraceId)`
- Supports multiple handlers per event (all execute)
- Used for immediate side effects (notifications, audit, etc.)

**Example Handler:**
```csharp
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent @event)
    {
        using (LogContext.PushProperty("TraceId", @event.TraceId))
        {
            Log.Information("Handling UserCreatedEvent");
            // Send notification, create audit log, etc.
        }
    }
}
```

### 10. **Event Listeners (Consumer)** (`Template.Consumer/Listeners/`)
- Handle events asynchronously in background service
- Inherit from `BaseEventListener<TEvent>`
- Implement `HandleEventAsync()` for business logic
- Inject `IMessageQueue` for receiving events
- Used for long-running operations (emails, external integrations)
- Runs in separate process (Template.Consumer)

**Example Listener:**
```csharp
public class UserCreatedEventListener : BaseEventListener<UserCreatedEvent>
{
    public UserCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue)
    {
    }

    protected override async Task HandleEventAsync(UserCreatedEvent @event)
    {
        Log.Information("Sending welcome email to {Email}", @event.Email);
        // Send email, call external API, etc.
        await SendWelcomeEmailAsync(@event.Email);
    }
}
```

### 11. **Message Queue** (`Template.Consumer/MessageQueues/`)
- Abstraction for pluggable message broker
- Implementations: InMemory (dev), RabbitMQ, Kafka, Service Bus (prod)
- Used to decouple API from Consumer
- Configured via appsettings.json

### 12. **Database Entities** (`Template.Database/Domain/Entities/`)
- Entity Framework Core models
- Namespace: `Template.Database.Domain.Entities`
- Generated/maintained by database design
- Partial classes with relationships defined
- Audit fields: `CreatedBy`, `CreatedOn`, `UpdatedBy`, `UpdatedOn`

### 13. **Database Context** (`Template.Database/Domain/Contexts/TemplateDbContext.cs`)
- Entity Framework Core DbContext
- Contains DbSet properties for all entities
- Injected into handlers as dependency
- Connection string from configuration

## Naming Conventions

| Layer | Pattern | Example |
|-------|---------|---------|
| Command | `{Action}{Entity}Command` | `CreateUserCommand`, `UpdateProductCommand` |
| Command Handler | `{Action}{Entity}CommandHandler` | `CreateUserCommandHandler` |
| Query | `{Action}{Entity}Query` | `GetUserByIdQuery`, `ListProductsQuery` |
| Query Handler | `{Action}{Entity}QueryHandler` | `GetUserByIdQueryHandler` |
| Event | `{Entity}{Action}Event` | `UserCreatedEvent`, `ProductUpdatedEvent` |
| Event Handler (API) | `{Entity}{Action}EventHandler` | `UserCreatedEventHandler` |
| Event Listener (Consumer) | `{Entity}{Action}EventListener` | `UserCreatedEventListener` |
| DTO | `{Entity}DTO` | `UserDTO`, `ProductDTO` |
| Request | `{Action}{Entity}Request` | `CreateUserRequest`, `UpdateProductRequest` |
| Controller | `{Entity}Controller` | `IdentityController`, `ProductsController` |
| Folder (Feature) | `{Feature}` | `Identity`, `Products`, `Orders` |

## Adding a New Feature - Step-by-Step

### Example: Adding a "Product" feature

#### Step 1: Create Database Entity
- File: `Template.Database/Domain/Entities/Product.cs`
- Add DbSet in `TemplateDbContext`

#### Step 2: Create Commands
- File: `Template.Commands/Products/ProductCommands/CreateProductCommand.cs`
- File: `Template.Commands/Products/ProductCommands/UpdateProductCommand.cs`
- File: `Template.Commands/Products/ProductCommands/DeleteProductCommand.cs`

#### Step 3: Create Command Handlers
- File: `Template.CommandHandlers/Products/ProductCommandHandlers/CreateProductCommandHandler.cs`
- Register in `Template.CommandHandlers/ServiceCollectionExtensions.cs` via `RegisterCommandHandlers()`
- Publish events after SaveChangesAsync()

#### Step 4: Create Queries
- File: `Template.Queries/Products/ProductQueries/GetProductByIdQuery.cs`
- File: `Template.Queries/Products/ProductQueries/ListProductsQuery.cs`

#### Step 5: Create Query Handlers
- File: `Template.QueryHandlers/Products/ProductQueryHandlers/GetProductByIdQueryHandler.cs`
- Register in `Template.QueryHandlers/ServiceCollectionExtensions.cs` via `RegisterQueryHandlers()`

#### Step 6: Create Events (for state changes)
- File: `Template.Events/Products/ProductCreatedEvent.cs`
- File: `Template.Events/Products/ProductUpdatedEvent.cs`
- Publish from command handlers

#### Step 7: Create API Event Handlers (optional, for immediate side effects)
- File: `Template.EventHandlers/Products/ProductCreatedEventHandler.cs`
- Register in `Template.EventHandlers/ServiceCollectionExtensions.cs` via `RegisterEventHandlers()`
- Used for quick operations (notifications, audit logs)

#### Step 8: Create Consumer Event Listeners (optional, for async processing)
- File: `Template.Consumer/Listeners/ProductCreatedEventListener.cs`
- Inherit from `BaseEventListener<ProductCreatedEvent>`
- Register in `Template.Consumer/ServiceCollectionExtensions.cs`
- Used for long-running operations (send emails, call external APIs)

#### Step 9: Create DTOs
- File: `Template.Shared.Models/DTOs/Products/ProductDTO.cs`

#### Step 10: Create Request Models
- File: `Template.Shared.Models/Requests/Products/CreateProductRequest.cs`
- Implement validation in `Validate()` method

#### Step 11: Create Controller
- File: `Template.API/Controllers/ProductsController.cs`
- Route: `[Route("api/products")]`
- Inject `IServiceBus`
- HTTP methods: GET (query), POST (create command), PATCH (update command)

## Key Patterns & Rules

### Service Bus Usage
```csharp
// In Controller or Handler
var result = await _bus.Send<TResult>(command_or_query);
```

### Event Publishing (in Command Handler)
```csharp
// After SaveChangesAsync()
var myEvent = new MyEvent
{
    TraceId = command.TraceId,
    // ... populate event data
};
await _bus.Publish(myEvent);
Log.Information("MyEvent published for {Id}", myEvent.Id);
```

### Logging with TraceId
```csharp
using (LogContext.PushProperty("TraceId", command.TraceId))
{
    Log.Information("Operation description: {Detail}", value);
}
```

### Password Hashing (Use Template.Utilities.Cryptography)
```csharp
string passwordSalt = PasswordSaltGenerator.GenerateSalt();
string hash = HashGenerator.GenerateSHA256Hash(password, passwordSalt);
```

### Async/Await
- All database operations must be async
- All handlers return `Task<TResult>`
- Always `await` LINQ operations (FirstOrDefaultAsync, ToListAsync, etc.)

### Validation Flow
1. Controller receives request
2. Call `request.Validate()` 
3. If `!validationResult.IsValid`, return `BadRequest(validationResult.Errors)`
4. Otherwise, send command/query via `_bus.Send<T>()`

### Event Publishing Flow
1. Command handler executes business logic
2. Call `await _dbContext.SaveChangesAsync()` to persist changes
3. Create event object with same `TraceId`
4. Call `await _bus.Publish(event)` to notify subscribers
5. API Event handlers execute synchronously (fire-and-forget pattern)
6. Consumer picks up event from message queue asynchronously

### Error Handling
- Return `null` from handlers for "not found" cases
- Controllers check for null and return `NotFound()`
- No exceptions for business logic failures
- Log warnings for unexpected conditions

## Consumer Service (Background Processing)

The `Template.Consumer` is a separate Windows/Linux service that processes events asynchronously:

### Architecture
```
API publishes events
     ↓
Message Queue (InMemory/RabbitMQ/Kafka/Service Bus)
     ↓
Consumer picks up events
     ↓
Event Listeners process (emails, integrations, long-running tasks)
```

### Message Queue Implementations
- **InMemory**: Development/testing (default)
- **RabbitMQ**: Production message queuing
- **Kafka**: High-throughput event streaming
- **Service Bus**: Azure cloud messaging

### Configuration
Set via `appsettings.json` in Consumer project:
```json
{
  "MessageQueue": {
    "Type": "RabbitMQ",
    "ConnectionString": "amqp://user:pass@localhost/"
  }
}
```

### Running the Consumer
```bash
cd Template.Consumer
dotnet run --configuration Production
```

### Adding New Event Listeners
1. Create class inheriting `BaseEventListener<YourEvent>`
2. Implement `HandleEventAsync()`
3. Register in `Template.Consumer/ServiceCollectionExtensions.cs`
4. Consumer automatically picks up and runs it

## Project Dependencies

### Template.API
- Microsoft.AspNetCore.OpenApi
- Serilog + Serilog.Settings.Configuration + Serilog.Sinks.MSSqlServer
- Swashbuckle.AspNetCore (Swagger)
- Entity Framework Core (SQL Server provider)
- Mapster

### Template.Consumer
- Microsoft.Extensions.Hosting (Worker Service)
- Serilog + Serilog.Extensions.Hosting + Serilog.Sinks.Console
- (RabbitMQ.Client, Confluent.Kafka, Azure.Messaging.ServiceBus - add as needed)

## Important Notes

1. **TraceId**: Always generate a new `Guid.NewGuid()` in controllers when creating commands/queries for tracking; propagate to events
2. **Database Context**: Injected into command handlers only (not query handlers)
3. **Event Publishing**: Published from command handlers after `SaveChangesAsync()`
4. **Logging**: All operations logged with TraceId for audit trail
5. **Naming**: Use `Template.*` prefix for all projects
6. **Async Pattern**: Strict async/await throughout - no synchronous DB calls
7. **Multiple Handlers**: Multiple event handlers can be registered for the same event type
8. **Separation of Concerns**: 
   - API Event Handlers = immediate side effects (sync)
   - Consumer Event Listeners = long-running tasks (async)

## Workflow Preferences

- **Commit on Changes**: Commit to git immediately after making changes
- Keep each logical change in its own commit
- Use descriptive commit messages

## Git Status
- Main branch: `release`
- Current working branch: `develop`
- All projects compile successfully: 0 warnings, 0 errors
