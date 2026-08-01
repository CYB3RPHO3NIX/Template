# Template Usage Examples

Real-world examples of using both methods to create microservices.

## Example 1: Quick Invoice Service

**Goal:** Create InvoiceService in 30 seconds

### Using PowerShell Script

```powershell
cd C:\Projects
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService"
```

**What happens:**
1. Template copied to `C:\Projects\InvoiceService`
2. All files renamed: `Template.API` → `InvoiceService.API`
3. All code updated: `using Template.*` → `using InvoiceService.*`
4. NuGet restored
5. Visual Studio opens with solution

**Result:** Fully functional microservice, ready to code!

```
InvoiceService/
├── InvoiceService.API/
│   ├── Program.cs
│   ├── appsettings.json
│   └── Controllers/
│       └── IdentityController.cs (ready to customize)
├── InvoiceService.Commands/
├── InvoiceService.Queries/
├── InvoiceService.Events/
└── InvoiceService.sln
```

---

## Example 2: Enterprise Services with Company Name

**Goal:** Create 3 services for Acme Corporation with proper namespaces

### Using PowerShell Script (Batch)

```powershell
$company = "AcmeCorp"
$basePath = "C:\Projects"

$services = @("Invoice", "Order", "Payment")

foreach ($service in $services) {
    Write-Host "Creating $service Service..."
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName "$($service)Service" `
        -OutputPath $basePath `
        -CompanyName $company
    Write-Host "✓ $service Service created!"
}
```

**Generated namespaces:**
```csharp
namespace AcmeCorp.InvoiceService.Commands { }
namespace AcmeCorp.OrderService.Queries { }
namespace AcmeCorp.PaymentService.Events { }
```

**Result:**
```
C:\Projects\
├── InvoiceService/      (with AcmeCorp.InvoiceService.* namespaces)
├── OrderService/        (with AcmeCorp.OrderService.* namespaces)
└── PaymentService/      (with AcmeCorp.PaymentService.* namespaces)
```

---

## Example 3: From Git Repository

**Goal:** Generate services from a Git-hosted template

### Using PowerShell Script

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "ShippingService" `
    -TemplatePath "https://github.com/your-org/CQRSTemplate.git" `
    -OutputPath "C:\Projects" `
    -CompanyName "Logistics Inc"
```

**What happens:**
1. Git clones from repository
2. `.git` folder removed (fresh repo for new service)
3. All Template references replaced with ShippingService
4. All Enterprise references replaced with Logistics Inc

**Result:** New service ready for independent Git initialization

---

## Example 4: Enterprise-Wide with dotnet new

**Goal:** Install template for entire development team

### Installation (One-Time)

**From NuGet (Recommended for enterprise):**

```bash
# IT Admin publishes template to internal NuGet
# Developers install globally
dotnet new install Enterprise.CQRSMicroservice --nuget-source https://internal-nuget.company.com
```

**Or from Git:**
```bash
dotnet new install "https://github.com/your-org/Template.git"
```

### Usage (Any Developer)

```bash
# Developer creates their service anytime
dotnet new cqrs-microservice -n InvoiceService

# Or with custom path
dotnet new cqrs-microservice -n OrderService -o "D:\Projects"

# Or with company prefix
dotnet new cqrs-microservice -n PaymentService -CompanyName "MyCompany"
```

**Result:** 
- Fully standardized services across the team
- Consistent architecture
- No manual file renaming needed
- One-line to create any service

---

## Example 5: CI/CD - Automated Service Generation

**Goal:** Auto-generate microservices via GitHub Actions workflow

### GitHub Actions Workflow

```yaml
name: Create New Microservice
on: 
  workflow_dispatch:
    inputs:
      service_name:
        description: 'Service name (e.g., InvoiceService)'
        required: true
      company_name:
        description: 'Company name prefix'
        required: false
        default: 'Enterprise'

jobs:
  create-service:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Create Service from Template
        shell: pwsh
        run: |
          .\Create-MicroserviceFromTemplate.ps1 `
            -ServiceName "${{ github.event.inputs.service_name }}" `
            -OutputPath "./generated" `
            -CompanyName "${{ github.event.inputs.company_name }}" `
            -SkipRestore
      
      - name: Verify Build
        run: |
          cd "./generated/${{ github.event.inputs.service_name }}"
          dotnet build
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v3
        with:
          name: ${{ github.event.inputs.service_name }}
          path: ./generated/${{ github.event.inputs.service_name }}
          
      - name: Create Pull Request
        uses: peter-evans/create-pull-request@v4
        with:
          commit-message: "Create new microservice: ${{ github.event.inputs.service_name }}"
          title: "New Microservice: ${{ github.event.inputs.service_name }}"
          body: |
            # New Microservice Generated
            
            **Service Name:** ${{ github.event.inputs.service_name }}
            **Company:** ${{ github.event.inputs.company_name }}
            
            This service was auto-generated from the CQRS template.
            
            ## Review Checklist
            - [ ] Verify project naming
            - [ ] Check namespaces
            - [ ] Review appsettings.json
            - [ ] Configure database
            - [ ] Setup CI/CD pipeline
          branch: microservice/${{ github.event.inputs.service_name }}
```

**Usage:** Go to GitHub Actions tab → Select workflow → Run with inputs

---

## Example 6: Custom Output Structure

**Goal:** Generate multiple services in organized folder structure

### PowerShell Script

