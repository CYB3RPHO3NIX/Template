# Testing Guide

## Overview

This project includes comprehensive test suites for all handler types:
- **Template.CommandHandler.Tests** - Tests for command handler business logic
- **Template.QueryHandler.Tests** - Tests for query handler data retrieval
- **Template.EventHandler.Tests** - Tests for event handler side effects
- **Template.Architecture.Tests** - Tests for architectural compliance

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run specific test project
```bash
dotnet test Template.CommandHandler.Tests
dotnet test Template.QueryHandler.Tests
dotnet test Template.EventHandler.Tests
dotnet test Template.Architecture.Tests
```

### Run specific test class
```bash
dotnet test Template.CommandHandler.Tests -k CreateUserCommandHandlerTests
```

### Run specific test method
```bash
dotnet test Template.CommandHandler.Tests -k "Handle_WithValidCommand_CreatesUserSuccessfully"
```

### Run with verbose output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run with code coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
```

## Test Structure

### Arrange-Act-Assert Pattern

All tests follow the AAA pattern for clarity:

```csharp
[Fact]
public async Task Handle_WithValidCommand_CreatesUserSuccessfully()
{
    // Arrange: Setup test data and mocks
    var command = new CreateUserCommand { ... };
    _mockBus.Setup(x => x.Send(...)).ReturnsAsync(false);

    // Act: Execute the code being tested
    var result = await _handler.Handle(command);

    // Assert: Verify the results
    Assert.NotNull(result);
    Assert.True(result.IsValid);
}
```

### Database Isolation

Tests use `UseInMemoryDatabase` with unique GUIDs to ensure isolation:

```csharp
var options = new DbContextOptionsBuilder<TemplateDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Unique per test
    .Options;

_dbContext = new TemplateDbContext(options);
```

### Resource Cleanup

Tests implement `IDisposable` to clean up resources:

```csharp
public class TestClass : IDisposable
{
    public void Dispose()
    {
        _dbContext?.Dispose();  // Clean up after each test
    }
}
```

## CommandHandler Tests

### Test File
`Template.CommandHandler.Tests/Identity/UserCommandHandlers/CreateUserCommandHandlerTests.cs`

### What's Tested
- ✅ Valid command execution creates expected result
- ✅ Error handling (user already exists)
- ✅ Database persistence
- ✅ Event publishing
- ✅ Password hashing with salt
- ✅ Data validation
- ✅ Audit trail updates

### Example Test
```csharp
[Fact]
public async Task Handle_WithValidCommand_CreatesUserSuccessfully()
{
    // Arrange
    var command = new CreateUserCommand
    {
        TraceId = Guid.NewGuid(),
        UserName = "testuser",
        Email = "test@example.com",
        Password = "SecurePassword123"
    };

    _mockBus
        .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
        .ReturnsAsync(false);

    // Act
    var result = await _handler.Handle(command);

    // Assert
    Assert.NotNull(result);
    var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result);
    Assert.Equal("testuser", createdUser.Username);
}
```

## QueryHandler Tests

### Test File
`Template.QueryHandler.Tests/Identity/UserQueryHandlers/GetUserByIdQueryHandlerTests.cs`

### What's Tested
- ✅ Returning correct entity by ID
- ✅ Null handling when entity not found
- ✅ DTO mapping correctness
- ✅ Filtering multiple entities
- ✅ Read-only behavior (no database changes)

### Example Test
```csharp
[Fact]
public async Task Handle_WithExistingUser_ReturnsUserDTO()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User
    {
        UserId = userId,
        Username = "testuser",
        Email = "test@example.com",
        PasswordHash = "hash",
        PasswordSalt = "salt",
        IsActive = true
    };

    await _dbContext.Users.AddAsync(user);
    await _dbContext.SaveChangesAsync();

    var query = new GetUserByIdQuery
    {
        TraceId = Guid.NewGuid(),
        UserId = userId
    };

    // Act
    var result = await _handler.Handle(query);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(userId, result.Id);
    Assert.Equal("testuser", result.UserName);
}
```

## EventHandler Tests

### Test File
`Template.EventHandler.Tests/Identity/UserCreatedEventHandlerTests.cs`

### What's Tested
- ✅ Successful event processing
- ✅ Error handling
- ✅ TraceId preservation
- ✅ Interface implementation
- ✅ Edge cases (empty strings, old/future dates)

### Example Test
```csharp
[Fact]
public async Task Handle_WithValidEvent_CompletesSuccessfully()
{
    // Arrange
    var @event = new UserCreatedEvent
    {
        TraceId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        UserName = "newuser",
        Email = "new@example.com",
        CreatedOn = DateTime.UtcNow
    };

    // Act
    var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

    // Assert
    Assert.Null(exception);
}
```

## Mocking Strategy

### Service Bus Mock
```csharp
_mockBus.Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
    .ReturnsAsync(false);

_mockBus.Verify(
    x => x.Publish(It.IsAny<UserCreatedEvent>()),
    Times.Once
);
```

### Query Verification
```csharp
_mockBus.Verify(
    x => x.Send(It.Is<DoesUserExistQuery>(q =>
        q.Email == "test@example.com" &&
        q.UserName == "testuser")),
    Times.Once
);
```

## Writing New Tests

### Step 1: Create Test File
```csharp
// Template.CommandHandler.Tests/Identity/UserCommandHandlers/YourCommandHandlerTests.cs
public class YourCommandHandlerTests : IDisposable
{
    private readonly TemplateDbContext _dbContext;
    private readonly Mock<IServiceBus> _mockBus;
    private readonly YourCommandHandler _handler;

