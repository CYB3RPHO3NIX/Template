# Architecture Enforcement Guide

## Overview

This project enforces CQRS and layered architecture through **multiple automated gates** that prevent junior developers from violating the established rules. No manual code review is needed to catch architectural violations - they're caught automatically before code even reaches the repository.

## Enforcement Layers

### Layer 1: Local Development (EditorConfig)

**File:** `.editorconfig`

**What it enforces:**
- ✅ Code formatting (indentation, line length, spacing)
- ✅ Naming conventions (PascalCase, camelCase, interfaces start with I)
- ✅ Modifier order (public, private, protected, static, etc.)

**How it works:**
- IDEs (Visual Studio, VS Code) automatically apply rules
- Warnings appear inline as developers code
- Code is reformatted on save

**Example violations caught:**
```csharp
// ❌ Wrong: field doesn't follow naming convention
public string FieldName;  // → Use _fieldName instead

// ❌ Wrong: incorrect modifier order
private static public string Value;  // → Use public static string

// ❌ Wrong: inconsistent spacing
int[] arr={1,2,3};  // → Use int[] arr = { 1, 2, 3 };
```

### Layer 2: Build-Time Analysis (Roslyn Analyzers)

**Install in projects (API and Consumer):**
```bash
dotnet add Template.API package StyleCop.Analyzers
dotnet add Template.Consumer package StyleCop.Analyzers
```

**What it enforces:**
- ✅ Documentation requirements (public members must have XML docs)
- ✅ Code quality rules (no magic numbers, proper error handling)
- ✅ Architectural patterns (method complexity, nesting depth)
- ✅ Naming consistency

**How it works:**
- Runs during `dotnet build`
- Fails build if violations found
- Creates compilation errors (not just warnings)

**Example build failure:**
```
error CS0626: Public member missing documentation comment
error CS1522: Identifier expected, base type has incorrect format
```

### Layer 3: Pre-Commit Hook (Local Validation)

**File:** `.claude/hooks/pre-commit-hook.ps1`

**What it enforces:**
- ✅ Successful build
- ✅ Passing architecture tests
- ✅ Passing unit tests
- ✅ No large files (>10MB)
- ✅ Naming conventions for new files
- ✅ No hardcoded connection strings

**How to setup:**

**On Windows (PowerShell):**
```powershell
# Copy hook to git hooks directory
Copy-Item ".claude/hooks/pre-commit-hook.ps1" ".git/hooks/pre-commit"

# Make it executable
icacls ".git/hooks/pre-commit" /grant:r "%USERNAME%":F

# Or run manually before committing:
PowerShell -ExecutionPolicy Bypass -File ".claude/hooks/pre-commit-hook.ps1"
```

**On Linux/Mac (Bash):**
```bash
# Create .git/hooks/pre-commit with this content:
#!/bin/bash
dotnet build -c Debug || exit 1
dotnet test Template.Architecture.Tests -c Debug --no-build || exit 1
```

**Example: Hook blocks commit**
```
❌ Pre-commit checks failed:
  Build failed. Fix compilation errors before committing.

Fix the issues above and try committing again.
```

### Layer 4: Architecture Tests (ArchUnitNET)

**File:** `Template.Architecture.Tests/ArchitectureTests.cs`

**What it enforces:**
- ✅ Commands don't depend on CommandHandlers
- ✅ Queries don't depend on QueryHandlers  
- ✅ Events don't depend on EventHandlers
- ✅ Handlers don't depend on Controllers
- ✅ Database layer doesn't depend on API
- ✅ All types use Template.* namespace
- ✅ Naming conventions (Commands end with "Command", Queries with "Query", etc.)
- ✅ Interfaces start with "I"
- ✅ No public fields
- ✅ No static fields (except constants)
- ✅ Handlers implement correct interfaces

**How to run:**
```bash
# Run all architecture tests
dotnet test Template.Architecture.Tests

# Run specific test
dotnet test Template.Architecture.Tests -k "CommandsShouldNotDependOnHandlers"

# With verbose output
dotnet test Template.Architecture.Tests --logger "console;verbosity=detailed"
```

**Example: Test failure**
```
FAIL CommandsShouldNotDependOnHandlers
  Reason: Create.CreateUserCommand depends on CreateUserCommandHandler
  Fix: Commands should only contain data, not import handler types
```

**How violations happen and get caught:**
```csharp
// ❌ WRONG: Command depends on Handler
namespace Template.Commands.Identity.UserCommands
{
    using Template.CommandHandlers.Identity.UserCommandHandlers;  // ❌ VIOLATION
    
    public class CreateUserCommand : ICommand<Guid?>
    {
        // Will FAIL architecture test: "Commands should not depend on handlers"
    }
}

// ✅ CORRECT: Command is pure data
namespace Template.Commands.Identity.UserCommands
{
    public class CreateUserCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public string UserName { get; set; }
        // No handler imports
    }
}
```

