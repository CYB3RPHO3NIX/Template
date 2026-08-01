# Microservice Generator CLI - Quick Reference

**One-command microservice generation with a standalone executable.**

## 🎯 Quick Start

```bash
# Windows
microservice-gen -n InvoiceService -o C:\Projects

# Linux/Mac
./microservice-gen -n InvoiceService -o ~/projects
```

**That's it!** Your complete microservice is ready. 🚀

---

## 📦 Build

### Windows (PowerShell)

```powershell
cd MicroserviceGenerator
.\build.ps1
# Output: build\bin\Release\microservice-gen.exe
```

### Linux/Mac (Bash)

```bash
cd MicroserviceGenerator
chmod +x build.sh
./build.sh
# Output: build/bin/microservice-gen
```

---

## 🚀 Usage

### Basic Command

```
microservice-gen -n <ServiceName> -o <OutputPath> [-c <CompanyName>]
```

### Examples

| Use Case | Command |
|----------|---------|
| **Simple** | `microservice-gen -n InvoiceService -o C:\Projects` |
| **With company** | `microservice-gen -n OrderService -o ~/projects -c AcmeCorp` |
| **Help** | `microservice-gen --help` |
| **Version** | `microservice-gen --version` |

### Real-World Examples

**Invoice Microservice:**
```bash
microservice-gen -n InvoiceService -o C:\Projects
cd C:\Projects\InvoiceService
dotnet build
dotnet run --project InvoiceService.API
```

**Multiple Services (PowerShell):**
```powershell
$services = @("Invoice", "Order", "Payment")
foreach ($svc in $services) {
    microservice-gen -n "$($svc)Service" -o C:\Projects -c MyCompany
}
```

**Multiple Services (Bash):**
```bash
for service in Invoice Order Payment; do
    ./microservice-gen -n "${service}Service" -o ~/projects -c MyCompany
done
```

---

## 📊 Generated Output

```
InvoiceService/
├── InvoiceService.API/
├── InvoiceService.Consumer/
├── InvoiceService.Commands/
├── InvoiceService.Queries/
├── InvoiceService.Events/
├── InvoiceService.Database/
└── InvoiceService.sln
```

**Ready to:**
- ✅ Build: `dotnet build`
- ✅ Run: `dotnet run --project InvoiceService.API`
- ✅ Deploy: Push to Git and CI/CD
- ✅ Develop: Add your domain logic

---

## ⚡ Performance

| Task | Time |
|------|------|
| Generate service | ~1 second |
| Build generated service | ~5-10 seconds |
| **Total setup time** | **~15-20 seconds** |

---

## 🔗 Arguments Reference

```
-n, --name <ServiceName>      Service name (Required)
-o, --output <Path>           Output directory (Required)
-c, --company <CompanyName>   Company name (Optional, default: Enterprise)
-h, --help                    Show help
-v, --version                 Show version
```

---

## ❌ Common Issues

| Issue | Solution |
|-------|----------|
| Binary not found | Run `build.ps1` or `build.sh` first |
| "Command not found" | Add to PATH or use full path: `./microservice-gen` |
| "Permission denied" (Linux/Mac) | Run `chmod +x microservice-gen` |
| "Project already exists" | Use different output directory |
| Invalid service name | Use PascalCase (e.g., `InvoiceService`) |

---

## 📋 Validation Rules

| Input | Rule | Example |
|-------|------|---------|
| Service Name | PascalCase, alphanumeric | ✅ `InvoiceService` ❌ `invoice-service` |
| Output Path | Must exist | `C:\Projects` must exist |
| Project Name | Cannot exist | Cannot create duplicate service |
| Company Name | Alphanumeric, spaces OK | ✅ `Acme Corp` ✅ `AcmeCorp` |

---

## 🎓 Next Steps

After generation:

```bash
cd InvoiceService
dotnet build           # Verify everything compiles
dotnet test            # Run existing tests
dotnet run --project InvoiceService.API  # Start API server
```

Then open `https://localhost:7001/swagger` to see API documentation.

---

## 📚 Documentation

- **Full Guide:** [CLI_BUILD_GUIDE.md](CLI_BUILD_GUIDE.md)
- **Architecture:** [../CLAUDE.md](../CLAUDE.md)
- **Examples:** [../TEMPLATE_EXAMPLES.md](../TEMPLATE_EXAMPLES.md)

---

## 💡 Pro Tips

1. **Create consistent services:** Use same `-c` (company name) for all services
2. **Organize by tier:** Create separate output directories for different service tiers
3. **Version control:** Initialize Git immediately after generation
4. **Batch operations:** Loop through multiple service names for rapid creation

---

## 🔐 Security

✅ No external dependencies
✅ No network calls
✅ No arbitrary code execution
✅ Input validation on all parameters
✅ File operations restricted to output directory

---

**Time to create your first microservice: < 1 minute** ⚡
