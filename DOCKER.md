# Docker Deployment Guide

This document describes how to build and run the Template microservice solution using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (v4.0+) installed and running
- Docker Compose (v2.0+)
- At least 4GB of RAM allocated to Docker
- Port 8080 (API), 1433 (SQL Server) available on your machine

## Quick Start

### Build and Start All Services

```bash
# From the project root directory
docker-compose up --build
```

This will:
1. Build the API Docker image
2. Build the Consumer Docker image
3. Start SQL Server 2022 container
4. Start the API service (port 8080)
5. Start the Consumer service
5. Initialize the database (via EF Core migrations)

### Access the Application

- **API Swagger Documentation**: http://localhost:8080/swagger
- **API Health Check**: http://localhost:8080/health
- **Liveness Probe**: http://localhost:8080/health/live
- **Readiness Probe**: http://localhost:8080/health/ready

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f consumer
docker-compose logs -f sql-server
```

### Stop Services

```bash
# Stop all containers (data persists in volumes)
docker-compose down

# Stop and remove all data
docker-compose down -v
```

## Service Details

### SQL Server (sql-server)
- **Image**: mcr.microsoft.com/mssql/server:2022-latest
- **Port**: 1433
- **SA Password**: `P@ssw0rd123!`
- **Database**: TemplateDb
- **Data Volume**: `sql-data` (persists on host)

### API Service (api)
- **Build**: Multi-stage Dockerfile optimizing image size
- **Base Image**: mcr.microsoft.com/dotnet/aspnet:10.0
- **Port**: 8080
- **Environment**: Docker
- **Health Check**: HTTP GET /health/live every 30s
- **Dependencies**: Requires healthy SQL Server

### Consumer Service (consumer)
- **Build**: Multi-stage Dockerfile for Worker Service
- **Base Image**: mcr.microsoft.com/dotnet/runtime:10.0
- **Environment**: Docker
- **Message Queue**: InMemory (default, configurable via MessageQueue__Type env var)
- **Dependencies**: Requires SQL Server and API service

## Configuration

### Environment Variables

Environment variables can be overridden via:

1. **docker-compose.yml** - recommended for development
2. **.env file** - create `.env` in project root with key=value pairs:

```env
MSSQL_SA_PASSWORD=YourSecurePassword123!
ConnectionStrings__DefaultConnection=Data Source=sql-server,1433;Initial Catalog=TemplateDb;User Id=sa;Password=YourSecurePassword123!;Encrypt=False;Trust Server Certificate=True
JwtSettings__Secret=your-very-long-production-secret-key-at-least-32-characters
```

### Important Settings for Production

**JWT Secret** (CRITICAL)
```env
JwtSettings__Secret=your-very-long-secret-key-change-this-in-production-at-least-32-characters-for-production-use
```

**SQL Server Password** (CRITICAL)
```env
MSSQL_SA_PASSWORD=ChooseAStrongPassword123!
```

**Database Connection String**
- Automatically configured in docker-compose.yml
- Format: `Data Source=sql-server,1433;Initial Catalog=TemplateDb;User Id=sa;Password=<MSSQL_SA_PASSWORD>;Encrypt=False;Trust Server Certificate=True`

## Network Architecture

Services communicate via the `template-network` bridge network:
- API ↔ SQL Server
- Consumer ↔ SQL Server
- Consumer ↔ API (if needed for commands/queries)

External access:
- API: 0.0.0.0:8080 → container 8080
- SQL Server: 0.0.0.0:1433 → container 1433

## Database Initialization

The API service automatically initializes the database on startup:
1. EF Core migrations are applied
2. Initial schema is created
3. Seed data is populated (if configured)

Monitor startup logs:
```bash
docker-compose logs -f api | grep -i "migration\|database"
```

## Development Workflow

### Code Changes

1. Make code changes in your local project
2. Rebuild and restart services:
```bash
docker-compose up --build
```

### Database Changes

After adding EF Core migrations:
```bash
# Rebuild and restart (migrations run automatically)
docker-compose up --build
```

### Clean Rebuild

```bash
# Remove all containers, networks, and volumes
docker-compose down -v

# Rebuild from scratch
docker-compose up --build
```

## Troubleshooting

### API Cannot Connect to Database

```bash
# Check SQL Server container is healthy
docker-compose ps

