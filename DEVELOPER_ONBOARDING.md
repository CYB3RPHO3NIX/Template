# Developer Onboarding Guide

Welcome to the Template project! This guide will get you up to speed on the architecture, patterns, and enforcement mechanisms.

## Quick Start (5 minutes)

### 1. Clone and Setup
```bash
git clone <repo>
cd Template
dotnet restore
dotnet build
```

### 2. Read These First (in order)
1. **[CLAUDE.md](CLAUDE.md)** - Architecture overview (5 min read)
2. **[SOLID_REFACTORING_GUIDE.md](SOLID_REFACTORING_GUIDE.md)** - Design patterns (10 min read)
3. **[ARCHITECTURE_ENFORCEMENT.md](ARCHITECTURE_ENFORCEMENT.md)** - How enforcement works (10 min read)

### 3. Enable Pre-commit Hook (Optional but Recommended)

**On Windows (PowerShell):**
```powershell
Copy-Item ".claude/hooks/pre-commit-hook.ps1" ".git/hooks/pre-commit"
icacls ".git/hooks/pre-commit" /grant:r "%USERNAME%":F
```

**On Linux/Mac:**
```bash
cp .claude/hooks/pre-commit-hook.ps1 .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

Now every commit will be automatically validated! 🚀

## Architecture at a Glance

### Request Flow
```
HTTP Request
    ↓
[Controller] validates request
    ↓
[Command/Query] sent via ServiceBus
    ↓
[Handler] executes business logic
    ↓
[Event] published for side effects
    ↓
[EventHandler] or [Consumer/EventListener] processes
    ↓
