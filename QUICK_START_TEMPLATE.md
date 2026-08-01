# Quick Start: Generate a Microservice from Template

Choose your preferred method below:

## ⚡ Method 1: PowerShell Script (Fastest)

Works immediately without any setup. One command to generate your entire microservice.

### Prerequisites
- PowerShell 5.0+ (built-in on Windows 10+)
- .NET 10.0 SDK
- Git (optional, for template from Git repo)

### Usage

**Basic** - Creates service in current directory:
```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService"
```

**With custom output path:**
```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "OrderService" -OutputPath "C:\Projects"
```

**With company name:**
```powershell
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "PaymentService" -CompanyName "Acme"
```

**From Git repository:**
```powershell
.\Create-MicroserviceFromTemplate.ps1 `
  -ServiceName "ShippingService" `
  -TemplatePath "https://github.com/your-org/CQRSTemplate.git" `
  -OutputPath "C:\Projects"
```

**Skip restore & VS open:**
```powershell
.\Create-MicroserviceFromTemplate.ps1 `
  -ServiceName "InvoiceService" `
  -SkipRestore `
  -DontOpen
```

### What It Does
1. Clones/copies the template
2. Renames all files/folders from "Template" to your service name
3. Updates all namespaces and class names
4. Replaces company name if provided
5. Restores NuGet packages
6. Opens solution in Visual Studio

### Output Structure
```
InvoiceService/
├── InvoiceService.API/              # REST API layer
├── InvoiceService.Consumer/         # Background service
├── InvoiceService.Commands/         # CQRS commands
├── InvoiceService.CommandHandlers/  # Command handlers
├── InvoiceService.Queries/          # CQRS queries
├── InvoiceService.QueryHandlers/    # Query handlers
├── InvoiceService.Events/           # Domain events
├── InvoiceService.EventHandlers/    # Event handlers
├── InvoiceService.Database/         # EF Core models
├── InvoiceService.Contracts/        # DTOs & interfaces
└── InvoiceService.sln
```

---

## 📦 Method 2: dotnet new Template (Professional)

For enterprise distribution via NuGet or team-wide installation.

### Prerequisites
- .NET 10.0 SDK

### Step 1: Install Template

**From local filesystem:**
```bash
dotnet new install "C:\Path\To\Template"
```

**From Git repository:**
```bash
dotnet new install "https://github.com/your-org/Template.git"
```

**From NuGet feed:**
```bash
dotnet new install Enterprise.CQRSMicroservice
```

### Step 2: Create Service

**Basic:**
```bash
dotnet new cqrs-microservice -n InvoiceService
```

**With custom path:**
```bash
dotnet new cqrs-microservice -n OrderService -o "C:\Projects\OrderService"
```

**With company name:**
```bash
dotnet new cqrs-microservice -n PaymentService -CompanyName "Acme"
```

### Step 3: Verify

```bash
dotnet build
dotnet run --project InvoiceService.API
```

### Step 4: Manage

**List installed templates:**
```bash
dotnet new list | grep cqrs
```

**Uninstall:**
```bash
dotnet new uninstall Enterprise.CQRSMicroservice
```

**Update:**
```bash
dotnet new uninstall Enterprise.CQRSMicroservice
dotnet new install Enterprise.CQRSMicroservice --nuget-source https://your-feed/
```

---

## 🔄 Comparison

| Feature | PowerShell Script | dotnet new |
|---------|-------------------|-----------|
| **Setup Time** | ⚡ None | 5 minutes |
| **Command** | `.\Create-*` | `dotnet new` |
| **Works Offline** | ✓ Yes | ✓ Yes |
| **Team Distribution** | Git + script | NuGet package |
| **Visual Studio Integration** | ✓ Launches VS | ⚠ Manual open |
| **Enterprise Ready** | ✓ Yes | ✓✓ Best |
| **Learning Curve** | Simple | Minimal |

---

## 📋 Examples

### Example 1: Quick Service Creation

**Goal:** Create InvoiceService in 30 seconds

