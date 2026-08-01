# Enterprise CQRS Microservice Template - PowerShell Guide

This guide shows how to use the PowerShell script to create new microservices with a single command.

## Quick Start

### Prerequisites

- **PowerShell 5.0+** - Built-in on Windows 10+
- **.NET 10.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Git** - Optional, only needed if using Git repository as template source

### Basic Usage

**Step 1: Navigate to Template Directory**
```powershell
cd C:\Path\To\Template
```

**Step 2: Run the Script**
```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -OutputPath "C:\Projects"
```

**Step 3: Done!**
```powershell
cd C:\Projects\InvoiceService
dotnet build
dotnet run --project InvoiceService.API
```

## Usage Examples

### Example 1: Basic Service Creation

```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService"
```

**Result:** Creates `InvoiceService` folder in current directory

### Example 2: Custom Output Path

```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "OrderService" -OutputPath "C:\Projects"
```

**Result:** Creates `C:\Projects\OrderService`

### Example 3: With Company Name

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "PaymentService" `
    -OutputPath "C:\Projects" `
    -CompanyName "AcmeCorp"
```

**Generated namespaces:** `AcmeCorp.PaymentService.*`

### Example 4: From Git Repository

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "ShippingService" `
    -TemplatePath "https://github.com/your-org/CQRSTemplate.git" `
    -OutputPath "C:\Projects"
```

### Example 5: Skip NuGet Restore & Visual Studio

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "InvoiceService" `
    -SkipRestore `
    -DontOpen
```

### Example 6: Batch Creation (Multiple Services)

```powershell
$services = @("Invoice", "Order", "Payment", "Shipping")
$basePath = "C:\Projects"
$company = "MyCompany"

foreach ($service in $services) {
    Write-Host "Creating $service Service..."
    
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName "$($service)Service" `
        -OutputPath $basePath `
        -CompanyName $company
    
    Write-Host "✓ $service Service created!`n"
}
```

## Script Parameters

### Required

| Parameter | Alias | Description | Example |
|-----------|-------|-------------|---------|
| `-ServiceName` | `-n` | Service name (PascalCase) | `InvoiceService` |
| `-OutputPath` | `-o` | Output directory path | `C:\Projects` |

### Optional

| Parameter | Alias | Description | Default |
|-----------|-------|-------------|---------|
| `-CompanyName` | `-c` | Company name for namespace | `Enterprise` |
| `-TemplatePath` | `-t` | Path to template (local or Git URL) | Current directory |
| `-SkipRestore` | | Skip NuGet restore | `false` |
| `-DontOpen` | | Don't open in Visual Studio | `false` |

## What the Script Does

1. **Validates Input**
   - Checks service name is PascalCase
   - Verifies output directory exists
   - Ensures project doesn't already exist

2. **Clones/Copies Template**
   - From local filesystem or Git repository
   - Preserves directory structure

3. **Renames Files & Folders**
   - `Template.*` → `YourServiceName.*`
   - Updates all project folders
   - Updates solution file name

4. **Replaces Text in All Files**
   - Updates namespaces: `Template` → `YourServiceName`
   - Updates company prefix: `Enterprise` → `YourCompanyName`
   - Applies to `.cs`, `.csproj`, `.sln`, `.json`, `.md`, `.xml` files

5. **Restores NuGet Packages**
   - Runs `dotnet restore` (can skip with `-SkipRestore`)
   - Ensures all dependencies are downloaded

6. **Opens in Visual Studio**
   - Launches solution in Visual Studio (can skip with `-DontOpen`)
   - Ready to start coding

## Generated Project Structure

All generated services have this structure:

```
YourService/
├── YourService.API/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Controllers/
│   └── YourService.API.csproj
├── YourService.Consumer/
│   ├── Worker.cs
│   ├── Program.cs
│   └── YourService.Consumer.csproj
├── YourService.Commands/
│   └── YourService.Commands.csproj
├── YourService.CommandHandlers/
│   └── YourService.CommandHandlers.csproj
├── YourService.Queries/
│   └── YourService.Queries.csproj
├── YourService.QueryHandlers/
│   └── YourService.QueryHandlers.csproj
├── YourService.Events/
│   └── YourService.Events.csproj
├── YourService.EventHandlers/
│   └── YourService.EventHandlers.csproj
├── YourService.Database/
│   └── YourService.Database.csproj
├── YourService.Contracts/
│   └── YourService.Contracts.csproj
└── YourService.sln
```

## Naming Conventions

### Service Name

- **Format:** PascalCase, alphanumeric only
- **Examples:**
  - ✅ `InvoiceService`
  - ✅ `OrderProcessingService`
  - ❌ `invoice-service` (kebab-case)
  - ❌ `invoiceService` (camelCase)

### Company Name

- **Format:** PascalCase, optional
- **Default:** `Enterprise`
- **Examples:**
  - ✅ `AcmeCorp`
  - ✅ `MyCompany`
  - Default namespaces: `Enterprise.InvoiceService.*`
  - Custom namespaces: `AcmeCorp.InvoiceService.*`

## Troubleshooting

### PowerShell Execution Disabled

**Error:**
```
Cannot be loaded because running scripts is disabled on this system
```

**Solution:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Template Not Found

**Error:**
```
✗ Template not found at: C:\Path\To\Template
```

**Solution:**
Verify the path contains `Template.sln`:
```powershell
Get-ChildItem "C:\Path\To\Template" -Filter "*.sln"
```

### Project Already Exists

**Error:**
```
✗ Destination already exists: C:\Projects\InvoiceService
```

**Solution:**
Use a different output directory or service name, or delete the existing folder.

### Permission Denied

**Error:**
```
✗ Access denied when writing files
```

**Solution:**
Run PowerShell as Administrator.

### Git Clone Failed

**Error:**
```
✗ Git clone failed
```

**Solution:**
- Verify Git is installed: `git --version`
- Verify repository URL is correct
- Check internet connection
- Verify Git credentials if private repository

## Performance

| Operation | Time |
|-----------|------|
| Validate inputs | ~100ms |
| Clone/copy template | ~500ms |
| Rename files | ~200ms |
| Replace text | ~600ms |
| NuGet restore | ~5-30 seconds |
| **Total** | **~1-2 minutes** |

*Times vary based on system performance and NuGet package availability*

## Next Steps After Generation

### 1. Verify Build
```powershell
cd YourService
dotnet build
```

### 2. Review Generated Files
- Check namespaces are correct
- Verify project names match
- Review appsettings.json

### 3. Configure Database
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=YourService;Trusted_Connection=true;"
  }
}
```

