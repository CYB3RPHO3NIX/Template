# Existing Database Setup Guide

This guide explains how to initialize Code First migrations when you already have a database with tables.

## 📋 Scenario

You have:
- ✓ A SQL Server database with existing tables
- ✓ Entity Framework Core models defined
- ✗ No migration history (no `__EFMigrationsHistory` table)

## 🚀 Step-by-Step Setup

### Step 1: Verify Database Connection

Update `TemplateDbContextFactory.cs` with your connection string:

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;TrustServerCertificate=True"
    );
}
```

### Step 2: Understand Your Current State

**Your entities** (`Domain/Entities/`) should match your database schema exactly:
- All existing tables should have corresponding entity classes
- All existing columns should have corresponding properties
- Navigate relationships should match foreign keys

### Step 3: Run Migration Script

```bash
cd Template.Database
.\Migration.ps1
```

**What happens:**
1. Script detects no migration history
2. Creates `Initial` migration representing current database state
3. Marks `Initial` migration as applied (doesn't recreate tables)
4. Sets up migration tracking for future changes

### Step 4: Verify Migration History

```bash
dotnet ef migrations list -p Template.Database.csproj
```

**Expected output:**
```
Initial
```

### Step 5: Make Your First Change

Now you can modify your entities:

```csharp
// In User.cs
public string? PhoneNumber { get; set; }
```

**Configure it:**
```csharp
// In UserConfiguration.cs
entity.Property(e => e.PhoneNumber).HasMaxLength(20);
```

**Generate and apply migration:**
```bash
.\Migration.ps1
```

You'll see:
```
Auto_20260801_180000
```

## 🔧 Advanced: Manual Initial Migration

If the automatic approach doesn't work:

### Option 1: Create Initial with No Operations

```bash
# Create the migration
dotnet ef migrations add Initial -p Template.Database.csproj

# Edit Migrations/Initial.cs to remove Up() operations:
public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Leave empty - tables already exist
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Leave empty
    }
}

# Apply it
dotnet ef database update -p Template.Database.csproj
```

### Option 2: Reset and Start Fresh

If initial migration has issues:

```bash
# 1. Remove migration files
rm Migrations/Initial.cs
rm Migrations/Auto_*.cs

# 2. Drop the database (WARNING: Deletes all data!)
dotnet ef database drop -p Template.Database.csproj --force

# 3. Recreate from scratch
.\Migration.ps1
```

⚠️ **WARNING**: This deletes all data. Only use in development!

## 📊 Migration History Management

### View Current Migrations
```bash
dotnet ef migrations list -p Template.Database.csproj
```

### Check Applied Migrations
Query the database:
```sql
SELECT * FROM __EFMigrationsHistory
ORDER BY MigrationId DESC;
```

### Revert to a Migration
```bash
dotnet ef database update Initial -p Template.Database.csproj
```

This rolls back all migrations after `Initial`.

## 🎯 Best Practices

### 1. Initial Setup
- ✓ Ensure all entities match database schema
- ✓ Configure all properties in Configuration classes
- ✓ Run migration script once to create `Initial`
- ✓ Commit both entities and Initial migration

### 2. Ongoing Development
- ✓ Modify entity (add/remove properties)
- ✓ Update Configuration class
- ✓ Run `.\Migration.ps1`
- ✓ Commit migrations and entities together
- ✓ Never manually edit migration files

### 3. Team Collaboration
- ✓ All team members start with same Initial migration
- ✓ Each developer runs migrations independently
- ✓ Migrations stack (Initial → Auto_001 → Auto_002 → ...)
- ✓ One migration per logical change

## ❌ Common Mistakes

### ❌ Not Running Initial Migration First
```bash
# Wrong - will fail
.\Migration.ps1  # Tries to add tables that exist

# Right - creates Initial first
# Script auto-detects and handles this
```

### ❌ Mixing Database First with Code First
```csharp
// Don't do this - scaffolds from DB
dotnet ef dbcontext scaffold "connection" Microsoft.EntityFrameworkCore.SqlServer

// Do this instead - define entities, run migrations
// Modify entity → Run .\Migration.ps1
```

### ❌ Committing Without Initial Migration
```bash
# Wrong - others get migration conflicts
git add entities/
git commit -m "Added entities"

# Right - commit together
git add Migrations/ entities/ Configuration/
git commit -m "Added entities with Initial migration"
```

## ✅ Success Checklist

- [ ] Database connection string verified
- [ ] All entities match database tables
- [ ] All properties match database columns
- [ ] Initial migration created and applied
- [ ] `__EFMigrationsHistory` table exists
- [ ] `dotnet ef migrations list` shows `Initial`
- [ ] Can modify entities and generate new migrations
- [ ] Tests still pass

## 📚 Related Guides

- [CODE_FIRST_SETUP.md](CODE_FIRST_SETUP.md) — General Code First workflow
- [CLAUDE.md](../CLAUDE.md) — Architecture patterns

---

**You now have full migration tracking on your existing database!** 🎉

All future changes will be tracked as migrations, making it easy to:
- Replicate changes across environments
- Collaborate with team members
- Maintain database schema history
- Rollback if needed
