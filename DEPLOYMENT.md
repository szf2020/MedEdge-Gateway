# MedEdge Gateway - Deployment Guide

**Version:** 1.0.0-beta
**Last Updated:** 2026-01-16
**Status:** Production-Ready (Phases 1-4 Complete)

## Table of Contents

1. [Quick Start](#quick-start)
2. [Prerequisites](#prerequisites)
3. [Deployment Options](#deployment-options)
4. [Local Development Setup](#local-development-setup)
5. [Docker Compose Deployment](#docker-compose-deployment)
6. [Kubernetes Deployment](#kubernetes-deployment)
7. [Monitoring & Logging](#monitoring--logging)
8. [Troubleshooting](#troubleshooting)
9. [Performance Tuning](#performance-tuning)
10. [Security Hardening](#security-hardening)

## Quick Start

### Fastest Deployment (Docker Compose)

```bash
# Clone repository
git clone https://github.com/bejranonda/MedEdge-Gateway.git
cd MedEdge

# Start all services
docker-compose up -d

# Wait for services to initialize (30-60 seconds)
sleep 60

# Verify deployment
curl http://localhost:5001/health          # FHIR API
curl http://localhost:5000                  # Dashboard
curl -i telnet localhost 1883               # MQTT

# Access dashboard
open http://localhost:5000                  # macOS
start http://localhost:5000                 # Windows
xdg-open http://localhost:5000              # Linux
```

**Services Available:**
- 🟢 **Dashboard:** http://localhost:5000 (Blazor WebAssembly)
- 🟢 **FHIR API:** http://localhost:5001/swagger (REST API)
- 🟢 **MQTT Broker:** localhost:1883 (Message broker)
- 🟢 **All other services:** Running in Docker network

## Prerequisites

### System Requirements

**Minimum:**
- 4GB RAM
- 2 CPU cores
- 10GB disk space
- Docker & Docker Compose (latest)
- .NET 8.0 SDK (for local development only)

**Recommended:**
- 8GB+ RAM
- 4+ CPU cores
- 50GB disk space
- Docker Desktop with 4GB memory allocation
- Git for version control

### Software Requirements

| Component | Version | Purpose |
|-----------|---------|---------|
| Docker | 20.10+ | Container runtime |
| Docker Compose | 3.8+ | Multi-container orchestration |
| .NET SDK | 8.0 | Build applications (dev only) |
| Git | 2.30+ | Version control |
| Nginx | Alpine | Dashboard web server |
| Eclipse Mosquitto | 2.0 | MQTT broker |

### Installation

**macOS (Homebrew):**
```bash
# Install Docker Desktop
brew install docker

# Install Git
brew install git
```

**Windows:**
- Download [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop)
- Download [Git for Windows](https://git-scm.com/download/win)
- Install and start Docker Desktop

**Linux (Ubuntu/Debian):**
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.20.0/docker-compose-$(uname -s)-$(uname -m)" \
  -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Install Git
sudo apt-get install git
```

## Deployment Options

### Option 1: Docker Compose (Recommended for Development/Demo)

**Ideal for:** Quick demo, local testing, small deployments

**Advantages:**
- ✅ Single command deployment
- ✅ All services in one network
- ✅ Automatic service startup/restart
- ✅ Health checks included

**Steps:** See [Docker Compose Deployment](#docker-compose-deployment)

### Option 2: Local Development (.NET)

**Ideal for:** Development, debugging, custom modifications

**Advantages:**
- ✅ Full IDE support
- ✅ Breakpoint debugging
- ✅ Hot reload capabilities
- ✅ Detailed error messages

**Prerequisites:** .NET 8.0 SDK + Visual Studio 2022 / VS Code

**Steps:** See [Local Development Setup](#local-development-setup)

### Option 3: Kubernetes (Production)

**Ideal for:** Enterprise deployments, high availability, multi-region

**Advantages:**
- ✅ Auto-scaling
- ✅ Self-healing
- ✅ Rolling updates
- ✅ Multi-replica deployment

**Prerequisites:** Kubernetes 1.24+, Helm 3+

**Steps:** See [Kubernetes Deployment](#kubernetes-deployment)

### Option 4: Azure Container Instances

**Ideal for:** Serverless containers, on-demand workloads

**CLI:**
```bash
# Create resource group
az group create --name MedEdgeRG --location eastus

# Deploy container group
az container create --resource-group MedEdgeRG \
  --name medEdge-deployment \
  --image medEdge:latest \
  --cpu 2 --memory 4
```

## Local Development Setup

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 (Community/Pro/Enterprise) or VS Code
- Docker (for MQTT broker)
- Git

### Setup Steps

**1. Clone Repository**
```bash
git clone https://github.com/bejranonda/MedEdge-Gateway.git
cd MedEdge
```

**2. Start MQTT Broker**
```bash
docker run -d \
  --name mosquitto \
  -p 1883:1883 \
  -v $(pwd)/mosquitto/config:/mosquitto/config \
  eclipse-mosquitto:2.0
```

**3. Open in IDE**

**Visual Studio 2022:**
```bash
# Open solution
open MedEdge.sln
```

**VS Code:**
```bash
code .
```

**4. Restore Dependencies**
```bash
dotnet restore
```

**5. Build Solution**
```bash
dotnet build --configuration Release
```

**6. Run Services (6 Terminal Windows)**

**Terminal 1: Device Simulator**
```bash
cd src/Edge/MedEdge.DeviceSimulator
dotnet run
```

**Terminal 2: Edge Gateway**
```bash
cd src/Edge/MedEdge.EdgeGateway
dotnet run
```

**Terminal 3: FHIR API**
```bash
cd src/Cloud/MedEdge.FhirApi
dotnet run
# Swagger: http://localhost:5000/swagger
```

**Terminal 4: Transform Service**
```bash
cd src/Cloud/MedEdge.TransformService
dotnet run
```

**Terminal 5: AI Engine**
```bash
cd src/Cloud/MedEdge.AiEngine
dotnet run
```

**Terminal 6: Dashboard (Optional - Hot Reload)**
```bash
cd src/Web/MedEdge.Dashboard
dotnet run
# Dashboard: https://localhost:7000
```

**7. Verify Services**
```bash
# Check FHIR API
curl http://localhost:5000/health

# Check MQTT connectivity
docker exec mosquitto mosquitto_sub -h localhost -t "#" -v

# Run tests
dotnet test
```

## Docker Compose Deployment

### File Structure

```bash
MedEdge/
├── docker-compose.yml        # Main orchestration file
├── src/
│   ├── Edge/
│   │   ├── MedEdge.DeviceSimulator/Dockerfile
│   │   └── MedEdge.EdgeGateway/Dockerfile
│   ├── Cloud/
│   │   ├── MedEdge.FhirApi/Dockerfile
│   │   ├── MedEdge.TransformService/Dockerfile
│   │   └── MedEdge.AiEngine/Dockerfile
│   └── Web/
│       └── MedEdge.Dashboard/Dockerfile
└── mosquitto/
    └── config/mosquitto.conf
```

### Quick Deploy

```bash
# Build all images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Clean up volumes
docker-compose down -v
```

### Verify Deployment

```bash
# Check running containers
docker-compose ps

# Expected output:
# NAME                IMAGE                      STATUS
# medEdge-mqtt        eclipse-mosquitto:2.0      Up 2 minutes
# medEdge-simulator   medEdge-simulator:latest   Up 2 minutes
# medEdge-gateway     medEdge-gateway:latest     Up 2 minutes
# medEdge-fhir-api    medEdge-fhir-api:latest    Up 2 minutes
# medEdge-transform   medEdge-transform:latest   Up 2 minutes
# medEdge-ai-engine   medEdge-ai-engine:latest   Up 2 minutes
# medEdge-dashboard   medEdge-dashboard:latest   Up 2 minutes

# Test endpoints
curl http://localhost:5001/health          # FHIR API health
curl http://localhost:5000                  # Dashboard
curl http://localhost:5001/fhir/Patient    # FHIR data

# View service logs
docker-compose logs fhir-api
docker-compose logs dashboard
docker-compose logs mosquitto
```

### Configuration via Environment Variables

Create `.env` file in project root:

```bash
# MQTT Configuration
MQTT_BROKER=mosquitto
MQTT_PORT=1883
MQTT_TLS=false

# FHIR API Configuration
FHIR_API_PORT=5001
DB_CONNECTION=Data Source=/app/data/medEdge.db

# Dashboard Configuration
DASHBOARD_PORT=5000
API_BASE_URL=http://localhost:5001

# Logging Level
LOG_LEVEL=Information

# Environment
ENVIRONMENT=Production
```

Load in docker-compose.yml:
```yaml
env_file:
  - .env
```

### Port Mapping

| Service | Internal | External | Protocol |
|---------|----------|----------|----------|
| MQTT | 1883 | 1883 | TCP |
| MQTT WebSocket | 9001 | 9001 | WebSocket |
| FHIR API | 8080 | 5001 | HTTP |
| Dashboard | 8080 | 5000 | HTTP |
| Simulator | (internal) | - | Modbus |
| Gateway | (internal) | - | Internal |
| Transform | (internal) | - | Internal |
| AI Engine | (internal) | - | Internal |

## Kubernetes Deployment

### Prerequisites

```bash
# Verify Kubernetes
kubectl version --client

# Verify Helm
helm version

# Create namespace
kubectl create namespace medEdge
```

### Helm Chart Structure

```bash
helm/medEdge/
├── Chart.yaml
├── values.yaml
├── templates/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── ingress.yaml
│   ├── configmap.yaml
│   └── pvc.yaml
```

### Deploy to Kubernetes

```bash
# Create namespace
kubectl create namespace medEdge

# Create config
kubectl create configmap medEdge-config \
  --from-literal=ENVIRONMENT=Production \
  --from-literal=LOG_LEVEL=Information \
  -n medEdge

# Deploy Helm chart
helm install medEdge ./helm/medEdge \
  --namespace medEdge \
  --values helm/values-prod.yaml

# Verify deployment
kubectl get pods -n medEdge
kubectl get svc -n medEdge

# Port forward for access
kubectl port-forward -n medEdge svc/medEdge-dashboard 5000:80 &
kubectl port-forward -n medEdge svc/medEdge-fhir-api 5001:80 &

# View logs
kubectl logs -n medEdge deployment/medEdge-fhir-api -f
```

## Monitoring & Logging

### Serilog Configuration

Edit `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/medEdge-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

### View Logs

**Docker Compose:**
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs -f fhir-api

# Last 100 lines
docker-compose logs --tail=100 transform-service
```

**Local Development:**
```bash
# Logs directory
ls -la logs/

# Watch logs
tail -f logs/medEdge-*.txt
```

**Kubernetes:**
```bash
# Pod logs
kubectl logs -n medEdge deployment/medEdge-fhir-api

# Stream logs
kubectl logs -n medEdge deployment/medEdge-fhir-api -f

# Previous logs (after crash)
kubectl logs -n medEdge deployment/medEdge-fhir-api --previous
```

### Health Checks

**FHIR API Health:**
```bash
curl http://localhost:5001/health

# Response:
# {"status":"healthy"}
```

**Docker Health Status:**
```bash
docker-compose ps

# Healthy containers show "Up"
# Unhealthy containers show "Restarting"
```

**All Services Status:**
```bash
# MQTT
mosquitto_pub -h localhost -t "health/check" -m "ping"

# Devices
curl http://localhost:5001/api/devices

# FHIR Data
curl http://localhost:5001/fhir/Patient
```

## Troubleshooting

### Common Issues & Solutions

**Issue 1: Ports Already in Use**

```bash
# Find process using port 5001
lsof -i :5001  # macOS/Linux
netstat -ano | findstr :5001  # Windows

# Kill process
kill -9 <PID>  # macOS/Linux
taskkill /PID <PID> /F  # Windows
```

**Issue 2: Docker Volume Permissions**

```bash
# Fix volume ownership
sudo chown -R $(id -u):$(id -g) mosquitto/
sudo chown -R $(id -u):$(id -g) data/
```

**Issue 3: MQTT Connection Refused**

```bash
# Check MQTT is running
docker-compose ps | grep mosquitto

# Restart MQTT
docker-compose restart mosquitto

# Check logs
docker-compose logs mosquitto
```

**Issue 4: Database Locked**

```bash
# Delete corrupted database
docker-compose down -v
rm -f data/medEdge.db

# Restart
docker-compose up -d
```

**Issue 5: Dashboard Not Loading**

```bash
# Check dashboard service
docker-compose logs dashboard

# Verify port is open
curl http://localhost:5000

# Restart dashboard
docker-compose restart dashboard
```

**Issue 6: High Memory Usage**

```bash
# Check resource usage
docker stats

# Reduce memory in docker-compose.yml
# Change deploy.resources.limits.memory to lower value

# Restart with lower memory
docker-compose restart
```

## Performance Tuning

### Database Optimization

```csharp
// Connection pooling (default: 25)
options.UseNpgsql(connectionString, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
    sqlOptions.MaxPoolSize = 50;
    sqlOptions.MinPoolSize = 10;
});

// Query optimization
var observations = await context.Observations
    .AsNoTracking()
    .Where(o => o.PatientId == patientId)
    .Select(o => new { o.Id, o.Value })
    .ToListAsync();
```

### MQTT Optimization

```yaml
# mosquitto.conf
max_connections -1
max_queued_messages 100
persistence_changeset_interval 20
retain_memory_threshold 128000
```

### Network Optimization

```yaml
# docker-compose.yml
networks:
  medEdge-network:
    driver: bridge
    driver_opts:
      com.docker.network.driver.mtu: 1500
```

### Caching Strategy

```csharp
// Cache observations for 5 minutes
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// In service
var cacheKey = $"observations_{patientId}";
if (!cache.TryGetValue(cacheKey, out var observations))
{
    observations = await repo.GetObservations(patientId);
    cache.Set(cacheKey, observations, TimeSpan.FromMinutes(5));
}
```

## Security Hardening

### Enable TLS/SSL

**Nginx Configuration:**
```nginx
server {
    listen 443 ssl http2;
    ssl_certificate /etc/nginx/certs/server.crt;
    ssl_certificate_key /etc/nginx/certs/server.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
}
```

### MQTT TLS

```yaml
# docker-compose.yml
mosquitto:
  ports:
    - "8883:8883"  # TLS port
  volumes:
    - ./certs:/mosquitto/certs:ro
```

**mosquitto.conf:**
```
listener 8883
certfile /mosquitto/certs/server.crt
keyfile /mosquitto/certs/server.key
protocol mqtt
```

### API Authentication

```csharp
// Add JWT authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-auth-server";
        options.Audience = "medEdge-api";
    });

// Require auth on endpoints
app.MapGet("/fhir/Patient", async (IFhirRepository repo) => ...)
    .RequireAuthorization();
```

### Database Encryption

```bash
# SQLite encryption
PRAGMA key = 'your-encryption-key';
PRAGMA cipher = 'sqlcipher';
```

### Environment Secrets

```bash
# Use .env.local (git ignored)
cat > .env.local << EOF
MYSQL_ROOT_PASSWORD=secure_password
JWT_KEY=your-secret-key
API_KEY=your-api-key
EOF

# Load in docker-compose.yml
env_file:
  - .env.local
```

### Network Security

```yaml
# docker-compose.yml
networks:
  medEdge-network:
    internal: true  # Isolated network
    driver: bridge
```

## Backup & Recovery

### Database Backup

```bash
# SQLite backup
docker-compose exec fhir-api \
  sqlite3 /app/data/medEdge.db ".backup /app/backup/medEdge-backup.db"

# Copy to host
docker cp medEdge-fhir-api:/app/backup/medEdge-backup.db ./backups/
```

### Volume Backup

```bash
# Backup named volume
docker run --rm -v fhir-data:/data -v $(pwd)/backups:/backup \
  busybox tar czf /backup/fhir-data-$(date +%Y%m%d).tar.gz -C /data .
```

### Restore from Backup

```bash
# Stop services
docker-compose down

# Restore database
docker run --rm -v fhir-data:/data -v $(pwd)/backups:/backup \
  busybox tar xzf /backup/fhir-data-latest.tar.gz -C /data

# Start services
docker-compose up -d
```

## Production Checklist

- [ ] Enable TLS/SSL for all endpoints
- [ ] Configure proper logging levels
- [ ] Set up database backups (daily)
- [ ] Configure monitoring and alerts
- [ ] Enable authentication/authorization
- [ ] Test disaster recovery procedures
- [ ] Document deployment architecture
- [ ] Set up auto-scaling policies
- [ ] Configure rate limiting
- [ ] Enable audit logging

## Support & Documentation

**Additional Resources:**
- [Architecture Documentation](docs/ARCHITECTURE.md)
- [FHIR Mapping Guide](docs/FHIR-MAPPING.md)
- [Implementation Summary](IMPLEMENTATION.md)
- [Project Status](PROJECT-STATUS.md)

**Getting Help:**
- GitHub Issues: https://github.com/bejranonda/MedEdge-Gateway/issues
- Documentation: See `docs/` directory
- Contact: See repository README

---

**Version:** 1.0.0-beta
**Status:** Production-Ready
**Last Updated:** 2026-01-16