HTTP Response
```

### Project Structure
```
Template.API              → REST endpoints (Swagger)
Template.Commands        → State-changing operations
Template.CommandHandlers → Execute commands
Template.Queries         → Read operations  
Template.QueryHandlers   → Execute queries
Template.Events          → Domain events
Template.EventHandlers   → In-process event handlers
Template.Consumer        → Background event processing
Template.Services        → DI/ServiceBus setup
Template.Database        → EF Core + SQL Server
Template.Contracts       → Interfaces & abstractions
Template.Shared.Models   → DTOs, requests, responses
Template.Utilities       → Helpers (crypto, etc.)
```

## Before You Code

### ✅ Checklist Before Starting

- [ ] Read CLAUDE.md
- [ ] Read SOLID_REFACTORING_GUIDE.md  
- [ ] Setup pre-commit hook
- [ ] Run `dotnet build` successfully
- [ ] Run `dotnet test Template.Architecture.Tests` - all pass
- [ ] Understand the CQRS pattern
- [ ] Understand Strategy pattern usage

### 🚫 Common Mistakes (Don't Do These!)

1. **Putting business logic in Controllers**
   ```csharp
   // ❌ WRONG
   [HttpPost]
   public async Task<IActionResult> Create(CreateUserRequest request)
   {
       var user = new User { Email = request.Email };  // Logic in controller!
       await _dbContext.Users.AddAsync(user);
       return Ok();
   }
   
   // ✅ CORRECT
   [HttpPost]
   public async Task<IActionResult> Create(CreateUserRequest request)
   {
       var userId = await _bus.Send(new CreateUserCommand { ... });
       return Ok(userId);
   }
   ```

2. **Commands/Queries importing from Handlers**
   ```csharp
   // ❌ WRONG - Architecture test WILL FAIL
   namespace Template.Commands
   {
       using Template.CommandHandlers;  // ❌
       public class MyCommand { }
   }
   
   // ✅ CORRECT
   namespace Template.Commands
   {
       public class MyCommand : ICommand<Guid?> { }  // Pure data
   }
   ```

3. **Hardcoding connection strings**
   ```csharp
   // ❌ WRONG
   var connection = "amqp://localhost:5672/";
   
   // ✅ CORRECT  
   var connection = configuration["MessageQueue:ConnectionString"];
   ```

4. **Using Console.WriteLine instead of Serilog**
   ```csharp
   // ❌ WRONG
   Console.WriteLine("User created");
   
   // ✅ CORRECT
   Log.Information("User created with UserId: {UserId}", userId);
   ```

5. **Methods over 30 lines**
   ```csharp
   // ❌ WRONG - Extract into separate methods
   public async Task<Guid?> Handle(CreateUserCommand command)
   {
       // 50+ lines of nested logic
   }
   
   // ✅ CORRECT - Clear, focused methods
   public async Task<Guid?> Handle(CreateUserCommand command)
   {
       ValidateCommand(command);
       var user = CreateUser(command);
       await SaveUserToDatabase(user);
       await PublishUserCreatedEvent(user);
       return user.Id;
   }
   ```

## Step-by-Step: Adding a New Feature

### Example: Add "Product" feature

#### Step 1: Create Entity
```csharp
// Template.Database/Domain/Entities/Product.cs
namespace Template.Database.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
```

Add to DbContext:
```csharp
public DbSet<Product> Products { get; set; }
```

#### Step 2: Create Command
```csharp
// Template.Commands/Products/ProductCommands/CreateProductCommand.cs
public class CreateProductCommand : ICommand<Guid?>
{
    public Guid TraceId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### Step 3: Create CommandHandler
```csharp
// Template.CommandHandlers/Products/ProductCommandHandlers/CreateProductCommandHandler.cs
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid?>
{
    private readonly IServiceBus _bus;
    private readonly TemplateDbContext _dbContext;

    public CreateProductCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
    {
        _bus = bus;
        _dbContext = dbContext;
    }

    public async Task<Guid?> Handle(CreateProductCommand command)
    {
        using (LogContext.PushProperty("TraceId", command.TraceId))
        {
            Log.Information("Creating product: {ProductName}", command.Name);
            
            var productId = Guid.NewGuid();
            await _dbContext.Products.AddAsync(new Product
            {
                ProductId = productId,
                Name = command.Name,
                Price = command.Price
            });
            
            await _dbContext.SaveChangesAsync();
            
            // Publish event for async processing
            await _bus.Publish(new ProductCreatedEvent 
            { 
                TraceId = command.TraceId, 
                ProductId = productId,
                Name = command.Name 
            });
            
            Log.Information("Product created: {ProductId}", productId);
            return productId;
        }
    }
}
```

#### Step 4: Create Event
```csharp
// Template.Events/Products/ProductCreatedEvent.cs
public class ProductCreatedEvent : IEvent
{
    public Guid TraceId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}
```

#### Step 5: Create EventListener (optional)
```csharp
// Template.Consumer/Listeners/ProductCreatedEventListener.cs
public class ProductCreatedEventListener : BaseEventListener<ProductCreatedEvent>
{
    public ProductCreatedEventListener(IMessageQueue messageQueue) : base(messageQueue) { }

    protected override async Task HandleEventAsync(ProductCreatedEvent @event)
    {
        Log.Information("Product created: {ProductName}", @event.Name);
        // Send notifications, update external systems, etc.
    }
}
```

#### Step 6: Create Query
```csharp
// Template.Queries/Products/ProductQueries/GetProductByIdQuery.cs
public class GetProductByIdQuery : IQuery<ProductDTO?>
{
    public Guid TraceId { get; set; }
    public Guid ProductId { get; set; }
}
```

#### Step 7: Create QueryHandler
```csharp
// Template.QueryHandlers/Products/ProductQueryHandlers/GetProductByIdQueryHandler.cs
public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDTO?>
{
    private readonly TemplateDbContext _dbContext;

    public async Task<ProductDTO?> Handle(GetProductByIdQuery query)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.ProductId == query.ProductId);
        
        return product == null ? null : new ProductDTO
        {
            Id = product.ProductId,
            Name = product.Name,
            Price = product.Price
        };
    }
}
```

#### Step 8: Create DTO
```csharp
// Template.Shared.Models/DTOs/Products/ProductDTO.cs
public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

#### Step 9: Create Request
```csharp
// Template.Shared.Models/Requests/Products/CreateProductRequest.cs
public class CreateProductRequest : IRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();
        
        if (string.IsNullOrWhiteSpace(Name))
            result.Errors.Add("Product name is required");
            
        if (Price <= 0)
            result.Errors.Add("Product price must be greater than 0");
            