### Layer 5: CI/CD Pipeline (GitHub Actions)

**File:** `.github/workflows/code-quality.yml`

**What it enforces (on every PR):**
- ✅ Code builds successfully
- ✅ All architecture tests pass
- ✅ Code formatting complies with EditorConfig
- ✅ Code coverage maintained
- ✅ No obvious code smells
- ✅ Commit messages follow conventions

**How it works:**
1. Trigger: Developer creates pull request
2. Runs: Full build + all tests
3. Result: Pass/Fail badge on PR
4. Blocks: Cannot merge if checks fail

**PR Workflow:**
```
Developer creates PR
  ↓
GitHub Actions runs (.github/workflows/code-quality.yml)
  ├─ Build verification
  ├─ Architecture tests
  ├─ Unit tests
  ├─ Code coverage
  └─ Code smell detection
  ↓
❌ If ANY check fails → PR blocked from merging
✅ If ALL checks pass → Can proceed to review
```

**Example: PR blocked**
```
❌ Checks have failed
  • Code Quality Checks (Build failed)
    - Template.Commands.CreateUserCommand imports Template.CommandHandlers
    - Architecture test: "CommandsShouldNotDependOnHandlers" failed

Required checks must pass before merging.
```

## Implementation Guide for Junior Developers

### When Adding a New Feature

Follow this checklist to ensure all enforcement gates pass:

#### 1. Create Command (if it's a state-changing operation)
```csharp
// ✅ CORRECT: Pure data class
namespace Template.Commands.Products.ProductCommands
{
    using Template.Contracts.Command;
    using System;

    public class CreateProductCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
```

**Violations caught by enforcement:**
- ❌ Importing from CommandHandlers → Architecture test fails
- ❌ Adding business logic → Pre-commit hook detects complexity
- ❌ Using non-Template namespace → EditorConfig + Architecture test
- ❌ Naming it something other than *Command → Architecture test fails

#### 2. Create CommandHandler
```csharp
// ✅ CORRECT: Implements ICommandHandler
namespace Template.CommandHandlers.Products.ProductCommandHandlers
{
    using Template.Contracts.CommandHandler;
    using Template.Commands.Products.ProductCommands;
    using Template.Database.Domain.Contexts;
    using Serilog;

    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid?>
    {
        private readonly TemplateDbContext _dbContext;
        
        public CreateProductCommandHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(CreateProductCommand command)
        {
            // Business logic here
            await _dbContext.SaveChangesAsync();
            
            // Publish event if needed
            // await _bus.Publish(new ProductCreatedEvent { ... });
            
            return productId;
        }
    }
}
```

**Violations caught:**
- ❌ Doesn't implement `ICommandHandler<,>` → Architecture test fails
- ❌ Named `CreateProduct` instead of `CreateProductCommandHandler` → Architecture test fails
- ❌ Depends on Controllers → Architecture test fails
- ❌ Method exceeds 50 lines → Pre-commit hook warns

#### 3. Build and Test
```bash
# This runs all enforcement checks:
dotnet build

# This specifically checks architecture:
dotnet test Template.Architecture.Tests

# This simulates the pre-commit hook:
PowerShell -ExecutionPolicy Bypass -File ".claude/hooks/pre-commit-hook.ps1"
```

**Issues caught before commit:**
- ❌ Compilation errors → Build fails
- ❌ Architectural violations → Tests fail
- ❌ Code style issues → EditorConfig warnings

#### 4. Commit
```bash
git add .
git commit -m "feat(products): add create product command and handler"

# Pre-commit hook automatically runs and validates everything
# If checks fail → Commit is blocked
# If checks pass → Commit succeeds
```

#### 5. Create Pull Request
```bash
git push origin feature/create-product
# Creates PR on GitHub
# CI/CD pipeline automatically runs all checks again
# PR shows pass/fail status
```

## Common Violations and How They're Caught

### Violation 1: Command imports Handler

**The violation:**
```csharp
// ❌ BAD
using Template.CommandHandlers;  // VIOLATION!

namespace Template.Commands
{
    public class MyCommand : ICommand { }
}
```

**Caught by:**
1. ✅ Architecture Test: `CommandsShouldNotDependOnHandlers`
2. ✅ CI/CD Pipeline: Architecture tests run on every PR
3. 🚫 Blocks: Commit if pre-commit hook enabled, PR merge otherwise

**Error message:**
```
FAIL: CommandsShouldNotDependOnHandlers
  Reason: Template.Commands.MyCommand depends on Template.CommandHandlers
  Fix: Remove the using statement. Commands are pure data.
```

### Violation 2: Query in API layer

