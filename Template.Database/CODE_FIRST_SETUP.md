# Code First Database Development Guide

This database project uses **Entity Framework Core Code First** approach. All database schema is defined through C# entity classes and the `TemplateDbContext`.

## 🎯 Architecture

### Entities
- **Location**: `Domain/Entities/`
- **Purpose**: Represent database tables
- **Files**:
  - `User.cs` - User accounts
  - `Role.cs` - Role definitions
  - `Permission.cs` - Permission definitions
  - `UserRole.cs` - User-Role mappings
  - `UserPermission.cs` - User-Permission mappings
  - `RolePermission.cs` - Role-Permission mappings
  - `ApiLog.cs` - API request/response logs
  - `ApplicationLog.cs` - Application event logs

### DbContext
- **File**: `Domain/Contexts/TemplateDbContext.cs`
- **Purpose**: Main database context for dependency injection
- **Method**: `OnModelCreating()` loads all entity configurations automatically

### Entity Configurations
- **Location**: `Domain/Configurations/`
- **Purpose**: Separate configuration classes for each entity (Fluent API)
- **Files**:
  - `UserConfiguration.cs` - User table schema
  - `RoleConfiguration.cs` - Role table schema
  - `PermissionConfiguration.cs` - Permission table schema
  - `UserRoleConfiguration.cs` - User-Role junction table
  - `UserPermissionConfiguration.cs` - User-Permission junction table
  - `RolePermissionConfiguration.cs` - Role-Permission junction table
  - `ApiLogConfiguration.cs` - API logs table
  - `ApplicationLogConfiguration.cs` - Application logs table
- **Each configuration defines**:
  - Table names and schemas
  - Column constraints (max length, type, default values)
  - Indexes
  - Foreign keys
  - Unique constraints

### DbContextFactory
- **File**: `Domain/Contexts/TemplateDbContextFactory.cs`
- **Purpose**: Creates DbContext instances for migrations
- **Used by**: EF Core CLI for generating/applying migrations

## 📋 Workflow

### 1. Modify Your Model

Edit entity files or DbContext configuration:

**Add a new property:**
```csharp
// In User.cs
public string? PhoneNumber { get; set; }
```

**Configure the property in DbContext:**
```csharp
// In OnModelCreating()
entity.Property(e => e.PhoneNumber).HasMaxLength(20);
```

### 2. Generate Migration

Run the migration script to auto-detect changes:

```bash
.\Migration.ps1
```

**What happens:**
- ✓ Analyzes model changes
- ✓ Generates `Auto_<timestamp>` migration
- ✓ Applies migration to database
- ✓ Lists all migrations

### 3. Review Generated Migration

Check the generated migration file:
```
Migrations/Auto_20260801120000_Initial.cs
```

Example content:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Users",
        schema: "identity",
        columns: table => new
        {
            UserId = table.Column<Guid>(nullable: false),
            Username = table.Column<string>(maxLength: 100, nullable: false),
            Email = table.Column<string>(maxLength: 255, nullable: false),
            // ... more columns
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Users", x => x.UserId);
            // ... more constraints
        });
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "Users", schema: "identity");
}
```

### 4. Database Automatically Updates

The migration script applies changes immediately:
- ✓ SQL Server receives schema changes
- ✓ Existing data preserved
- ✓ Indexes created
- ✓ Constraints applied

## 🔧 Configuration

### Entity Configuration Pattern

Each entity has its own configuration class in `Domain/Configurations/`:

**File: UserConfiguration.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Domain.Entities;

namespace Template.Database.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        // Table and schema
        entity.ToTable("Users", "identity");

        // Indexes
        entity.HasIndex(e => e.Email, "IX_Users_Email");
        entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

        // Columns
        entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");
        entity.Property(e => e.Email).HasMaxLength(255);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");

        // Foreign keys
        entity.HasOne(d => d.CreatedByNavigation)
            .WithMany(p => p.InverseCreatedByNavigation)
            .HasForeignKey(d => d.CreatedBy)
            .HasConstraintName("FK_Users_CreatedBy");
    }
}
```