### 4. Start API Server
```powershell
dotnet run --project YourService.API
```

### 5. Browse API Documentation
Open: `https://localhost:7001/swagger`

### 6. Initialize Git
```powershell
git init
git add .
git commit -m "Initial commit from CQRS template"
git remote add origin https://github.com/your-org/YourService.git
git push -u origin main
```

## Advanced Usage

### Custom Template Location

If your template is in a non-standard location:

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "MyService" `
    -OutputPath "C:\Projects" `
    -TemplatePath "D:\CustomTemplates\CQRSTemplate"
```

### Integration with CI/CD

Run from CI/CD pipeline:

```powershell
# PowerShell in GitHub Actions / Azure Pipeline
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName $env:SERVICE_NAME `
    -OutputPath $env:OUTPUT_PATH `
    -CompanyName $env:COMPANY_NAME `
    -SkipRestore  # CI/CD will restore separately
```

### Batch Update All Services

Update multiple existing services to newer template version:

```powershell
$services = @("InvoiceService", "OrderService", "PaymentService")

foreach ($service in $services) {
    Write-Host "Updating $service..."
    
    # Backup old version
    Copy-Item $service "${service}_backup" -Recurse
    
    # Generate new version
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName $service `
        -OutputPath "." `
        -DontOpen
    
    # Merge changes manually
    Write-Host "Review changes in $service"
}
```

## Best Practices

1. **Use Consistent Company Names**
   - All services: same `-CompanyName` value
   - Ensures consistent namespace hierarchy

2. **Initialize Git Immediately**
   ```powershell
   cd MyService
   git init
   git add .
   git commit -m "Initial commit from CQRS template"
   ```

3. **Keep Template Updated**
   - Pull latest template regularly
   - Compare with your services
   - Adopt improvements

4. **Document Customizations**
   - Track template modifications
   - Document why changes were made
   - Version control your customizations

5. **Test Generation**
   - Generate to test directory first
   - Verify before committing
   - Build and test immediately

## Architecture Reference

After generating a service, review:
- **CQRS Pattern:** [CLAUDE.md - Architecture Layers](CLAUDE.md#architecture-layers)
- **Adding Features:** [CLAUDE.md - Adding a New Feature](CLAUDE.md#adding-a-new-feature---step-by-step)
- **Queue Setup:** [QUEUE_INITIALIZATION_GUIDE.md](QUEUE_INITIALIZATION_GUIDE.md)
- **Event Publishing:** [CLAUDE.md - Event Publishing Flow](CLAUDE.md#event-publishing-flow)

## Support

For issues or questions:

1. Check [TEMPLATE_EXAMPLES.md](TEMPLATE_EXAMPLES.md) for real-world examples
2. Review [CLAUDE.md](CLAUDE.md) for architecture guide
3. See troubleshooting section above
4. Run `.\Create-MicroserviceFromTemplate.ps1 -Help` for help

---

**Happy microservice creation!** 🚀
