# MedEdge Gateway - System Architecture

## 1. Overview

MedEdge Gateway is a five-layer medical device connectivity platform that demonstrates expertise in Industrial IoT, FHIR healthcare interoperability, and AI-powered clinical intelligence.

## 2. Quick Understanding: How It All Works

For a comprehensive explanation of how the complete system works, see the **[TECHNICAL-GUIDE.md](../TECHNICAL-GUIDE.md)** and **[ARCHITECTURE-DIAGRAMS.md](../ARCHITECTURE-DIAGRAMS.md)** in the root directory.

**30-Second Summary:**
Every 500 milliseconds, dialysis machines transmit vital signs via Modbus TCP. The Edge Gateway translates these to MQTT messages and publishes to the cloud. The Transform Service converts MQTT telemetry into FHIR Observation resources stored in the database. Simultaneously, the AI Engine analyzes each observation for anomalies using clinical thresholds. When an alert occurs (e.g., hypotension detected), it's broadcast via SignalR WebSocket to the Blazor dashboard in real-time. Clinicians see the alert immediately and can take action (e.g., emergency stop) by posting a DeviceRequest through the FHIR API, which gets translated back to MQTT commands and executed on the device.

**Three Key Innovation Points:**
1. **Bi-Directional:** Unlike previous NEXADIA Sync7 (device→cloud only), MedEdge supports cloud→device commands
2. **Real-Time:** Sub-second latency from device telemetry to clinician alert via SignalR WebSockets
3. **Intelligent:** Hybrid AI combines statistical detection (deterministic, reliable) with optional LLM explanations (contextual, insightful)

---

## 2. Layered Architecture

### Layer 1: Edge Computing (Physical Devices)
**Components:**
- Dialog+ and Dialog iQ simulators (Modbus TCP servers)
- Realistic telemetry generation
- Chaos mode for anomaly injection

**Responsibilities:**
- Simulate dialysis machine behavior
- Generate realistic vital sign data
- Respond to Modbus read/write commands

### Layer 2: Edge Gateway (Protocol Translation)
**Components:**
- Modbus TCP client polling devices
- MQTT publisher with TLS
- SQLite offline buffer with store-and-forward
- Polly resilience patterns

**Responsibilities:**
- Poll dialysis machines at 500ms intervals
- Transform Modbus registers to JSON telemetry
- Publish to MQTT broker with TLS encryption
- Buffer messages during network outages
- Implement retry logic with exponential backoff

### Layer 3: Messaging Broker (Pub/Sub)
**Components:**
- Eclipse Mosquitto MQTT broker
- Topic-based message routing
- Persistent message storage

**Topics:**
```
bbraun/dialysis/{deviceId}/telemetry  # Device → Cloud
bbraun/dialysis/{deviceId}/commands   # Cloud → Device
```

### Layer 4: Cloud Intelligence (FHIR & AI)
**Components:**

#### 4a. FHIR API Server
- ASP.NET Core Minimal APIs
- FHIR R4 resource endpoints
- EF Core with SQLite/PostgreSQL
- Swagger/OpenAPI documentation

**Resources:**
- Patient (demographics, MRN)
- Device (dialysis machines)
- Observation (vital signs)
- DiagnosticReport (session summaries)
- DeviceRequest (treatment commands)

#### 4b. Transform Service
- MQTT subscriber
- JSON to FHIR Observation mapper
- LOINC code assignment
- Batch persistence

#### 4c. AI Clinical Engine
**Layer 1: Statistical Detection**
- Z-Score analysis on 20-reading window
- Clinical thresholds:
  - Blood Flow < 150 mL/min → CRITICAL
  - Arterial Pressure < 80 mmHg → WARNING (Hypotension)
  - Venous Pressure > 250 mmHg → CRITICAL
  - Temperature > 38.5°C → WARNING

**Layer 2: LLM-Based Explanations (Optional)**
- Semantic Kernel + Azure OpenAI integration
- Clinical context analysis
- Risk stratification