```powershell
$company = "MyCompany"
$servicesPath = "C:\Projects\Microservices"
$services = @(
    @{ Name = "UserService"; Tier = "Identity" },
    @{ Name = "OrderService"; Tier = "Business" },
    @{ Name = "NotificationService"; Tier = "Integration" }
)

foreach ($service in $services) {
    $tierPath = Join-Path $servicesPath $service.Tier
    
    Write-Host "Creating $($service.Name) in $($service.Tier)..."
    
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName $service.Name `
        -OutputPath $tierPath `
        -CompanyName $company
}
```

**Generated structure:**
```
Microservices/
├── Identity/
│   └── UserService/
│       ├── UserService.API/
│       ├── UserService.Commands/
│       └── UserService.sln
├── Business/
│   ├── OrderService/
│   └── ProductService/
└── Integration/
    ├── NotificationService/
    └── EmailService/
```

---

## Example 7: Development Workflow

**Goal:** Set up local development with generated services

### Step-by-Step

```bash
# 1. Create service
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "MyService" -DontOpen

# 2. Navigate to service
cd MyService

# 3. Initialize Git
git init
git add .
git commit -m "Initial commit from CQRS template"

# 4. Add remote
git remote add origin https://github.com/myorg/MyService.git

# 5. Configure local settings
Copy-Item "appsettings.json" "appsettings.Development.json"
# Edit appsettings.Development.json with local database/queue settings

# 6. Build and verify
dotnet build
dotnet run --project MyService.API

# 7. Browse to API
Start-Process "https://localhost:7001/swagger"
```

---

## Example 8: Template Customization

**Goal:** Extend template with company-specific features

### Before Distribution

```bash
# 1. Clone template
git clone https://github.com/your-org/Template.git
cd Template

# 2. Customize (add your features, update docs, etc.)
# - Add company-specific middleware
# - Update authentication strategy
# - Add logging configuration
# - etc.

# 3. Test the template
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "TestService"
cd TestService
dotnet build  # Should work
dotnet test   # If tests exist

# 4. Commit customizations
git add .
git commit -m "Customize template with company standards"

# 5. Publish
git push origin main

# 6. Create release tag
git tag -a v1.1.0 -m "Release v1.1.0 with company customizations"
git push origin v1.1.0

# 7. Publish to NuGet (for enterprise)
dotnet pack -c Release -o ./nupkg
dotnet nuget push ./nupkg/*.nupkg -s https://internal-nuget-server
```

---

## Example 9: Migration from Old Monolith

**Goal:** Generate new microservices during modernization

### Strategy

```powershell
# Old monolith features to extract
$features = @(
    "Invoicing",
    "Inventory", 
    "Shipping",
    "Reporting"
)

$company = "LegacyCorp"

# Generate a service for each feature
foreach ($feature in $features) {
    $serviceName = "$($feature)Service"
    
    Write-Host "Extracting $feature..."
    
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName $serviceName `
        -OutputPath "C:\Projects\Modernization" `
        -CompanyName $company
        
    # TODO: Copy relevant code from monolith into new service
    # TODO: Update database migration scripts
    # TODO: Setup API routes matching old endpoints
    # TODO: Implement database sync/migration layer
}

Write-Host "✓ All services generated. Ready for feature migration!"
```

---

## Example 10: Quick Reference Card

```powershell
# ⚡ FASTEST METHOD - PowerShell

# Basic
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "ServiceName"

# Full control
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "ServiceName" `
    -OutputPath "C:\Projects" `
    -CompanyName "Company" `
    -TemplatePath "C:\Templates\CQRSTemplate"

# Flags
-DontOpen      # Don't open in VS
-SkipRestore   # Skip NuGet restore
```

```bash
# 📦 PROFESSIONAL METHOD - dotnet new

# Install (one-time)
dotnet new install "path/to/template"

# Create service
dotnet new cqrs-microservice -n ServiceName

# List all installed
dotnet new list | grep cqrs

# Uninstall
dotnet new uninstall Enterprise.CQRSMicroservice
```

---

## Troubleshooting Examples

### Example 1: PowerShell execution disabled

```powershell
# Error: "Script execution is disabled"

# Solution: Enable for current user
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Then run script
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "ServiceName"
```

### Example 2: File permissions issue

```powershell
# Error: "Access denied" when writing files

# Solution: Run PowerShell as Administrator
# Right-click PowerShell → "Run as administrator"
# Then run script

.\Create-MicroserviceFromTemplate.ps1 -ServiceName "ServiceName"
```

### Example 3: Template not found

```powershell
# Error: "Template.sln not found"

# Solution: Verify template path
Get-ChildItem "C:\Path\To\Template" -Filter "*.sln"

# Should show: Template.sln
# If not, use correct path
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "ServiceName" `
    -TemplatePath "C:\Correct\Path"
```

---

## Performance Benchmarks

| Task | PowerShell | dotnet new |
|------|-----------|-----------|
| Install template | N/A | 30 seconds |
| Create service | 15-20 seconds | 5-8 seconds |
| Restore NuGet | 30-60 seconds | Automatic |
| Total first service | ~2 minutes | ~1.5 minutes |
| Total per additional service (after install) | ~2 minutes | 30-40 seconds |

**Takeaway:** PowerShell is faster for a single service. dotnet new is much faster after initial setup, especially for creating many services.

---

## Next Steps

After generating a service:

1. ✅ Verify project builds: `dotnet build`
2. 🔧 Configure database connection in `appsettings.json`
3. 📝 Add your domain entities
4. 🎯 Create first command/query pair
5. 🚀 Setup CI/CD pipeline
6. 📊 Configure monitoring & logging

See [CLAUDE.md](CLAUDE.md) for architecture details and getting started guide.
