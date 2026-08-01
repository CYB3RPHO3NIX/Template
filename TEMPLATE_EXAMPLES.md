# PowerShell Script - Real-World Examples

Real-world examples of using the PowerShell script to create microservices.

## Example 1: Quick Invoice Service (30 seconds)

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

**Next:**
```powershell
cd InvoiceService
dotnet build
dotnet run --project InvoiceService.API
```

---

## Example 2: Multiple Services with Company Name

**Goal:** Create 3 services for Acme Corporation with proper namespaces

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
    
    Write-Host "✓ $service Service created!`n"
}
```

**Result:**
```
C:\Projects\
├── InvoiceService/   (AcmeCorp.InvoiceService.*)
├── OrderService/     (AcmeCorp.OrderService.*)
└── PaymentService/   (AcmeCorp.PaymentService.*)
```

**Generated namespaces:**
```csharp
namespace AcmeCorp.InvoiceService.Commands { }
namespace AcmeCorp.OrderService.Queries { }
namespace AcmeCorp.PaymentService.Events { }
```

---

## Example 3: From Git Repository

**Goal:** Generate services from a Git-hosted template

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
4. All Logistics Inc references applied

---

## Example 4: Batch Services Organization

**Goal:** Create services organized by business tier

```powershell
$company = "MyCompany"
$basePath = "C:\Projects\Microservices"

$services = @(
    @{ Name = "UserService"; Tier = "Identity" },
    @{ Name = "OrderService"; Tier = "Business" },
    @{ Name = "NotificationService"; Tier = "Integration" }
)

foreach ($service in $services) {
    $tierPath = Join-Path $basePath $service.Tier
    New-Item -ItemType Directory -Path $tierPath -Force | Out-Null
    
    Write-Host "Creating $($service.Name) in $($service.Tier)..."
    
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName $service.Name `
        -OutputPath $tierPath `
        -CompanyName $company
}
```

**Result:**
```
Microservices/
├── Identity/
│   └── UserService/
├── Business/
│   └── OrderService/
└── Integration/
    └── NotificationService/
```

---

## Example 5: Development Workflow

**Goal:** Set up local development with generated services

```powershell
# 1. Create service
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "MyService" -DontOpen

# 2. Navigate
cd MyService

# 3. Initialize Git
git init
git add .
git commit -m "Initial commit from CQRS template"
git remote add origin https://github.com/myorg/MyService.git

# 4. Create local settings
Copy-Item "appsettings.json" "appsettings.Development.json"
# Edit appsettings.Development.json with local settings

# 5. Build and verify
dotnet build

# 6. Run API
dotnet run --project MyService.API

# 7. Browse API docs
Start-Process "https://localhost:7001/swagger"

# 8. Push to remote
git push -u origin main
```

---

## Example 6: Skip NuGet Restore (Faster)

**Goal:** Generate service quickly without restoring NuGet

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "QuickService" `
    -SkipRestore `
    -DontOpen

cd QuickService

# Restore manually later
dotnet restore
dotnet build
```

---

## Example 7: From Different Template Location

**Goal:** Use a custom template directory

```powershell
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "CustomService" `
    -OutputPath "C:\Projects" `
    -TemplatePath "D:\MyTemplates\CustomCQRS"
```

---

## Example 8: Automation Script

**Goal:** Reusable PowerShell function for rapid generation

```powershell
function New-Microservice {
    param(
        [Parameter(Mandatory=$true)]
        [string]$ServiceName,
        
        [string]$OutputPath = "C:\Projects",
        [string]$CompanyName = "Enterprise"
    )
    
    $scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
    
    & "$scriptPath\Create-MicroserviceFromTemplate.ps1" `
        -ServiceName $ServiceName `
        -OutputPath $OutputPath `
        -CompanyName $CompanyName
    
    Write-Host "✓ Service created at: $(Join-Path $OutputPath $ServiceName)"
}

