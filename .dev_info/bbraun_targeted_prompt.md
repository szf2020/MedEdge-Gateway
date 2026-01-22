# Agent Task: Build "Nexadia Next" - Intelligent Renal Care Platform

**Project Goal:** Create a flagship portfolio project for a B. Braun interview. This system, "Nexadia Next," is a conceptual next-generation prototype of B. Braun's **NEXADIA® Sync** system. It demonstrates how industrial device connectivity (Modbus), interoperability (FHIR), and AI (Clinical Decision Support) can converge.

**Context:** The user needs a production-grade, self-contained demo that runs locally via Docker. It must look and feel like a professional medical software product.

---

## 1. Architectural Blueprint (The "Nexadia" Ecosystem)

**Root Solution:** `NexadiaNext.sln`

### A. The "Dialysis Floor" (Edge Simulation)
*   **Context:** Simulates the physical clinic environment.
*   **Project:** `src/Edge/DialogSimulator` (.NET 8 Console).
    *   **Role:** Simulates 3x **Dialog iQ** (B. Braun's latest) and **Dialog+** machines.
    *   **Protocol:** Modbus TCP (Industrial Standard).
    *   **Data Points:**
        *   *Venous Pressure* (mmHg)
        *   *Arterial Pressure* (mmHg)
        *   *Blood Flow Rate* (mL/min)
        *   *Dialysate Temperature* (°C)
        *   *Conductivity* (mS/cm)
    *   **Behavior:** Generates realistic "session" data. Includes a **"Trigger Hypotension"** command to simulate a patient blood pressure crash for the AI to catch.

### B. The Connectivity Engine (Gateway)
*   **Context:** The "Sync" component.
*   **Project:** `src/Edge/NexadiaGateway` (.NET 8 Worker).
    *   **Role:** Connects to the "machines" (simulator) via Modbus.
    *   **Function:**
        1.  Polls Modbus registers every 500ms.
        2.  Enriches data with Device Metadata (Serial #, Model).
        3.  Publishes to internal MQTT Broker (topic: `bbraun/dialysis/{deviceId}/telemetry`).
    *   **Resilience:** Uses `Polly` for "Always-On" connectivity.

### C. The Intelligence Core (Cloud/Server)
*   **Context:** The central processing unit.
*   **Project:** `src/Core/NexadiaHub` (ASP.NET Core Web API + Background Services).
    *   **Service 1: Ingestion & Transformation:**
        *   Subscribes to MQTT.
        *   **FHIR Mapping:** Converts telemetry to **HL7 FHIR R4** resources:
            *   `Device` (The Dialog Machine)
            *   `Patient` (The fictitious patient)
            *   `Observation` (The vital signs, properly coded with LOINC where applicable).
    *   **Service 2: AI Anomaly Detector (The "Innovation"):**
        *   **Feature:** "Intelligent Hypotension Guard".
        *   **Logic:** Analyzes a sliding window of Venous/Arterial pressure.
        *   **Implementation:** Hybrid Semantic Kernel.
            *   *Mode A (Demo/Offline):* Uses statistical Z-Score to detect sudden drops (Reliable for presentation).
            *   *Mode B (Cloud):* Placeholder for LLM-based analysis of trends.
        *   **Output:** Generates a `Flag` object or FHIR `RiskAssessment`.

### D. The Clinical Dashboard (UI)
*   **Project:** `src/Web/NexadiaMonitor` (Blazor WebAssembly).
*   **Design:** Professional, clean, "Medical Grade" UI (White/Blue/Grey palette).
*   **Features:**
    *   **"Ward View":** Grid showing status of all 3 machines (Green = Dialysis in progress, Red = Alarm).
    *   **"Session Detail":** Click a machine to see real-time Chart.js waveforms of Blood Flow & Pressure.
    *   **"FHIR Inspector":** A developer tab showing the raw JSON being generated (Prove interoperability!).
    *   **"Emergency Test":** A big button to inject the "Hypotension Event" into the simulator and watch the AI catch it.

---

## 2. Implementation Roadmap

### Phase 1: Foundation & FHIR (The "Sync" Layer)
*   **Goal:** Establish the FHIR server and database.
*   **Tasks:**
    *   Init solution.
    *   Create `NexadiaHub` (Web API).
    *   Install `Firely.Fhir.R4` SDK.
    *   Setup SQLite with EF Core.
    *   **Deliverable:** Swagger UI where I can `GET /fhir/Device` and see the "Dialog iQ" machines.

### Phase 2: The Industrial Edge
*   **Goal:** Get data flowing.
*   **Tasks:**
    *   Create `DialogSimulator` (NModbus).
    *   Create `NexadiaGateway`.
    *   Dockerize Mosquitto (MQTT).
    *   **Deliverable:** Running the simulator logs showing "Publishing telemetry..." and Gateway receiving it.

### Phase 3: The Intelligence (AI)
*   **Goal:** Add the "Wow" factor.
*   **Tasks:**
    *   Implement `AnomalyDetectionService` in the Hub.
    *   Connect SignalR for real-time alerts.
    *   **Deliverable:** Console log showing "CRITICAL ALERT: Hypotension detected on Device 001".

### Phase 4: The Glass (UI)
*   **Goal:** Make it see-able.
*   **Tasks:**
    *   Blazor WASM setup with MudBlazor or standard Bootstrap.
    *   Real-time charts.
    *   **Deliverable:** A beautiful dashboard running on localhost.

---

## 3. Technical Constraints & Standards
*   **Strict Standard:** Use HL7 FHIR R4 standard structures.
*   **Naming:** Use B. Braun terminology (`Dialog+`, `Dialog iQ`, `Nexadia`).
*   **Zero-Config:** `docker-compose up` runs the *entire* cluster (Sim, Broker, API, UI).
*   **Documentation:** `README.md` must explain the "Data Journey" from Modbus Register -> FHIR Observation.

**Immediate Action:** Begin **Phase 1**. Scaffold the Solution and the FHIR API.
