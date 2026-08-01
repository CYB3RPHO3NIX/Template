# Quick Start - Official Visual Studio Template

Generate enterprise CQRS microservices using the official .NET template.

## ⚡ 60-Second Setup

### 1. Install Template (One-Time)
```bash
cd Template
dotnet new install .
```

### 2. Create Microservice
```bash
dotnet new cqrs-microservice -n InvoiceService
```

### 3. Run
```bash
cd InvoiceService
dotnet run --project InvoiceService.API
```

**Done!** Open `https://localhost:7001/swagger`

---

## 📋 Common Commands

```bash
# Create service with custom company name
dotnet new cqrs-microservice -n OrderService -CompanyName "Acme"

# Create in specific directory
dotnet new cqrs-microservice -n PaymentService -o C:\Projects

# Show help
dotnet new cqrs-microservice --help

# List installed templates
dotnet new list | grep cqrs
```

---

## 🎯 Next Steps

1. Review [GETTING_STARTED.md](GETTING_STARTED.md)
2. Check [CLAUDE.md](CLAUDE.md) for architecture
3. See [TEMPLATE_EXAMPLES.md](TEMPLATE_EXAMPLES.md) for real-world examples

---

**That's it!** You now have a production-ready microservice. 🚀
