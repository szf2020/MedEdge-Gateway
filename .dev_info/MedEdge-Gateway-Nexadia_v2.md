# MedEdge Gateway - NEXADIA Evolution Platform

> **A Next-Generation Medical Device Connectivity & Clinical Intelligence Platform**  
> Portfolio Project for B. Braun IIoT Architect / FHIR Backend Developer Roles

---

## 🎯 Executive Summary

**MedEdge Gateway** is a production-grade prototype demonstrating:
- **Industrial IoT Architecture** — Edge gateway bridging dialysis machines to cloud infrastructure
- **FHIR R4 Interoperability** — Standards-compliant healthcare data exchange
- **AI-Powered Clinical Intelligence** — Real-time anomaly detection and decision support
- **Bi-Directional Communication** — Device ↔ FHIR (beyond Sync7's uni-directional approach)

**Demo Command:** `docker-compose up` → Full platform running at `http://localhost:5000`

---

## 📐 System Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│  EDGE LAYER — "The Dialysis Floor"                                      │
│  ┌────────────────────┐     ┌────────────────────┐                     │
│  │  Dialog+ Simulator │     │  Dialog iQ Simulator│                    │
│  │  (Modbus TCP:502)  │     │  (Modbus TCP:503)   │                    │
│  └─────────┬──────────┘     └─────────┬──────────┘                     │
│            │                          │                                 │
│            └──────────┬───────────────┘                                 │
│                       ▼                                                 │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Edge Gateway (.NET 8 Worker Service)                            │   │
│  │  • Modbus Polling (NModbus) → Canonical JSON                     │   │
│  │  • MQTT Publishing (MQTTnet + TLS 1.2)                           │   │
│  │  • Store-and-Forward (SQLite buffer for offline resilience)      │   │
│  │  • Polly Retry/Circuit Breaker Patterns                          │   │
│  └──────────────────────────────┬──────────────────────────────────┘   │
└─────────────────────────────────┼───────────────────────────────────────┘
                                  │ MQTT over TLS
                                  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  MESSAGING LAYER                                                        │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Eclipse Mosquitto (Docker)                                      │   │
│  │  Topics: bbraun/dialysis/{deviceId}/telemetry                    │   │
│  │          bbraun/dialysis/{deviceId}/commands                     │   │
│  └──────────────────────────────┬──────────────────────────────────┘   │
└─────────────────────────────────┼───────────────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  CLOUD LAYER — "The Intelligence Core"                                  │
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Transform Service (.NET 8)                                      │   │
│  │  • MQTT Subscriber → FHIR R4 Mapper                              │   │
│  │  • LOINC/SNOMED Coding (85354-9 BP, 8867-4 HR, etc.)             │   │
│  │  • Publishes FHIR Observations to API                            │   │
│  └──────────────────────────────┬──────────────────────────────────┘   │
│                                 │                                       │
│  ┌──────────────────────────────▼──────────────────────────────────┐   │
│  │  AI Clinical Engine                                              │   │
│  │  • Layer 1: Statistical (Z-Score) — Immediate hard alarms        │   │
│  │  • Layer 2: Semantic Kernel — Contextual clinical explanations   │   │
│  │  • Outputs: RiskAssessment / Alert / Clinical Narrative          │   │
│  └──────────────────────────────┬──────────────────────────────────┘   │
│                                 │                                       │
│  ┌──────────────────────────────▼──────────────────────────────────┐   │
│  │  FHIR R4 API (ASP.NET Core Minimal APIs)                         │   │
│  │  • Resources: Patient, Device, Observation, DeviceMetric         │   │
│  │  • FHIR Subscriptions (WebSocket real-time notifications)        │   │
│  │  • SMART on FHIR Authorization (OAuth 2.0)                       │   │
│  │  • Bulk Data Export (/$export endpoint)                          │   │
│  │  • Storage: PostgreSQL (prod) / SQLite (demo)                    │   │
│  └──────────────────────────────┬──────────────────────────────────┘   │
└─────────────────────────────────┼───────────────────────────────────────┘
                                  │ SignalR WebSocket
                                  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER — "The Clinical Dashboard"                          │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │  Blazor WebAssembly Dashboard                                    │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │   │
│  │  │ Fleet View  │  │ Live Vitals │  │ FHIR        │              │   │
│  │  │ (3 Devices) │  │ (Real-time) │  │ Inspector   │              │   │
│  │  │ ●●○ Status  │  │ Charts.js   │  │ JSON View   │              │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘              │   │
│  │  [ 🔴 Trigger Hypotension ]  [ ⛔ Emergency Stop ]               │   │
│  └─────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 🔥 Key Innovations (Beyond Current NEXADIA Suite)

| Feature | Current NEXADIA Sync7 | MedEdge Gateway |
|---------|----------------------|-----------------|
| Data Flow | Uni-directional (Device → FHIR) | **Bi-directional** (Device ↔ FHIR) |
| Notifications | Polling-based | **FHIR R4 Subscriptions** (WebSocket push) |
| Intelligence | Static rules | **Hybrid AI** (Statistical + LLM) |
| Compliance | Basic FHIR | **USCDI v3** + US Core |
| Research | N/A | **Bulk Data API** (NDJSON export) |

---

## 📁 Solution Structure

```
MedEdge/
├── src/
│   ├── Edge/
│   │   ├── MedEdge.DeviceSimulator/     # Dialog+/iQ Modbus TCP simulators
│   │   └── MedEdge.EdgeGateway/         # .NET 8 Worker: Modbus → MQTT
│   ├── Cloud/
│   │   ├── MedEdge.FhirApi/             # ASP.NET Core FHIR R4 server
│   │   ├── MedEdge.TransformService/    # MQTT → FHIR transformer
│   │   └── MedEdge.AiEngine/            # Semantic Kernel clinical AI
│   ├── Web/
│   │   └── MedEdge.Dashboard/           # Blazor WASM clinical dashboard
│   └── Shared/
│       └── MedEdge.Core/                # Domain models, DTOs, interfaces
├── tests/
│   ├── MedEdge.FhirApi.Tests/
│   └── MedEdge.Integration.Tests/
├── docs/
│   ├── ARCHITECTURE.md                  # Detailed system design
│   ├── DEMO-SCRIPT.md                   # 5-minute demo walkthrough
│   └── FHIR-MAPPING.md                  # Telemetry → FHIR mapping guide
├── docker-compose.yml                   # One-command full stack
├── docker-compose.override.yml          # Development overrides
└── README.md                            # Project overview + quick start
```

---

## 🚀 Implementation Roadmap

### Phase 1: FHIR Backbone ⭐ *Start Here*
> **Goal:** Working FHIR API with proper resource modeling

**Deliverables:**
1. Solution scaffold with Clean Architecture layers
2. EF Core + SQLite database with seed data
3. FHIR R4 REST API endpoints:
   - `POST /fhir/Observation` — Create observation
   - `GET /fhir/Observation?patient={id}` — Query by patient
   - `GET /fhir/Device` — List dialysis machines
   - `GET /fhir/Patient` — List patients
4. Swagger UI at `/swagger`

**Verification:** `curl -X GET http://localhost:5000/fhir/Patient` returns FHIR Bundle

---

### Phase 2: Industrial Edge Pipeline
> **Goal:** Real-time telemetry flowing from simulated hardware

**Deliverables:**
1. **Device Simulator** (Modbus TCP server)
   - Simulates Dialog+ and Dialog iQ machines
   - Generates realistic dialysis telemetry:
     - Blood Flow Rate: 200-400 mL/min
     - Venous/Arterial Pressure: 50-200 mmHg
     - Dialysate Temperature: 35-38°C
     - Conductivity: 13.5-14.5 mS/cm
   - Includes "Chaos Mode" for anomaly injection

2. **Edge Gateway** (Worker Service)
   - Polls Modbus registers every 500ms
   - Transforms to canonical JSON
   - Publishes to MQTT with TLS
   - SQLite buffer for offline resilience

3. **Infrastructure**
   - Dockerized Mosquitto MQTT broker
   - Container networking

**Verification:** MQTT Explorer shows JSON payloads on `bbraun/dialysis/+/telemetry`

---

### Phase 3: Clinical Intelligence Layer
> **Goal:** AI-powered anomaly detection with bi-directional control

**Deliverables:**
1. **Transform Service**
   - MQTT Subscriber → FHIR Observation mapper
   - LOINC coding for all vital signs

2. **AI Anomaly Detector** (Hybrid Architecture)
   ```csharp
   public interface IAnomalyDetector
   {
       AnomalyResult Analyze(TelemetryWindow window);
   }
   
   // Implementation 1: Statistical (Z-Score) - Demo reliable
   // Implementation 2: Semantic Kernel - LLM explanations
   ```

3. **Bi-Directional Control**
   - FHIR `DeviceRequest` → MQTT command → Modbus write
   - "Emergency Stop" demonstrates write-back capability

**Verification:** Console logs "CRITICAL: Hypotension detected on Device-001"

---

### Phase 4: Clinical Dashboard
> **Goal:** Professional-grade real-time visualization

**Deliverables:**
1. **Blazor WebAssembly UI**
   - B. Braun color palette (White/Green/Grey)
   - MudBlazor component library

2. **Three-Panel Layout:**
   - **Fleet View**: Device status cards (🟢🟡🔴)
   - **Live Vitals**: Chart.js real-time waveforms
   - **FHIR Inspector**: Syntax-highlighted JSON

3. **Scenario Controls:**
   - `[Trigger Hypotension]` — Inject clinical event
   - `[Emergency Stop]` — Demonstrate bi-directional

4. **SignalR Integration** — Live push updates

**Verification:** Charts update immediately on page load; alerts appear within 2 seconds

---

### Phase 5: Demo & Polish
> **Goal:** Interview-ready demonstration

**Deliverables:**
1. **Automated Demo Scenario** (5 minutes)
   - 0:00-0:30 — System startup, green status
   - 0:30-2:00 — Normal operation, data flowing
   - 2:00-3:00 — Hypotension event triggered
   - 3:00-4:00 — AI alert, intervention simulated
   - 4:00-5:00 — Recovery, FHIR report generated

2. **Documentation**
   - `README.md` with Mermaid architecture diagram
   - `DEMO-SCRIPT.md` with exact steps

---

## 🛠 Technical Specifications

### Technology Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| **Edge** | .NET 8, NModbus, MQTTnet, Polly | Device connectivity |
| **Messaging** | Eclipse Mosquitto | MQTT broker |
| **API** | ASP.NET Core 8, Firely SDK, EF Core | FHIR server |
| **AI** | Semantic Kernel, Azure OpenAI (optional) | Clinical intelligence |
| **Database** | PostgreSQL / SQLite | FHIR storage |
| **Real-time** | SignalR | WebSocket push |
| **Frontend** | Blazor WASM, Chart.js, MudBlazor | Dashboard |
| **Containers** | Docker, docker-compose | Orchestration |

### Code Quality Standards

- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → Presentation)
- **Patterns:** Repository, CQRS, Dependency Injection
- **Logging:** Serilog with structured JSON output
- **Testing:** xUnit for unit tests, integration tests for FHIR endpoints
- **Style:** File-scoped namespaces, Records for DTOs, `.editorconfig` enforced

