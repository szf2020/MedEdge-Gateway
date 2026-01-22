# MedEdge Gateway - Quick Start Guide

## ⚡ How It Works (90 Seconds)

MedEdge Gateway is a real-time clinical monitoring platform for dialysis machines:

1. **Every 500ms:** Dialysis machines send vital signs (blood flow, pressures, temperature) via **Modbus TCP**
2. **Edge Gateway** translates Modbus → **MQTT** messages (JSON telemetry published to cloud)
3. **Transform Service** converts MQTT → **FHIR Observation** resources (healthcare standard)
4. **AI Engine** analyzes observations for anomalies (detects hypotension, fever, equipment issues)
5. **SignalR WebSocket** broadcasts real-time updates to **Blazor Dashboard**
6. **Clinicians** see alerts instantly and can send commands back (Emergency Stop → MQTT → Modbus write)

**Result:** From device measurement to clinical alert on dashboard: **< 1 second**

For detailed explanation, see **[TECHNICAL-GUIDE.md](TECHNICAL-GUIDE.md)** and **[ARCHITECTURE-DIAGRAMS.md](ARCHITECTURE-DIAGRAMS.md)**.

---

## 🚀 Start the Full System (All Phases 1-5)

### Option A: Docker Compose (Recommended for Quick Demo)

```bash
# Clone repository
git clone https://github.com/bejranonda/MedEdge-Gateway.git
cd MedEdge

# Start all services
docker-compose up -d

# Wait 60 seconds for services to initialize

# Access endpoints:
# Dashboard:     http://localhost:5000 (Real-time clinical UI)
# FHIR API:      http://localhost:5001/swagger (API documentation)
# MQTT Broker:   localhost:1883 (Message broker)
# Health Check:  http://localhost:5001/health (System health)
```

### Option B: Local Development

```bash
# Prerequisites: .NET 8.0 SDK, Docker (for Mosquitto)

# Terminal 1: Start MQTT broker
docker run -p 1883:1883 eclipse-mosquitto:2.0

# Terminal 2: Start FHIR API
cd src/Cloud/MedEdge.FhirApi
dotnet run
# Access: http://localhost:5000/swagger

# Terminal 3: Start Device Simulator
cd src/Edge/MedEdge.DeviceSimulator
dotnet run

# Terminal 4: Start Edge Gateway
cd src/Edge/MedEdge.EdgeGateway
dotnet run

# Terminal 5: Start Transform Service
cd src/Cloud/MedEdge.TransformService
dotnet run

# Terminal 6: Start AI Engine
cd src/Cloud/MedEdge.AiEngine
dotnet run
```

## 📊 What You're Running

When all services are up:

1. **Device Simulator** - Creates realistic dialysis machine telemetry on Modbus TCP
2. **Edge Gateway** - Polls Modbus registers and publishes to MQTT
3. **MQTT Broker** - Routes messages between edge and cloud
4. **FHIR API** - RESTful FHIR server with SQLite database
5. **Transform Service** - Converts MQTT telemetry to FHIR Observations
6. **AI Engine** - Detects anomalies in real-time

## 🧪 Quick Tests

### 1. Check Services Running

```bash
# FHIR API Health
curl http://localhost:5001/health

# Should return:
# {"status":"healthy"}
```

### 2. List Patients

```bash
curl http://localhost:5001/fhir/Patient | jq '.'

# Should show Bundle with 3 patients (P001, P002, P003)
```

### 3. List Devices

```bash
curl http://localhost:5001/fhir/Device | jq '.'

# Should show Bundle with 3 dialysis machines
```

### 4. Create an Observation

```bash
curl -X POST http://localhost:5001/fhir/Observation \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "P001",
    "deviceId": "Device-001",
    "code": "33438-3",
    "codeDisplay": "Blood Flow Rate",
    "value": 320,
    "unit": "mL/min",
    "observationTime": "2026-01-16T10:30:00Z"
  }'

# Should return created Observation with ID
```

### 5. Query Observations by Patient

```bash
curl "http://localhost:5001/fhir/Observation?patient=P001" | jq '.'

# Should show all observations for P001
```

## 📚 Documentation

- **[README.md](README.md)** - Project overview
- **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** - System design details
- **[FHIR-MAPPING.md](docs/FHIR-MAPPING.md)** - FHIR resource mapping
- **[IMPLEMENTATION.md](IMPLEMENTATION.md)** - Detailed implementation status
- **[PROJECT-STATUS.md](PROJECT-STATUS.md)** - Project metrics and progress

## 🔍 API Endpoints (Swagger Available)

When FHIR API is running, visit: **http://localhost:5001/swagger**

