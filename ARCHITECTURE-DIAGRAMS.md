# MedEdge Gateway - Architecture Diagrams & Explanations

**Purpose:** Visual representations of system architecture and data flows
**Last Updated:** 2026-01-16

---

## 1. System Layer Architecture

```
┌────────────────────────────────────────────────────────────────────┐
│                        PRESENTATION TIER                           │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐  │
│  │  Blazor WebAssembly Dashboard (Browser)                      │  │
│  │                                                               │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌────────────────────┐ │  │
│  │  │ Fleet View   │  │ Live Vitals  │  │ FHIR Inspector    │ │  │
│  │  │ - Devices    │  │ - 6 Charts   │  │ - Patient Query   │ │  │
│  │  │ - Status     │  │ - Real-time  │  │ - Device Query    │ │  │
│  │  │ - Alerts     │  │ - Thresholds │  │ - Export JSON     │ │  │
│  │  └──────────────┘  └──────────────┘  └────────────────────┘ │  │
│  │                           ↕                                    │  │
│  │           SignalR WebSocket (HTTP/1.1 Upgrade)               │  │
│  └──────────────────────────────────────────────────────────────┘  │
└────────────────────────────┬─────────────────────────────────────────┘
                             │
                    HTTP/REST & WebSocket
                             │
┌────────────────────────────▼─────────────────────────────────────────┐
│                         APPLICATION TIER                             │
│                                                                       │
│  ┌─────────────────────┐  ┌────────────────────────────────────────┐│
│  │  FHIR API Server    │  │  SignalR Hub                           ││
│  │  (ASP.NET Core)     │  │  (TelemetryHub)                        ││
│  │                     │  │                                         ││
│  │  Endpoints:         │  │  Group-based routing:                  ││
│  │  - GET /fhir/*      │  │  - Subscribe(deviceId)                 ││
│  │  - POST /fhir/*     │  │  - BroadcastVitalUpdate                ││
│  │  - /api/devices     │  │  - BroadcastAlerts                     ││
│  │  - /health          │  │  - GetSubscriberCount                  ││
│  │                     │  │                                         ││
│  └─────────────────────┘  └────────────────────────────────────────┘│
│                   ↕                     ↕                            │
│          ┌────────────────────────────────────────┐                │
│          │  EF Core + SQLite Database            │                │
│          │  - Patients, Devices, Observations    │                │
│          │  - Relationships & constraints        │                │
│          └────────────────────────────────────────┘                │
└────────────────────────────┬─────────────────────────────────────────┘
                             │
                      REST API, MQTT
                             │
┌────────────────────────────▼─────────────────────────────────────────┐
│                         SERVICE TIER                                 │
│                                                                       │
│  ┌──────────────────────┐      ┌──────────────────────┐             │
│  │ Transform Service    │      │ AI Engine            │             │
│  │                      │      │                      │             │
│  │ MQTT Subscriber:     │      │ Anomaly Detector:    │             │
│  │ - Listens to MQTT    │      │ - Statistical model  │             │
│  │                      │      │ - 8 clinical checks  │             │
│  │ FHIR Mapper:         │      │ - Risk stratification│             │
│  │ - LOINC codes        │      │ - Clinical recs      │             │
│  │ - Unit conversions   │      │                      │             │
│  │                      │      │ Output:              │             │
│  │ Output:              │      │ - Severity levels    │             │
│  │ - FHIR Observations  │      │ - Findings           │             │
│  │ - Bundles            │      │ - Recommendations    │             │
│  └──────────────────────┘      └──────────────────────┘             │
│                                       ↕                              │
│              ┌─────────────────────────────────────────┐            │
│              │  MQTT Broker (Eclipse Mosquitto)       │            │
│              │  - Topic: bbraun/dialysis/+/telemetry  │            │
│              │  - TLS 1.2 encryption                  │            │
│              │  - Pub/Sub distribution                │            │
│              └─────────────────────────────────────────┘            │
└────────────────────────────┬─────────────────────────────────────────┘
                             │
                        MQTT over TLS
                             │
┌────────────────────────────▼─────────────────────────────────────────┐
│                         EDGE TIER                                    │
│                                                                       │
│  ┌────────────────────────┐        ┌──────────────────────┐         │
│  │ Edge Gateway           │        │ Device Simulator     │         │
│  │                        │        │                      │         │
│  │ Modbus Client:         │◄───────│ Modbus TCP Server:   │         │
│  │ - Polls every 500ms    │        │ - 3 servers (502-504)│         │
│  │ - Reads registers      │        │ - Realistic telemetry│         │
│  │ - Unit conversion      │        │ - Chaos mode support │         │
│  │                        │        │                      │         │
│  │ Offline Buffering:     │        │ Measurements:        │         │
│  │ - Local SQLite DB      │        │ - Blood flow, temp   │         │
│  │ - Max 10K messages     │        │ - Pressures, conduct │         │
│  │ - Auto-replay          │        │ - Updates: 500ms     │         │
│  │                        │        │                      │         │
│  │ Polly Resilience:      │        └──────────────────────┘         │
│  │ - Retry: 3x + backoff  │                                         │
│  │ - Circuit breaker      │         (In production: Real devices)  │
│  │ - Timeout handling     │                                         │
│  └────────────────────────┘                                         │
│                                                                       │
│              (Docker Container: medEdge-gateway)                    │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Data Flow Diagram

### Real-Time Telemetry Path

```
DEVICE                        EDGE                    CLOUD
════════════════════════════════════════════════════════════════