### Layer 5: Presentation Layer (User Interface)
**Components:**
- Blazor WebAssembly SPA
- MudBlazor component library
- Chart.js real-time waveforms
- SignalR WebSocket push

**Three-Panel Dashboard:**
1. Fleet View - Device status cards
2. Live Vitals - Real-time charts
3. FHIR Inspector - Resource explorer

## 3. Data Flow

### Telemetry Ingestion Pipeline
```
Modbus Registers
    ↓
Edge Gateway (NModbus client)
    ↓
MQTT Publisher (TLS)
    ↓
Mosquitto Broker
    ↓
Transform Service (Subscriber)
    ↓
FHIR Observation Mapper
    ↓
FHIR API (POST /fhir/Observation)
    ↓
SQL Database
    ↓
SignalR Hub (Broadcast)
    ↓
Blazor Dashboard (WebSocket)
```

### Bi-Directional Control Pipeline
```
Dashboard UI (Emergency Stop)
    ↓
FHIR API (POST /fhir/DeviceRequest)
    ↓
Device Command Service
    ↓
MQTT Publisher (Command Topic)
    ↓
Mosquitto Broker
    ↓
Edge Gateway (Subscriber)
    ↓
Modbus Write (Function Code 06)
    ↓
Device Simulator (Set STOP flag)
```

## 4. Key Innovation Features

### 4.1 Bi-Directional FHIR
- **Current NEXADIA Sync7:** Uni-directional (Device → FHIR)
- **MedEdge:** Bi-directional (Device ↔ FHIR)
- Enables treatment parameter updates from cloud to device

### 4.2 FHIR R4 Subscriptions (2025 Feature)
- WebSocket-based real-time notifications
- Topic subscriptions for clinical alerts
- Eliminates polling overhead

### 4.3 Hybrid AI Engine
- Deterministic + Generative approaches
- Statistical detection for reliability
- LLM for contextual explanations

### 4.4 USCDI v3 Compliance
- Full US Core Data Interoperability support
- Comprehensive vital signs with LOINC codes
- Medication and procedure data extensibility

### 4.5 Industrial Resilience
- Store-and-forward buffering
- Circuit breaker patterns
- Exponential backoff retry logic
- Offline operation capability

## 5. Database Schema

### Patients Table
```
Id (PK)          | String
Mrn (unique)     | String
GivenName        | String
FamilyName       | String
BirthDate        | DateTime
Gender           | String (male, female, other, unknown)
CreatedAt        | DateTime
UpdatedAt        | DateTime
```

### Devices Table
```
Id (PK)                | String
DeviceId (unique)      | String
Manufacturer           | String
Model                  | String
SerialNumber (unique)  | String
Status                 | String (active, inactive, off)
AssignedPatientId (FK) | String (nullable)
CreatedAt              | DateTime
UpdatedAt              | DateTime
```

### Observations Table
```
Id (PK)         | String
PatientId (FK)  | String
DeviceId (FK)   | String
Code            | String (LOINC code)
CodeDisplay     | String
Value           | Double
Unit            | String
Status          | String (preliminary, final, amended, cancelled)
ObservationTime | DateTime
CreatedAt       | DateTime
UpdatedAt       | DateTime

Indexes:
- PatientId
- DeviceId
- Code
- ObservationTime
```

## 6. FHIR Resource Mapping

### Patient Resource
Maps to `FhirPatientEntity`
```json
{
  "resourceType": "Patient",
  "id": "P001",
  "identifier": [{"system": "http://example.org/mrn", "value": "P001"}],
  "name": [{"given": ["John"], "family": "Doe", "use": "official"}],
  "birthDate": "1965-03-15",
  "gender": "male"
}
```