### Security

- TLS 1.2+ for all communications
- OAuth 2.0 / SMART on FHIR authorization
- Audit logging for all FHIR operations
- TPM-based device attestation (simulated)

---

## ✅ Success Criteria

| Criteria | Description |
|----------|-------------|
| **Zero-Config Start** | `docker-compose up` brings up all 5 containers |
| **Immediate Visuals** | Dashboard shows moving charts on first load |
| **AI Detection** | "Trigger Hypotension" causes RED alert within 2 seconds |
| **FHIR Compliance** | All resources validate against R4 specification |
| **Bi-Directional** | "Emergency Stop" halts data stream from simulator |
| **Offline Resilience** | Gateway buffers data when MQTT broker is down |

---

## 💬 Interview Talking Points

### For FHIR Backend Developer Role:
- "I implemented FHIR R4 Subscriptions — the cutting-edge feature for 2025"
- "The system supports bi-directional FHIR, extending beyond Sync7's capabilities"
- "I used proper LOINC coding: 85354-9 for Blood Pressure, 8867-4 for Heart Rate"
- "The Bulk Data API enables research export in NDJSON format"

### For IIoT Architect Role:
- "Here's the full data pipeline: Modbus → MQTT → FHIR → Dashboard"
- "The edge gateway runs containerized on ARM64 for Raspberry Pi deployment"
- "Store-and-forward buffering ensures zero data loss during network outages"
- "Polly circuit breakers enable graceful degradation"

### Domain Knowledge:
- Understanding of NEXADIA expert, monitor, mobile companion ecosystem
- Knowledge of Dialog+ and Dialog iQ machine specifications
- USCDI v3 compliance for regulatory alignment
- Modern FHIR features: Subscriptions, Bulk Data, SMART on FHIR

---

## 🎬 Quick Start

```bash
# Clone and run
git clone https://github.com/your-org/MedEdge.git
cd MedEdge
docker-compose up -d

# Open dashboard
start http://localhost:5000

# Open FHIR API docs
start http://localhost:5001/swagger
```

---

## 📋 Agent Instructions

When implementing this project, follow these principles:

1. **Start with Phase 1** — The FHIR API is the foundation
2. **Self-Contained** — Must run without external cloud dependencies by default
3. **Mock AI Mode** — If no API key, use statistical detection (100% reliable for demo)
4. **Professional Aesthetics** — Use B. Braun branding, medical-grade UI feel
5. **Documentation First** — Update README and docs with each phase

**Immediate Next Step:** Execute Phase 1 — Scaffold solution and implement FHIR API

---

*Last Updated: 2026-01-16*