Medical Device                 Edge Gateway             MQTT Broker
    ↓                              ↓                      ↓
 [Modbus]                    [Modbus Client]          [Message Hub]
    │                              │                      │
    ├─ Register 40001             │                       ├─→ Transform Service
    │  (Blood Flow)               │                       │   └─→ FHIR Conversion
    │                             │                       │       └─→ FHIR API
    ├─ Register 40003             │                       │
    │  (Art. Pressure)            │                       ├─→ AI Engine
    │                      ┌──────▼───────┐              │   └─→ Anomaly Detection
    ├─ Register 40005      │ Convert to   │              │       └─→ Alert
    │  (Ven. Pressure)     │ Engineering  │              │
    │                      │ Units        │              └─→ SignalR Hub
    ├─ Register 40007      │              │                  └─→ Dashboard
    │  (Temperature)       └──────┬───────┘
    │                             │
    └─ Register 40009        [JSON Payload]
       (Conductivity)             │
                            ┌─────▼──────┐
         ┌─────────────────►│   MQTT     │
         │                 │ Publish    │
         │                 └─────┬──────┘
         │                       │
         │                  [MQTT Topic]
         │    bbraun/dialysis/Device-001/telemetry
         │                       │
         │                   [Parallel]
         │                   ┌───┴────┬────────┬─────────┐
         │                   │        │        │         │
         │              [Path 1]  [Path 2] [Path 3]  [Path 4]
         │
         └─ Retry if fails
            with backoff
```

### Alert Propagation Path

```
ANOMALY DETECTED                ALERT GENERATED               CLINICIAN SEES
════════════════════════════════════════════════════════════════════════════

Blood Flow = 145 mL/min        AI Engine:                    Dashboard:
(Below critical 150)            Check threshold               RED ALERT
       ↓                        145 < 150? YES                    ↓
                                    ↓                       ┌──────────────┐
MQTT Message                   Create Alert:                │  Finding:    │
{bloodFlow: 145}               {severity: Critical,         │  Hypotension │
       ↓                        finding: "..."}             │  detected    │
                                    ↓                       │              │
AI Engine                      SignalR Broadcast            │ Recommend:  │
Analyzes                       to Group("Device-001")        │ Check needle │
       ↓                             ↓                       └──────────────┘
Compares to                    WebSocket Message                   ↓
Thresholds                     "AlertsReceived"            Clinician Action
       ↓                             ↓                      (view/stop/call)
CRITICAL                       Browser receives
Hypotension                     JSON alert
       ↓                             ↓
Risk Level:                    React component
CRITICAL                       renders alert