        return result;
    }
}
```

#### Step 10: Create Controller
```csharp
// Template.API/Controllers/ProductsController.cs
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IServiceBus _bus;

    public ProductsController(IServiceBus bus)
    {
        _bus = bus;
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var product = await _bus.Send<ProductDTO?>(new GetProductByIdQuery
        {
            TraceId = Guid.NewGuid(),
            ProductId = productId
        });
        
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var validation = request.Validate();
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        var productId = await _bus.Send<Guid?>(new CreateProductCommand
        {
            TraceId = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        });
        
        return Ok(productId);
    }
}
```

#### Step 11: Test It

```bash
# Build
dotnet build

# Run architecture tests
dotnet test Template.Architecture.Tests

# All tests should pass ✅
```

#### Step 12: Commit
```bash
git add .
git commit -m "feat(products): add create and get product endpoints"

# Pre-commit hook validates everything automatically
```

## How Enforcement Catches Mistakes

### Scenario 1: You accidentally import Handler in Command
```csharp
// You write this by mistake:
using Template.CommandHandlers;  // Oops!

namespace Template.Commands
{
    public class CreateProductCommand : ICommand<Guid?> { }
}
```

**What happens:**
1. ❌ You run `dotnet build` → FAILS (if analyzers enabled)
2. ❌ You run architecture tests → TEST FAILS: "CommandsShouldNotDependOnHandlers"
3. ❌ You try to commit → PRE-COMMIT HOOK BLOCKS IT
4. ❌ Even if you bypass → CI/CD BLOCKS THE PULL REQUEST

**Result:** You're forced to fix it. ✅

### Scenario 2: You make a method too complex
```csharp
public async Task<Guid?> Handle(CreateProductCommand command)
{
    // 80 lines of nested if-else, multiple responsibilities
}
```

**What happens:**
1. 📋 Pre-commit hook detects high complexity
2. 📋 CI/CD metrics flag it
3. 👤 Code review catches it and requests refactoring

**Result:** You learn to extract methods. ✅

## Running Tests

```bash
# All tests
dotnet test

# Just architecture tests
dotnet test Template.Architecture.Tests

# Specific architecture test
dotnet test Template.Architecture.Tests -k "CommandsShouldNotDependOnHandlers"

# With verbose output
dotnet test --logger "console;verbosity=detailed"
```

## Debugging Tips

### Check if architecture tests pass
```bash
dotnet test Template.Architecture.Tests

# Should see all tests pass ✅
```

### Find violations quickly
```bash
# Run build and show errors
dotnet build /p:TreatWarningsAsErrors=true

# Shows all violations in output
```

### Debug using Serilog
```csharp
// Add to appsettings.json
{
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}

// Now Debug logs appear
Log.Debug("Debug message here: {Value}", value);
```

## Getting Help

### Architecture Questions
- Read: [CLAUDE.md](CLAUDE.md)
- Ask: Your tech lead or senior developer

### Design Pattern Questions  
- Read: [SOLID_REFACTORING_GUIDE.md](SOLID_REFACTORING_GUIDE.md)
- Ask: Your tech lead

### Enforcement Questions
- Read: [ARCHITECTURE_ENFORCEMENT.md](ARCHITECTURE_ENFORCEMENT.md)
- Ask: Your tech lead

### Setup/Environment Issues
- Check: This onboarding guide
- Ask: Devops team

## Key Takeaways

✅ **DO:**
- Follow naming conventions strictly
- Keep methods small (5-10 lines)
- Use dependency injection
- Log with Serilog
- Write commands/queries as pure data
- Test your code before committing
- Read the enforcement docs

❌ **DON'T:**
- Put business logic in controllers
- Import handlers in commands/queries
- Use Console.WriteLine
- Hardcode configuration values
- Make methods over 30 lines
- Violate layer dependencies
- Skip running tests

## Your First Commit

1. Pick a small feature to add
2. Follow the "Adding a New Feature" example above
3. Run `dotnet build`
4. Run `dotnet test Template.Architecture.Tests`
5. Commit with: `git commit -m "feat(feature-name): description"`
6. Push to your branch
7. Create pull request
8. Wait for CI/CD to validate
9. Request code review

**Welcome to the team! You've got this! 🚀**