**The violation:**
```csharp
// ❌ BAD
namespace Template.API.Controllers
{
    public class UserController
    {
        public void GetUser(Guid id)
        {
            // Creates Query here - violates layering!
            var query = new GetUserByIdQuery { UserId = id };
        }
    }
}
```

**Caught by:**
1. ✅ Code Review: Manual review (this is presentation-tier logic)
2. 🚫 Architecture Test: Could add rule if needed

**Best practice:**
```csharp
// ✅ CORRECT
namespace Template.API.Controllers
{
    public class UserController
    {
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var query = new GetUserByIdQuery { TraceId = Guid.NewGuid(), UserId = userId };
            var user = await _bus.Send<UserDTO?>(query);
            return user == null ? NotFound() : Ok(user);
        }
    }
}
```

### Violation 3: Method too long or complex

**Caught by:**
1. ✅ Pre-commit Hook: Detects complexity
2. ✅ Code Review: Manual review
3. 📊 CI/CD: Code coverage metrics

**Example violation:**
```csharp
public async Task<Guid?> Handle(CreateUserCommand command)
{
    // 100+ lines of nested if-else
    // Multiple responsibilities
    // Hard to test
    // CI/CD might flag based on complexity metrics
}
```

**Should be refactored:**
```csharp
public async Task<Guid?> Handle(CreateUserCommand command)
{
    LogOperationStart();
    ValidateCommand(command);
    
    var existingUser = await CheckUserExists(command);
    if (existingUser) return null;
    
    var userId = await CreateUser(command);
    await PublishUserCreatedEvent(command, userId);
    
    LogOperationComplete(userId);
    return userId;
}
```

### Violation 4: Magic numbers/hardcoded values

**Caught by:**
1. ✅ EditorConfig/Analyzers: StyleCop rules
2. ✅ Pre-commit Hook: Detects hardcoded IPs/ports
3. ✅ Code Review: Manual review

**Violation:**
```csharp
public class RabbitMQMessageQueue
{
    private const string ConnectionString = "amqp://localhost:5672/";  // ❌
}
```

**Correct approach:**
```csharp
public class RabbitMQMessageQueue
{
    private readonly string _connectionString;
    
    public RabbitMQMessageQueue(string connectionString)
    {
        _connectionString = connectionString;  // From appsettings.json
    }
}
```

## Bypassing Checks (Not Recommended)

### Force commit (bypass pre-commit hook)
```bash
git commit --no-verify
# ❌ NOT recommended - defeats purpose
# ✅ Use only in emergencies, but CI/CD will still catch it
```

### Skip failed test
```bash
git push --force
# ❌ NOT recommended - breaks history
# ✅ CI/CD will still block PR merge
```

## Dashboard: Enforcement Status

Create a status page showing enforcement health:
```bash
# Current architecture compliance
dotnet test Template.Architecture.Tests --logger "console;verbosity=quiet"

# Build success rate (from CI/CD logs)
# Latest PR: All checks passing ✅

# Code coverage trend
# Baseline: 75% → Current: 78% ↑
```

## For Team Leads

### Monitoring Violations

**Check enforcement effectiveness:**
```bash
# View recent violations
git log --grep="architecture test failed" --grep="build failed" -i

# Check test results history
# (from GitHub Actions)
# → Go to Actions tab
# → View "Code Quality Checks"
# → See pass/fail trend
```

### Updating Rules

**To add a new enforcement rule:**

1. **Add to ArchitectureTests.cs:**
```csharp
[Fact]
public void MyNewRule()
{
    var rule = Types()
        .That()
        .ResideInNamespace("Template.MyLayer")
        .Should()
        .NotDependOnAny(Types().That().ResideInNamespace("Template.OtherLayer"))
        .Because("Reason for this rule");

    rule.Check(Architecture);
}
```

2. **Commit and push**
3. **All future builds will enforce it**

### Disabling a Rule (Temporarily)

If a rule is too strict:
```csharp
[Fact(Skip = "Reviewing architecture approach")]
public void StrictRule()
{
    // Temporarily disabled while team discusses
}
```

## Summary

This project has **5 layers of enforcement**:

| Layer | Tools | Timing | Blocks |
|-------|-------|--------|--------|
| 1. Local Dev | EditorConfig | Real-time | Warnings only |
| 2. Build | Roslyn Analyzers | On `dotnet build` | ✅ Yes |
| 3. Pre-commit | PowerShell script | Before commit | ✅ Yes (if enabled) |
| 4. Architecture | ArchUnitNET tests | On test run | ✅ Yes |
| 5. CI/CD | GitHub Actions | On PR | ✅ Yes (blocks merge) |

**Result:** Junior developers cannot accidentally violate architecture because:
- IDE warns them immediately
- Build fails if violations
- Tests fail if violations
- Pre-commit hook blocks commit
- CI/CD blocks PR merge

**No code review is needed to catch architectural violations** - they're prevented automatically!