Total Latency: ~90ms
```

---

## 3. Component Interaction Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPLOYMENT VIEW                              │
└─────────────────────────────────────────────────────────────────┘

Docker Network: medEdge-network

┌──────────────────┐      ┌──────────────────┐    ┌─────────────┐
│  Simulator       │      │  Gateway         │    │  MQTT       │
│  Container       │◄────►│  Container       │───►│  Container  │
│                  │      │                  │    │             │
│  Port 502-504    │      │  Modbus Client   │    │  Port 1883  │
│  (Modbus TCP)    │      │  MQTT Publisher  │    │             │
└──────────────────┘      └──────────────────┘    └──────┬──────┘
                                                         │
                                    ┌────────────────────┼─────────────────┐
                                    │                    │                 │
                         ┌──────────▼─────────┐ ┌──────▼──────┐ ┌────────▼──────┐
                         │ Transform Service  │ │ AI Engine   │ │ Streaming     │
                         │ Container          │ │ Container   │ │ (future)      │
                         │                    │ │             │ │               │
                         │ MQTT Subscriber    │ │ Real-time   │ │               │
                         │ FHIR Mapper        │ │ Analysis    │ │               │
                         │ FHIR API Client    │ │             │ │               │
                         └──────┬─────────────┘ └─────────────┘ └───────────────┘
                                │
                                │ REST POST
                                │ /fhir/Observation
                                │
                         ┌──────▼───────────┐
                         │ FHIR API         │
                         │ Container        │
                         │                  │
                         │ Port 5001        │
                         │ REST endpoints   │
                         │ SignalR Hub      │
                         └──────┬───────────┘
                                │
                         ┌──────▼───────────┐
                         │ Database Volume  │
                         │                  │
                         │ medEdge.db       │
                         │ (SQLite)         │
                         └──────────────────┘


                         ┌──────────────────┐
                         │ Dashboard        │
                         │ Container        │
                         │                  │
                         │ Port 5000        │
                         │ Nginx + WASM     │
                         └──────┬───────────┘
                                │
                                │ HTTP/WebSocket
                                │
                    User Browser (localhost:5000)
```

---

## 4. Request Flow: Creating an Observation

```
Dashboard User          API Server           Database          MQTT
═══════════════════════════════════════════════════════════════════

   │                        │                  │               │
   │ Click: View Device     │                  │               │
   ├─────────────────────►  │                  │               │
   │                        │                  │               │
   │ Browser sends:         │                  │               │
   │ GET /fhir/Device/001   │                  │               │
   ├─────────────────────►  │                  │               │
   │                        │                  │               │
   │                        │ Query Devices    │               │
   │                        ├─────────────────►│               │
   │                        │                  │               │
   │                        │ Return Data      │               │
   │                        │◄─────────────────┤               │
   │                        │                  │               │
   │ JSON Response          │                  │               │
   │◄─────────────────────┤                  │               │
   │                        │                  │               │
   │ Render Device Card     │                  │               │
   │◄─────────────────────┤                  │               │
   │                        │                  │               │
   │ SignalR Connected      │                  │               │
   ├─────────────────────► │                  │               │
   │ Subscribe("Device-01") │                  │               │
   │                        │ Add to Group     │               │
   │                        │ ("Device-001")   │               │
   │                        │                  │               │
   │ (Now receiving updates from Transform Service via MQTT)  │
   │                        │                  │               │
   ├─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─► Transform Service ─────────────►│
   │                        │    listening     │  MQTT Sub:    │
   │ Real-time Vital Signs  │  for Device-001  │ Blood Flow    │
   │◄─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┤   telemetry      │  Pressure     │
   │                        │◄────────────────┐│  Temperature  │
   │ (via SignalR)          │ Convert to FHIR │               │
   │                        │ POST to API:    │               │
   │                        ├────────────────►│               │
   │                        │ Store as FHIR   │               │
   │                        │ Observation     │               │
   │◄─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┤◄─────────────────┤               │
   │ [Blood Flow: 320]      │ SignalR Broadcast
   │ [Pressure: 120]        │ ("VitalSignUpdate")
   │ [Temp: 36.5]           │
```

---

## 5. Real-Time Update Architecture

```
Signal Flow for Real-Time Dashboard Updates:

MQTT Message Published
(bbraun/dialysis/Device-001/telemetry)
         ↓
    [Broker Routes]
         ↓
   ┌─────┴──────┬──────────┐
   ↓            ↓          ↓
Transform    AI Engine   SignalR
Service      Analyzes    Hub
             Thresholds  │
   │            │        │
   │ Creates    │Detects │
   │FHIR Obs   │Anomaly │
   │            │        │
   │            ├────────┤
   │            ↓        │
   │         Broadcasts  │
   │         Alert to    │
   │         Group       │
   │            │        │
   └─────┬──────┴────────┤
         ↓               │
    [SignalR Hub]        │
         ├───────────────┘
         │
    [Connected Clients Group: Device-001]
         ├─ Client 1 (Dashboard A)
         ├─ Client 2 (Dashboard B)
         └─ Client 3 (Dashboard C)

    Each receives:
    ├─ VitalSignUpdate (if subscribed)
    └─ AlertsReceived (if anomaly detected)

    Browser receives WebSocket message
         ↓
    JavaScript callback triggered
         ↓
    Blazor component updated
         ↓
    React/update UI
         ↓
    User sees change immediately
    (no page refresh needed)
```