    public YourCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TemplateDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TemplateDbContext(options);
        _mockBus = new Mock<IServiceBus>();
        _handler = new YourCommandHandler(_mockBus.Object, _dbContext);
    }

    [Fact]
    public async Task Handle_WithValidCommand_SucceedsAsync()
    {
        // Arrange
        var command = new YourCommand { /* ... */ };

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
```

### Step 2: Test Happy Path
```csharp
[Fact]
public async Task Handle_WithValidInput_ReturnsExpectedResult()
{
    // Most important test - the successful case
}
```

### Step 3: Test Error Cases
```csharp
[Fact]
public async Task Handle_WithInvalidInput_ReturnsNull()
{
    // Test validation and error handling
}

[Fact]
public async Task Handle_WithDuplicateData_ReturnsNull()
{
    // Test business rule violations
}
```

### Step 4: Test Side Effects
```csharp
[Fact]
public async Task Handle_PublishesExpectedEvents()
{
    // Verify events are published correctly
    _mockBus.Verify(x => x.Publish(...), Times.Once);
}
```

## Test Naming Convention

Follow the pattern: `Handle_[Condition]_[ExpectedResult]`

```csharp
// Good test names
Handle_WithValidCommand_CreatesUserSuccessfully
Handle_WithExistingUser_ReturnsFalse
Handle_PublishesUserCreatedEvent
Handle_WithNullEmail_StoresEmptyString

// Bad test names
TestCreateUser
Test1
HandleTest
ItWorks
```

## Running Tests in CI/CD

Tests run automatically on every PR via GitHub Actions:

```yaml
- name: Run Tests
  run: dotnet test --logger "console;verbosity=detailed"
```

If any test fails:
- ❌ PR cannot be merged
- 👤 Developer must fix or update the test
- 🔄 Tests must pass before merge

## Test Coverage Goals

**Target:** 80%+ code coverage

**Run coverage:**
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
```

**Coverage by area:**
- Handlers: 95%+ (critical path logic)
- Queries: 90%+ (data retrieval logic)
- Events: 85%+ (side effect logic)
- Architecture: 100% (strict enforcement)

## Common Test Scenarios

### Testing Data Persistence
```csharp
var result = await _handler.Handle(command);
var persisted = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result);
Assert.NotNull(persisted);
```

### Testing Query Results
```csharp
var result = await _handler.Handle(query);
Assert.NotNull(result);
Assert.Equal(expectedValue, result.Property);
```

### Testing Event Publishing
```csharp
_mockBus.Verify(x => x.Publish(It.IsAny<YourEvent>()), Times.Once);
```

### Testing Error Handling
```csharp
_mockBus.Setup(x => x.Send(...)).ReturnsAsync(true);  // User exists
var result = await _handler.Handle(command);
Assert.Null(result);  // Should return null on error
```

## Debugging Tests

### Run single test with debugging
```bash
dotnet test Template.CommandHandler.Tests -k "SpecificTestName" --verbosity diagnostic
```

### View test output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Check database state
In your test, print database state before/after:
```csharp
var countBefore = await _dbContext.Users.CountAsync();
// ... test code ...
var countAfter = await _dbContext.Users.CountAsync();
Assert.Equal(countBefore + 1, countAfter);
```

## Test Categories

Tests can be categorized using traits:

```csharp
[Fact(DisplayName = "User creation happy path")]
public async Task Handle_WithValidCommand_CreatesUserSuccessfully()
{
    // ...
}
```

Run tests by category:
```bash
dotnet test --filter "DisplayName~'happy path'"
```

## Best Practices

✅ **DO:**
- Test one thing per test
- Use descriptive test names
- Follow Arrange-Act-Assert pattern
- Use in-memory database for isolation
- Mock external dependencies
- Test both success and failure paths
- Clean up resources with IDisposable

❌ **DON'T:**
- Test multiple scenarios in one test
- Use vague test names
- Test implementation details
- Share database between tests
- Mock internal classes
- Skip error case testing
- Leave resources uncleaned

## When Tests Fail

### Red Test
```
FAIL: Handle_WithValidCommand_CreatesUserSuccessfully
Expected: User created
Actual: User not created
```

**Action:** Check the handler implementation

### Flaky Test
```
Sometimes passes, sometimes fails randomly
```

**Cause:** Usually database isolation or timing issues
**Fix:** Ensure unique in-memory database per test

### Test Passes Locally, Fails in CI
```
Works on my machine!
```

**Cause:** Usually environment difference
**Fix:** Check for hardcoded values, system dependencies

## Summary

Tests serve as:
1. **Specification** - Define expected behavior
2. **Documentation** - Show how to use handlers
3. **Safety net** - Catch regressions
4. **Quality gate** - Enforce standards

Run tests frequently:
```bash
# Before committing
dotnet test

# During development
dotnet test --watch

# In CI/CD
dotnet test --logger "trx"
```

Tests + Architecture Enforcement = **Confidence in Code Quality** ✅