**DbContext auto-loads all configurations:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Automatically loads all IEntityTypeConfiguration implementations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TemplateDbContext).Assembly);
}
```

This approach keeps each configuration clean and separated.

### Adding New Entities

1. **Create Entity Class**:
   ```csharp
   // File: Domain/Entities/Department.cs
   namespace Template.Database.Domain.Entities;

   public class Department
   {
       public Guid DepartmentId { get; set; }
       public string Name { get; set; } = string.Empty;
       public string? Description { get; set; }
       public bool IsActive { get; set; } = true;
       public Guid CreatedBy { get; set; }
       public DateTime CreatedOn { get; set; }
       public Guid? UpdatedBy { get; set; }
       public DateTime? UpdatedOn { get; set; }
   }
   ```

2. **Add DbSet to Context**:
   ```csharp
   // In TemplateDbContext
   public virtual DbSet<Department> Departments { get; set; }
   ```

3. **Configure in OnModelCreating**:
   ```csharp
   modelBuilder.Entity<Department>(entity =>
   {
       entity.ToTable("Departments", "identity");
       entity.Property(e => e.DepartmentId).HasDefaultValueSql("(newsequentialid())");
       entity.Property(e => e.Name).HasMaxLength(100);
       entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
       // ... more configuration
   });
   ```

4. **Generate Migration**:
   ```bash
   .\Migration.ps1
   ```

5. **Done!** Database schema is automatically updated.

## 📁 Folder Structure

```
Template.Database/
├── Domain/
│   ├── Contexts/
│   │   ├── TemplateDbContext.cs           # Main DbContext
│   │   └── TemplateDbContextFactory.cs    # Design-time factory for migrations
│   ├── Configurations/
│   │   ├── UserConfiguration.cs           # Entity configs
│   │   ├── RoleConfiguration.cs
│   │   ├── PermissionConfiguration.cs
│   │   ├── UserRoleConfiguration.cs
│   │   ├── UserPermissionConfiguration.cs
│   │   ├── RolePermissionConfiguration.cs
│   │   ├── ApiLogConfiguration.cs
│   │   └── ApplicationLogConfiguration.cs
│   └── Entities/
│       ├── User.cs
│       ├── Role.cs
│       ├── Permission.cs
│       ├── UserRole.cs
│       ├── UserPermission.cs
│       ├── RolePermission.cs
│       ├── ApiLog.cs
│       └── ApplicationLog.cs
├── Migrations/
│   ├── 20260801120000_Initial.cs         # Generated automatically
│   └── TemplateDbContextModelSnapshot.cs # EF Core internals
├── Migration.ps1                          # Run this to generate & apply migrations
├── CODE_FIRST_SETUP.md                    # This guide
└── Template.Database.csproj
```

## 🚀 Common Tasks

### View Migration History
```bash
dotnet ef migrations list -p Template.Database.csproj
```

Output:
```
20260801120000_Initial
20260801121530_Auto_20260801121530
(Pending) 20260801122000_Auto_20260801122000
```

### Revert to Previous Migration
```bash
dotnet ef database update 20260801120000_Initial -p Template.Database.csproj
```

### Generate SQL Script
```bash
dotnet ef migrations script 20260801120000_Initial 20260801121530_Auto_20260801121530 `
    -p Template.Database.csproj `
    -o migration.sql
```

This creates `migration.sql` with all SQL commands.

### Start Fresh (Development Only)
```bash
dotnet ef database drop -p Template.Database.csproj
.\Migration.ps1
```

⚠️ **WARNING**: This deletes ALL data. Use only in development!

## 🔄 Integration with Application

### Program.cs Registration
```csharp
// In Template.API/Program.cs
services.AddDbContext<TemplateDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.MigrationsAssembly("Template.Database")
    )
);
```

### Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TemplateDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## ✅ Best Practices

1. **One Change at a Time**
   - Make model changes
   - Run migration
   - Commit both together

2. **Review Generated Migrations**
   - Check generated SQL before applying
   - Ensure indexes are created correctly
   - Verify foreign keys

3. **Keep Entities Lean**
   - One entity per table
   - Use navigation properties for relationships
   - Don't duplicate data across entities

4. **Use Meaningful Names**
   - Entity: `User`, `Role`, `Permission`
   - Navigation: `CreatedByNavigation`, `InverseParentPermission`
   - Index: `IX_Users_Email`, `UQ_Users_Username`

5. **Audit Fields**
   - Always include `CreatedBy`, `CreatedOn`
   - Include `UpdatedBy`, `UpdatedOn` for mutable entities
   - Set `UpdatedOn` default to `GETUTCDATE()`

6. **Schema Organization**
   - Use schemas: `identity`, `api`, `log`
   - Logical grouping of related tables

7. **Testing**
   - Always test migrations in development first
   - Check data integrity with existing data
   - Verify constraints don't break inserts

## 🐛 Troubleshooting

### Migration Won't Generate
**Problem**: "No changes detected"
**Solution**: Verify your model changes were saved and match OnModelCreating()

### Migration Won't Apply
**Problem**: "Foreign key constraint violated"
**Solution**: Check data integrity; manually clean data or adjust migration

### DbContext Connection Issues
**Problem**: "Cannot connect to database"
**Solution**: Verify connection string in TemplateDbContextFactory and appsettings.json

## 📚 References

- [EF Core Migrations Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [EF Core Modeling Documentation](https://learn.microsoft.com/en-us/ef/core/modeling/)
- [SQL Server EF Core Provider](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/)

---

**Migration is fully automated. Just modify your code and run `.\Migration.ps1`!** 🚀