---

## 6. Database Relationship Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                    DATABASE SCHEMA                           │
└──────────────────────────────────────────────────────────────┘

Patient (Demographics)
┌────────────────┐
│ Id (PK)        │
│ Mrn (UK)       │◄─────────┐
│ GivenName      │          │
│ FamilyName     │          │ 1:N
│ BirthDate      │          │
│ Gender         │          │
│ CreatedAt      │          │
└────────────────┘          │
         ▲                   │
         │ 1:1              │
         │                  │
                            │
Device (Equipment)     Observation (Vitals)
┌────────────────┐          ┌──────────────────┐
│ Id (PK)        │◄────────►│ Id (PK)          │
│ DeviceId (UK)  │ 1:N      │ PatientId (FK)   │◄─┐
│ Manufacturer   │          │ DeviceId (FK)    │  │
│ Model          │          │ Code             │  │1:N
│ SerialNumber   │          │ CodeDisplay      │  │
│ Status         │          │ Value            │  │
│ CreatedAt      │          │ Unit             │  │
└────────────────┘          │ Status           │  │
                            │ ObservationTime  │  │
                            │ CreatedAt        │  │
                            └──────────────────┘  │
                                    ▲             │
                                    │ N:1         │
                                    │             │
                         Historical Data Storage─┘

Indices for Performance:
├─ Patient.Mrn (search by MRN)
├─ Device.DeviceId (search by device)
├─ Observation.PatientId (patient queries)
├─ Observation.DeviceId (device queries)
└─ Observation.ObservationTime (time-range queries)
```

---

## 7. Resilience & Error Handling

```
Edge Gateway Resilience:

Normal Path:
Device ──Modbus──► Gateway ──MQTT──► Broker ──ok
                      ✓                   ✓

Device Offline:
Device ─X─ (no data) ──BUFFER──► LocalDB ──retry──► Broker
                      Stores up to 10K messages
                      Replay when connection restored

MQTT Broker Down:
Device ──Modbus──► Gateway ──X─ (can't connect)
                      ✓
                   [Offline Buffer]
                   Stores locally
                      │
                   [Wait/Retry]
                   Exponential backoff
                      │
                   Broker comes back up
                      │
                   [Auto-Replay]
                   All buffered messages sent
                      │
                   Normal operation resumes


Polly Circuit Breaker:

Normal (Closed)
All requests pass through ──► Success

After 5 failures:
Circuit opens ──X──► Fast-fail (no wait)
                     Return error immediately

Wait 30 seconds:

Half-Open (test mode)
Allow 1 request ──► Success? → Closed (normal)
                → Failure? → Open (retry)
```

---

## 8. Technology Integration Points

```
MedEdge Ecosystem Integration:

┌─────────────────────────────────────────────────────────────┐
│  MedEdge Gateway (FHIR R4 Interoperability Hub)            │
└─────────────────────────────────────────────────────────────┘

Integrations:

Left Side (Data In):              Right Side (Data Out):
├─ Medical Device Integration      ├─ EHR Systems
│  └─ Dialysis Machines            │  └─ Patient Records
│                                  │
├─ Other Dialysis Equipment        ├─ Clinical Analytics
│  └─ via Modbus/HL7v2             │  └─ Dashboards
│                                  │
├─ ICU Equipment                   ├─ Research Platforms
│  └─ Vitals, Ventilators          │  └─ De-identified Data
│                                  │
└─ Lab Systems                     └─ Compliance Systems
   └─ Results                         └─ Audit Trails


Data Format Conversions:

Device Protocol (Modbus) ──────┐
                                 ├──► FHIR R4 (Standard)
MQTT (Device Format) ────────┐  │
                              ├──► HL7 v2 (Legacy)
Transform Service ────────────┘  │
                                 └──► Custom APIs
```

---

## Summary

These diagrams show:

1. **System Layers**: How components stack from device to dashboard
2. **Data Flow**: How information moves through the system
3. **Components**: What each service does and how they interact
4. **Timing**: Latencies and how fast alerts propagate
5. **Database**: How data is organized and related
6. **Resilience**: How system handles failures gracefully
7. **Integration**: How MedEdge fits into healthcare ecosystem

For detailed explanations, see [TECHNICAL-GUIDE.md](TECHNICAL-GUIDE.md)