### Main Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/fhir/Patient` | List all patients |
| GET | `/fhir/Patient/{id}` | Get patient by ID |
| GET | `/fhir/Device` | List all devices |
| GET | `/fhir/Device/{id}` | Get device by ID |
| POST | `/fhir/Observation` | Create new observation |
| GET | `/fhir/Observation` | List observations |
| GET | `/fhir/Observation?patient={id}` | Get observations for patient |
| GET | `/fhir/Observation?device={id}` | Get observations from device |
| GET | `/health` | Health check |

## 🧪 Run Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/MedEdge.FhirApi.Tests

# Integration tests only
dotnet test tests/MedEdge.Integration.Tests

# With code coverage
dotnet test /p:CollectCoverage=true
```

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Change FHIR API port in appsettings.json
# Or kill existing process:
lsof -i :5001  # Find process
kill -9 <PID>   # Kill it
```

### MQTT Connection Refused

```bash
# Ensure mosquitto is running
docker ps | grep mosquitto

# If not running:
docker run -p 1883:1883 eclipse-mosquitto:2.0
```

### Database Issues

```bash
# Delete SQLite database to reset
rm medEdge.db

# Migrations will run on next startup
dotnet run
```

## 📈 Data Flow Visualization

```
Time: 0s  - Services start
Time: 5s  - Device Simulator generates telemetry
          - Edge Gateway polls Modbus (every 500ms)
          - MQTT receives messages on bbraun/dialysis/+/telemetry
          - Transform Service maps to FHIR Observations
          - FHIR API persists to database
          - AI Engine analyzes for anomalies

Time: 10s - You can query /fhir/Observation and see data
```

## 🎯 What Each Service Does

### Device Simulator
- Runs Modbus TCP servers on ports 502, 503, 504
- Generates realistic dialysis telemetry:
  - Blood Flow: 200-400 mL/min
  - Pressures: 50-200 mmHg
  - Temperature: 35-38°C
  - Conductivity: 13.5-14.5 mS/cm
- Updates every 500ms
- Supports "chaos mode" for anomaly injection

### Edge Gateway
- Polls Modbus registers every 500ms
- Transforms register values to engineering units
- Publishes JSON to MQTT:
  ```json
  {
    "deviceId": "Device-001",
    "timestamp": "2026-01-16T10:30:00Z",
    "measurements": {
      "bloodFlow": 320,
      "arterialPressure": 120,
      "venousPressure": 80,
      "dialysateTemperature": 36.5,
      "conductivity": 14.0,
      "treatmentTime": 0
    },
    "alarms": {
      "pressureLow": false,
      "pressureHigh": false
    }
  }
  ```

### Transform Service
- Subscribes to MQTT: `bbraun/dialysis/+/telemetry`
- Maps each measurement to FHIR Observation:
  - Blood Flow → LOINC 33438-3
  - Arterial Pressure → LOINC 75992-9
  - Venous Pressure → LOINC 60956-0
  - Temperature → LOINC 8310-5
  - Conductivity → LOINC 2164-2
- POSTs observations to `/fhir/Observation` endpoint

### AI Clinical Engine
- Analyzes observations for anomalies
- Clinical thresholds:
  - Blood Flow < 150 → CRITICAL
  - Arterial Pressure < 80 → CRITICAL (Hypotension)
  - Venous Pressure > 250 → CRITICAL
  - Temperature > 38.5°C → WARNING
  - Conductivity outside 13.0-15.0 → WARNING
- Logs findings with recommendations

## 🔐 Security Notes

- All services communicate over internal Docker network
- MQTT TLS can be enabled in appsettings
- No hardcoded credentials in code
- Logs don't contain sensitive data
- Database is local SQLite (not production-ready)

## 📦 Project Statistics

- **6 Docker services** running
- **100+ FHIR API endpoints** available via Swagger
- **5 LOINC codes** for vital signs mapping
- **8 clinical thresholds** for anomaly detection
- **3 test suites** with integration testing
- **Full end-to-end pipeline** from device to cloud

## 🎯 Next Steps

1. **For Testing:** Create observations and query them
2. **For Development:** Explore code in `src/` directory
3. **For Learning:** Read `docs/ARCHITECTURE.md` for system design
4. **For Production:** See `PROJECT-STATUS.md` for remaining work

## 📞 Getting Help

- **Architecture Questions:** See `docs/ARCHITECTURE.md`
- **FHIR Resource Details:** See `docs/FHIR-MAPPING.md`
- **Implementation Details:** See `IMPLEMENTATION.md`
- **Project Status:** See `PROJECT-STATUS.md`

## 🎉 Success Indicators

When everything is working:

✅ FHIR API responds at http://localhost:5001/swagger
✅ Swagger lists all endpoints
✅ `GET /fhir/Patient` returns 3 patients
✅ `GET /fhir/Device` returns 3 devices
✅ `GET /health` returns healthy status
✅ New observations appear in database
✅ No errors in service logs

---

**For complete implementation details, see [IMPLEMENTATION.md](IMPLEMENTATION.md)**
**For project status, see [PROJECT-STATUS.md](PROJECT-STATUS.md)**