# Usage:
New-Microservice -ServiceName "PaymentService" -CompanyName "Acme"
```

---

## Example 9: Validation & Error Handling

**Goal:** Create services with validation

```powershell
function New-ValidatedMicroservice {
    param(
        [Parameter(Mandatory=$true)]
        [ValidatePattern('^[A-Z][a-zA-Z0-9]*$')]
        [string]$ServiceName,
        
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_})]
        [string]$OutputPath
    )
    
    $projectPath = Join-Path $OutputPath $ServiceName
    
    if (Test-Path $projectPath) {
        Write-Error "Project already exists at: $projectPath"
        return
    }
    
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName $ServiceName `
        -OutputPath $OutputPath
    
    Write-Host "✓ Successfully created $ServiceName"
}

# Usage (with validation):
New-ValidatedMicroservice -ServiceName "OrderService" -OutputPath "C:\Projects"

# This will error (invalid name):
New-ValidatedMicroservice -ServiceName "order-service" -OutputPath "C:\Projects"
# Error: order-service does not match pattern '^[A-Z][a-zA-Z0-9]*$'
```

---

## Example 10: Integration with CI/CD

**Goal:** Generate services in GitHub Actions / Azure Pipeline

**PowerShell in GitHub Actions:**
```yaml
name: Create Microservice
on: workflow_dispatch
  inputs:
    service_name:
      required: true
      description: "Service name (e.g., InvoiceService)"

jobs:
  create:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Create Service
        shell: pwsh
        run: |
          .\Create-MicroserviceFromTemplate.ps1 `
            -ServiceName "${{ github.event.inputs.service_name }}" `
            -OutputPath "./generated" `
            -SkipRestore
      
      - name: Build Service
        run: |
          cd "./generated/${{ github.event.inputs.service_name }}"
          dotnet build
      
      - name: Upload Artifact
        uses: actions/upload-artifact@v3
        with:
          name: ${{ github.event.inputs.service_name }}
          path: ./generated/${{ github.event.inputs.service_name }}
```

---

## Example 11: Template Update Check

**Goal:** Update template and propagate changes

```powershell
# 1. Get latest template
git -C "C:\Templates\CQRSTemplate" pull origin main

# 2. Generate updated version
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "TestService" `
    -OutputPath "C:\Projects" `
    -TemplatePath "C:\Templates\CQRSTemplate"

# 3. Verify changes
cd C:\Projects\TestService
git diff

# 4. Review and commit
git add .
git commit -m "Update template to latest version"
```

---

## Example 12: Quick Reference Commands

```powershell
# Basic - just works
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService"

# Custom path
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -OutputPath "C:\Projects"

# With company
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -CompanyName "Acme"

# From Git
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -TemplatePath "https://github.com/org/Template.git"

# All options
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "InvoiceService" `
    -OutputPath "C:\Projects" `
    -CompanyName "Acme" `
    -TemplatePath "." `
    -SkipRestore `
    -DontOpen

# Show help
.\Create-MicroserviceFromTemplate.ps1 -Help
```

---

## Next Steps

After generation:

```powershell
cd YourService
dotnet build                                    # Build project
dotnet test                                     # Run tests
dotnet run --project YourService.API            # Start API
# Browse: https://localhost:7001/swagger        # View API docs
```

Then:
- Add domain entities
- Create commands and queries
- Implement handlers
- Configure database
- Setup message queue
- Deploy!

---

## Troubleshooting Examples

### PowerShell Execution Disabled

```powershell
# Error: cannot be loaded because running scripts is disabled

# Fix:
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Template Not Found

```powershell
# Error: Template not found

# Verify:
Get-ChildItem "C:\Path" -Filter "*.sln"

# Fix: Use correct path
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "Service" `
    -TemplatePath "C:\Correct\Path"
```

### Project Already Exists

```powershell
# Error: Project folder already exists

# Fix 1: Different output path
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "Service" `
    -OutputPath "C:\DifferentPath"

# Fix 2: Different service name
.\Create-MicroserviceFromTemplate.ps1 `
    -ServiceName "DifferentName" `
    -OutputPath "C:\Projects"
```

---

**Happy microservice generating!** 🚀