```powershell
# PowerShell approach
.\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -OutputPath "C:\Projects"
# ✓ Done! InvoiceService is ready
```

### Example 2: Enterprise Deployment

**Goal:** Create multiple services with company namespace

```powershell
# Create three services for Acme Corp
$services = @("Invoice", "Order", "Payment")

foreach ($service in $services) {
    .\Create-MicroserviceFromTemplate.ps1 `
        -ServiceName "$($service)Service" `
        -OutputPath "C:\Projects\AcmeCorp" `
        -CompanyName "AcmeCorp"
}
```

### Example 3: CI/CD Pipeline

**Goal:** Auto-generate microservices in GitHub Actions

```yaml
name: Create Microservice
on: workflow_dispatch

jobs:
  create:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Create Service
        run: |
          .\Create-MicroserviceFromTemplate.ps1 `
            -ServiceName "InvoiceService" `
            -OutputPath "output"
      - uses: actions/upload-artifact@v3
        with:
          name: InvoiceService
          path: output
```

---

## ✅ Validation Checklist

After creating your service, verify:

- [ ] Solution opens in Visual Studio
- [ ] All projects build without errors
- [ ] Namespace is correct (e.g., `AcmeCorp.InvoiceService`)
- [ ] No "Template" references remain in code
- [ ] `.sln` file name matches service name
- [ ] Database context is named correctly

**Quick validation:**
```bash
cd MyService
dotnet build        # Should succeed
dotnet run --project MyService.API  # Should start API
```

---

## 🚀 Next Steps

1. **Review Architecture**: See [CLAUDE.md](CLAUDE.md) for design patterns
2. **Configure Database**: Update connection string in `appsettings.json`
3. **Add Features**: Create commands/queries in respective folders
4. **Configure Message Queue**: Choose broker (InMemory, RabbitMQ, Kafka, Service Bus)
5. **Deploy**: Push to Git and CI/CD pipeline

---

## 📚 Documentation

- **Architecture Guide**: [CLAUDE.md](CLAUDE.md)
- **CQRS Pattern**: [CLAUDE.md](CLAUDE.md) - Architecture Layers section
- **Queue Management**: [QUEUE_INITIALIZATION_GUIDE.md](QUEUE_INITIALIZATION_GUIDE.md)
- **Detailed Template Usage**: [TEMPLATE_USAGE_GUIDE.md](TEMPLATE_USAGE_GUIDE.md)

---

## ❓ Troubleshooting

### PowerShell script errors

**Error:** "Script execution disabled"
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**Error:** "Template not found"
```powershell
# Ensure Template.sln exists in the path you provided
Get-ChildItem -Filter "*.sln"
```

### dotnet new errors

**Error:** "Unknown template"
```bash
dotnet new list  # Verify template is installed
dotnet new install "path/to/template"  # Reinstall
```

**Error:** "Service already exists"
```bash
# Remove destination folder first, or specify different output path
dotnet new cqrs-microservice -n MyService -o "C:\Projects\MyService"
```

---

## 💡 Pro Tips

1. **Create all your services with consistent company name:**
   ```powershell
   $company = "Acme"
   .\Create-MicroserviceFromTemplate.ps1 -ServiceName "InvoiceService" -CompanyName $company
   .\Create-MicroserviceFromTemplate.ps1 -ServiceName "OrderService" -CompanyName $company
   ```

2. **Use version control from the start:**
   ```bash
   cd MyService
   git init
   git add .
   git commit -m "Initial commit from CQRS template"
   ```

3. **Keep template updated:**
   - Clone latest template regularly
   - Compare with your generated services
   - Adopt improvements to new services

4. **Document your customizations:**
   - Services often have domain-specific modifications
   - Keep separate from template changes
   - Consider a template versioning strategy

---

## 🤝 Support

- **Questions**: Review [CLAUDE.md](CLAUDE.md) or [TEMPLATE_USAGE_GUIDE.md](TEMPLATE_USAGE_GUIDE.md)
- **Issues**: Check troubleshooting above
- **Contribute**: Improve the template and share back

**Happy microservice creating! 🚀**
