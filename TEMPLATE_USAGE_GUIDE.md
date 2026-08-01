# Enterprise CQRS Microservice Template - Usage Guide

This guide shows how to use this template to create new microservices with a single command.

## Quick Start (Local Usage)

### Option 1: Using Local Template (Immediate)

**Prerequisites:**
- .NET 10.0 SDK installed
- Git installed

**Step 1: Clone the Template**
```bash
git clone https://github.com/your-org/Template.git C:\Templates\CQRSTemplate
cd C:\Templates\CQRSTemplate
```

**Step 2: Install the Template**
```bash
dotnet new install C:\Templates\CQRSTemplate
```

**Step 3: Create a New Microservice**
```bash
# Basic usage (uses default name "MyMicroservice")
dotnet new cqrs-microservice -n InvoiceService

# With company name override
dotnet new cqrs-microservice -n OrderService -CompanyName "YourCompanyName"

# Specify output directory
dotnet new cqrs-microservice -n PaymentService -o C:\Projects\Services
```

**Step 4: Verify Installation**
```bash
dotnet new list | grep cqrs
```

### Option 2: Using PowerShell Script (Fastest)

If you want to skip the `dotnet new` setup entirely, use the provided PowerShell script.

See the **PowerShell Automation** section below.

## Installation Methods

### Method 1: Local Installation (Recommended for Teams)

Perfect for shared team development.

```bash
# Install from local filesystem
dotnet new install "C:\Path\To\Template"

# Or install from Git repository
dotnet new install "https://github.com/your-org/Template.git"

# Verify installation
dotnet new list
```

### Method 2: NuGet Package (For Distribution)

To share with the entire organization via NuGet:

**Step 1: Create NuGet Package**
```bash
cd C:\Projects\Template

# Pack the template
dotnet pack .template.config\template.json -o C:\NuGetOutput

# Or use this detailed command
dotnet new pack --output-dir C:\NuGetOutput
```

**Step 2: Publish to Internal NuGet Feed**
```bash
dotnet nuget push C:\NuGetOutput\*.nupkg -s https://your-nuget-server/
```

**Step 3: Install from NuGet**
```bash
# Install from NuGet feed
dotnet new install Enterprise.CQRSMicroservice

# Create new service
dotnet new cqrs-microservice -n InvoiceService
```

**Step 4: Update Template**
```bash
# When template is updated, reinstall
dotnet new uninstall Enterprise.CQRSMicroservice
dotnet new install Enterprise.CQRSMicroservice
```

### Method 3: GitHub Template Repository

Make this a GitHub template repository for one-click creation:

1. Go to repository settings
2. Enable "Template repository"
3. Users click "Use this template" button
4. Manual find-replace for "Template" → "ServiceName"

## Usage Examples

### Example 1: Invoice Service

```bash
dotnet new cqrs-microservice -n InvoiceService
cd InvoiceService
dotnet build
dotnet run --project InvoiceService.API
```

**Generated structure:**
```
InvoiceService/
├── InvoiceService.API/
├── InvoiceService.Consumer/
├── InvoiceService.Database/
├── InvoiceService.Commands/
├── InvoiceService.Queries/
├── InvoiceService.Events/
├── InvoiceService.EventHandlers/
├── InvoiceService.CommandHandlers/
├── InvoiceService.QueryHandlers/
├── InvoiceService.Contracts/
└── InvoiceService.sln
```

### Example 2: Order Service with Company Name

```bash
dotnet new cqrs-microservice -n OrderService -CompanyName "Acme Corp"
```

**Namespaces generated:**
```csharp
namespace AcmeCorp.OrderService.Commands { }
namespace AcmeCorp.OrderService.Queries { }
namespace AcmeCorp.OrderService.Events { }
// etc.
```

### Example 3: Batch Creation (Multiple Services)

```powershell
$services = @("InvoiceService", "OrderService", "PaymentService", "ShippingService")
$basePath = "C:\Projects"

foreach ($service in $services) {
    Write-Host "Creating $service..."
    dotnet new cqrs-microservice -n $service -o "$basePath\$service"
    Write-Host "$service created successfully!`n"
}
```

## Template Variables

| Variable | Purpose | Example |
|----------|---------|---------|
| `ServiceName` | Name of your microservice | `InvoiceService` |
| `RootNamespace` | Root namespace (auto-derived from ServiceName) | `InvoiceService` |
| `CompanyName` | Optional company name prefix | `AcmeCorp` |

### Variable Replacement Scope

- **ServiceName** replaces:
  - All `Template` folder/file names
  - Project file names (`.csproj`)
  - Namespace declarations
  - Class names and references

- **CompanyName** replaces:
  - `Enterprise` in namespace prefixes
  - Company branding in documentation

## Managing Templates

### List Installed Templates
```bash
dotnet new list
dotnet new list | grep cqrs
```

### Uninstall Template
```bash
# Uninstall local template
dotnet new uninstall "C:\Path\To\Template"

