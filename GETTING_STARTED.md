# Getting Started - Official Visual Studio Template

This is an **official .NET Visual Studio template** for creating enterprise CQRS microservices.

## ⚡ Quick Start

### Step 1: Install Template (One-Time)

```bash
dotnet new install .
```

Run this command from the Template root directory.

### Step 2: Create Your Microservice

```bash
dotnet new cqrs-microservice -n InvoiceService
```

Or with custom company name:

```bash
dotnet new cqrs-microservice -n InvoiceService -CompanyName "AcmeCorp"
```

### Step 3: Build & Run

```bash
cd InvoiceService
dotnet build
dotnet run --project InvoiceService.API
```

Browse to: `https://localhost:7001/swagger`

---

## 📋 Template Parameters

### Required
- `-n, --name <ServiceName>` - Service name (e.g., `InvoiceService`)

### Optional
- `-CompanyName <Name>` - Company namespace (default: `Enterprise`)
- `-o, --output <Path>` - Output directory

### Examples

**Basic:**
```bash
dotnet new cqrs-microservice -n PaymentService
```

**With company:**
```bash
dotnet new cqrs-microservice -n OrderService -CompanyName "MyCompany"
```

**Custom path:**
```bash
dotnet new cqrs-microservice -n InventoryService -o C:\Projects
```

---

## 📦 What You Get

Complete microservice with:
- ✅ REST API (ASP.NET Core)
- ✅ Background Consumer Service
- ✅ CQRS Commands & Queries
- ✅ Event-Driven Architecture
- ✅ Database Models (EF Core)
- ✅ Message Queue Integration
- ✅ Swagger/OpenAPI Docs
- ✅ Structured Logging (Serilog)

---

## 🏗️ Project Structure

```
YourService/
├── YourService.API/              # REST endpoints
├── YourService.Consumer/         # Background service
├── YourService.Commands/         # CQRS commands
├── YourService.CommandHandlers/  # Command logic
├── YourService.Queries/          # CQRS queries
├── YourService.QueryHandlers/    # Query logic
├── YourService.Events/           # Domain events
├── YourService.EventHandlers/    # Event handlers
├── YourService.Database/         # EF Core models
├── YourService.Contracts/        # DTOs & interfaces
└── YourService.sln               # Solution
```

---

## 🔧 Management

### List Installed Templates
```bash
dotnet new list | grep cqrs
```

### Uninstall
```bash
dotnet new uninstall Enterprise.CQRS.Microservice
```

### Update
```bash
dotnet new uninstall Enterprise.CQRS.Microservice
dotnet new install .
```

---

## 📚 Documentation

- **Architecture:** [CLAUDE.md](CLAUDE.md)
- **Queue Setup:** [QUEUE_INITIALIZATION_GUIDE.md](QUEUE_INITIALIZATION_GUIDE.md)
- **Examples:** [TEMPLATE_EXAMPLES.md](TEMPLATE_EXAMPLES.md)
- **Detailed Guide:** [TEMPLATE_USAGE_GUIDE.md](TEMPLATE_USAGE_GUIDE.md)

---

## 🚀 Next Steps

1. Create service: `dotnet new cqrs-microservice -n MyService`
2. Review [CLAUDE.md](CLAUDE.md) for architecture
3. Add your domain entities
4. Create commands and queries
5. Implement handlers
6. Configure database
7. Deploy!

---

**Happy microservice building!** 🎉