# Check SQL Server logs
docker-compose logs sql-server

# Test connection manually
docker exec -it template-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'P@ssw0rd123!' -Q 'SELECT @@VERSION'
```

### API Container Exits Immediately

```bash
# Check API logs for errors
docker-compose logs api

# Ensure SQL Server is healthy first
docker-compose logs sql-server | grep -i "health\|started"
```

### Port Already in Use

```bash
# Find process using port 8080
netstat -ano | findstr :8080  # Windows
lsof -i :8080                  # macOS/Linux

# Change port in docker-compose.yml:
# ports:
#   - "8081:8080"  # Use 8081 instead
```

### Database Locked

```bash
# Restart containers
docker-compose restart

# If that fails, remove containers and volumes
docker-compose down -v
docker-compose up
```

## Production Deployment

For production deployment:

1. **Use environment-specific docker-compose files**:
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.prod.yml up
   ```

2. **Configure secrets management**:
   - Use Docker Secrets (Swarm) or Kubernetes Secrets
   - Never commit `.env` files to git
   - Use external secret stores (Vault, Azure Key Vault, etc.)

3. **Use dedicated SQL Server**:
   - Replace local SQL Server with managed instance
   - Update connection string in environment variables
   - Ensure proper SSL/TLS encryption

4. **Scale services**:
   ```yaml
   api:
     deploy:
       replicas: 3
   consumer:
     deploy:
       replicas: 2
   ```

5. **Enable logging to central system**:
   - Configure Serilog to use external sink
   - Aggregate logs from all containers
   - Use ELK, Datadog, or similar

6. **Use image registries**:
   - Push images to Docker Hub, ECR, ACR, or GCR
   - Use versioned tags (e.g., `myrepo/template-api:1.0.0`)
   - Reference registry in docker-compose.yml

Example production docker-compose snippet:
```yaml
api:
  image: myregistry/template-api:1.0.0
  ports:
    - "80:8080"
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - JwtSettings__Secret=${JWT_SECRET}  # From secret store
    - ConnectionStrings__DefaultConnection=${DB_CONNECTION_STRING}
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost:8080/health/live"]
    interval: 30s
    timeout: 10s
    retries: 3
    start_period: 40s
```

## Docker Image Information

### API Image
- **Size**: ~400-500 MB (multi-stage build)
- **Layers**: 
  - Base runtime: aspnet:10.0 (~200 MB)
  - Application: .NET assemblies and dependencies
- **Scan for vulnerabilities**: `docker scout cves template-api:latest`

### Consumer Image
- **Size**: ~300-400 MB (multi-stage build)
- **Layers**:
  - Base runtime: runtime:10.0 (~120 MB)
  - Application: .NET assemblies and dependencies

## Kubernetes Deployment

For Kubernetes deployment, reference the Dockerfiles and convert docker-compose.yml to Kubernetes manifests:

1. Create ConfigMap for appsettings
2. Create Secret for sensitive configuration
3. Create Deployment for API service
4. Create Deployment for Consumer service
5. Create StatefulSet for SQL Server (or use managed database)
6. Create Service and Ingress for external access

Example:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: template-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: template-api
  template:
    metadata:
      labels:
        app: template-api
    spec:
      containers:
      - name: api
        image: myregistry/template-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: template-secrets
              key: db-connection-string
        livenessProbe:
          httpGet:
            path: /health/live
            port: 8080
          initialDelaySeconds: 40
          periodSeconds: 30
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 8080
          initialDelaySeconds: 20
          periodSeconds: 10
```

## Monitoring and Observability

With Docker, enable monitoring via:

1. **Health Checks** - Built-in to docker-compose (3 endpoints)
2. **Logging** - Configure Serilog sinks for centralized logging
3. **Metrics** - Add Prometheus exporter and Grafana dashboards
4. **Tracing** - Enable distributed tracing with OpenTelemetry

Configure in appsettings.json or environment:
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://seq:5341" }
      }
    ]
  }
}
```

## Cleanup

Remove all Docker artifacts:
```bash
# Stop and remove containers, networks, volumes
docker-compose down -v

# Remove images
docker rmi template-api:latest template-consumer:latest

# Clean up dangling images
docker image prune -f
```