# Uninstall NuGet package
dotnet new uninstall Enterprise.CQRSMicroservice
```

### Update Template

When you update the template source:

```bash
# Uninstall old version
dotnet new uninstall "C:\Path\To\Template"

# Reinstall with updates
dotnet new install "C:\Path\To\Template"
```

## Customization

### Adding New Parameters

Edit `.template.config\template.json` to add custom parameters:

```json
"symbols": {
  "DatabaseEngine": {
    "type": "parameter",
    "description": "Database: SqlServer or PostgreSQL",
    "defaultValue": "SqlServer",
    "replaces": "SqlServer"
  }
}
```

Then use in templates:
```bash
dotnet new cqrs-microservice -n MyService -DatabaseEngine PostgreSQL
```

### Conditional Files/Folders

Use replacement tokens in filenames:

- `Template.sln` → `{ServiceName}.sln`
- `Template.API` → `{ServiceName}.API`
- `src/Template/` → `src/{ServiceName}/`

### Post-Actions

Configure automatic actions after template generation (in `template.json`):

```json
"postActions": [
  {
    "id": "restore",
    "actionId": "210D431B-A78B-4D2F-B762-4F2289A3F200",
    "description": "Restore NuGet packages"
  },
  {
    "id": "openSolution",
    "actionId": "84C0DA21-51C8-4541-9E7C-61978820175F",
    "description": "Open in Visual Studio"
  }
]
```

## PowerShell Automation Script

See `Create-MicroserviceFromTemplate.ps1` for automated generation with validation.

Usage:
```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -OutputPath "C:\Projects"

# With company name
.\Create-MicroserviceFromTemplate.ps1 `
  -ServiceName "OrderService" `
  -OutputPath "C:\Projects" `
  -CompanyName "AcmeCorp"
```

## CI/CD Integration

### GitHub Actions

```yaml
name: Generate Microservice

on: 
  workflow_dispatch:
    inputs:
      service_name:
        description: 'Service name (e.g., InvoiceService)'
        required: true

jobs:
  create-service:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Create Service
        run: |
          dotnet new install ./
          dotnet new cqrs-microservice -n ${{ github.event.inputs.service_name }} -o ./output
      
      - name: Upload artifact
        uses: actions/upload-artifact@v3
        with:
          name: ${{ github.event.inputs.service_name }}
          path: ./output
```

## Troubleshooting

### Template not found
```bash
# Verify installation
dotnet new list | grep cqrs

# Reinstall if missing
dotnet new install "C:\Path\To\Template"
```

### Namespace not replaced correctly
- Ensure `ServiceName` doesn't have special characters
- Check `template.json` `sourceName` matches project name
- Run: `dotnet new cqrs-microservice --help` to verify parameters

### Post-action failures (restore/open)
- These are non-critical and can be skipped with `--no-restore`
- Manual restore: `dotnet restore` after generation

### File encoding issues (special characters)
- Ensure `.template.config/template.json` is UTF-8
- File paths should use PascalCase (e.g., `InvoiceService`)

## Best Practices

1. **Naming Convention**
   - Use PascalCase: `InvoiceService`, not `invoice-service` or `invoiceservice`
   - Keep service names concise: `Invoice` or `InvoiceService`, not `InvoicingMicroservice`

2. **Namespace Consistency**
   - Company: `YourCompany` (PascalCase)
   - Service: `ServiceName` (PascalCase)
   - Result: `YourCompany.ServiceName.*`

3. **Git Workflow**
   ```bash
   # Create from template
   dotnet new cqrs-microservice -n MyService
   
   # Initialize git
   cd MyService
   git init
   git add .
   git commit -m "Initial commit from CQRS microservice template"
   git remote add origin https://github.com/org/MyService.git
   git push -u origin main
   ```

4. **Template Updates**
   - Keep template in sync with your architecture standards
   - Version your template releases
   - Document breaking changes
   - Test template generation regularly

5. **Sharing Templates**
   - Internal NuGet feed for organization
   - Private GitHub repository with template enabled
   - Team wiki with setup instructions

## Advanced Usage

### Programmatic Template Generation (C#)

```csharp
using System.Diagnostics;

var processInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = $"new cqrs-microservice -n MyService -o C:\\Projects",
    UseShellExecute = false,
    RedirectStandardOutput = true
};

using (var process = Process.Start(processInfo))
{
    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    Console.WriteLine(output);
}
```

### Batch Service Creation

```powershell
$services = @(
    @{ Name = "InvoiceService"; Company = "Acme" },
    @{ Name = "OrderService"; Company = "Acme" },
    @{ Name = "PaymentService"; Company = "Acme" }
)

foreach ($service in $services) {
    dotnet new cqrs-microservice `
        -n $service.Name `
        -CompanyName $service.Company `
        -o "C:\Projects\$($service.Name)"
}
```

## Support & Issues

- Documentation: See `CLAUDE.md` and architecture guides
- Report template issues: Create an issue in the template repository
- Contribute improvements: Submit PRs to the template