### Device Resource
Maps to `FhirDeviceEntity`
```json
{
  "resourceType": "Device",
  "id": "Device-001",
  "identifier": [
    {"system": "http://bbraun.com/device-id", "value": "Device-001"},
    {"system": "http://bbraun.com/serial", "value": "DG001"}
  ],
  "manufacturer": "Medical Device",
  "modelNumber": "Dialog+",
  "status": "active"
}
```

### Observation Resource
Maps to `FhirObservationEntity`
```json
{
  "resourceType": "Observation",
  "id": "Obs-001",
  "status": "final",
  "code": {
    "coding": [{"system": "http://loinc.org", "code": "85354-9", "display": "Blood Pressure"}]
  },
  "subject": {"reference": "Patient/P001"},
  "device": {"reference": "Device/Device-001"},
  "effectiveDateTime": "2026-01-16T10:00:00Z",
  "value": {"value": 120, "unit": "mmHg"}
}
```

## 7. Clean Architecture Implementation

```
Domain Layer (Entities, Value Objects)
    ↑
Application Layer (Use Cases, DTOs, Services)
    ↑
Infrastructure Layer (Database, External APIs)
    ↑
Presentation Layer (Controllers, Views)
```

**Dependencies point inward only**
- Entities know nothing about outer layers
- Services depend on abstractions (interfaces)
- Each layer independent and testable

## 8. Security Architecture

### Transport Layer
- TLS 1.2+ for all external communications
- Certificate pinning for MQTT (future)

### Application Layer
- OAuth 2.0 for FHIR API authentication (Phase 4+)
- SMART on FHIR for patient apps
- Audit logging for all write operations

### Data Layer
- Input validation on all API endpoints
- SQL parameter binding (EF Core)
- No sensitive data in logs

### Infrastructure Layer
- Environment-based configuration
- Secrets manager integration (Azure Vault)
- Rate limiting on public endpoints

## 9. Scalability Considerations

### Horizontal Scaling
- Stateless API design (no session affinity)
- Distributed MQTT broker clusters
- Database connection pooling

### Vertical Scaling
- Async/await throughout (.NET)
- Non-blocking I/O operations
- Efficient database indexes

### Caching Strategy
- Redis for observation cache
- SignalR message buffering
- ETag support for REST responses

## 10. Monitoring & Observability

### Structured Logging
- Serilog with JSON output
- Correlation IDs across requests
- Performance metrics per endpoint

### Health Checks
- `/health` endpoint for orchestration
- Database connectivity checks
- External service availability

### Metrics to Track
- Request latency (p50, p95, p99)
- Observation creation rate
- Device connectivity status
- Queue depth (MQTT, buffer)

## 11. Deployment Architecture

### Development
- SQLite for local development
- In-memory databases for tests
- Docker Compose for local stack

### Production
- PostgreSQL for reliable RDBMS
- Redis for caching
- Kubernetes orchestration (future)
- Load balancer in front of APIs

## 12. Future Extensions

### Phase 5+
- FHIR Bulk Data API for research export
- Real-time subscriptions with WebSockets
- Multi-tenant support
- Advanced AI (risk prediction models)
- Mobile patient app with SMART on FHIR
- Integration with hospital EMR systems

---

## 13. Related Documentation

For deeper understanding of how the system works, refer to:

- **[TECHNICAL-GUIDE.md](../TECHNICAL-GUIDE.md)** - Comprehensive 100+ page guide explaining every component and flow
- **[ARCHITECTURE-DIAGRAMS.md](../ARCHITECTURE-DIAGRAMS.md)** - 8 detailed architecture diagrams from different perspectives
- **[README.md](../README.md)** - Project overview with "How It Works" end-to-end flow
- **[FHIR-MAPPING.md](FHIR-MAPPING.md)** - Detailed FHIR resource specifications and mappings
- **[DEPLOYMENT.md](../DEPLOYMENT.md)** - Complete deployment guide and troubleshooting

---

**Last Updated:** 2026-01-16
**Status:** Phase 5 Complete - All 5 phases implemented, production-ready
