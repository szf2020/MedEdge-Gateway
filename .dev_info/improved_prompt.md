# Agent Task: Build "MedConnect Hub" - Intelligent IIoT & FHIR Platform

**Project Goal:** Create a production-grade prototype of a medical device connectivity platform ("MedConnect Hub") that bridges the gap between industrial IoT (dialysis machines) and healthcare interoperability standards (FHIR), enhanced with AI-driven insights.

**Context:** This is a portfolio project to demonstrate architectural competence for B. Braun. It must be self-contained, easy to run, and visually impressive.

---

## 1. Architectural Blueprint & Tech Stack

**Root Solution:** `MedConnectHub.sln`

### A. Edge Layer (Simulation & Ingestion)
*   **Context:** Runs on "Edge" (simulated via Docker).
*   **Projects:**
    *   `src/Edge/DeviceSimulator`: .NET 8 Console App.
        *   **Role:** Simulates 3 physical "Dialog+" dialysis machines.
        *   **Protocol:** Modbus TCP (Server).
        *   **Behavior:** Generates realistic waveforms (sine/noise) for BP, Flow, Temp. Includes a "Demo Mode" trigger to inject anomalies (e.g., hypotension event).
    *   `src/Edge/EdgeGateway`: .NET 8 Worker Service.
        *   **Role:** Polls Modbus, converts to internal canonical format, pushes to MQTT.
        *   **Libs:** `NModbus` (Modbus client), `MQTTnet` (MQTT client).
        *   **Resilience:** Polly policies for retry/circuit breaking on connection loss.

### B. Messaging Infrastructure
*   **Broker:** Eclipse Mosquitto (running in Docker).
*   **Topic Structure:** `devices/{deviceId}/telemetry`, `devices/{deviceId}/alarms`.

### C. Cloud/Core Layer (Processing & API)
*   **Context:** Central processing logic.
*   **Projects:**
    *   `src/Core/MedConnect.TransformService`: .NET 8 Worker Service.
        *   **Role:** Subscribes to MQTT. Uses **Semantic Kernel** (or a mock strategy if no API key) to analyze data windows for anomalies. Maps data to FHIR `Observation` resources.
        *   **AI Logic:** If `UseRealAI=false`, use statistical heuristics (Standard Deviation). If `true`, use LLM to describe the anomaly in natural language.
    *   `src/Core/MedConnect.FhirApi`: ASP.NET Core Web API (Minimal APIs).
        *   **Role:** FHIR Facade.
        *   **Standard:** HL7 FHIR R4 (using `Firely SDK` / `Hl7.Fhir.R4`).
        *   **Storage:** SQLite (for easy demo portability) with EF Core.
        *   **Endpoints:**
            *   `POST /fhir/Observation` (Ingest)
            *   `GET /fhir/Observation` (Query by patient/date)
            *   `GET /fhir/Device`
            *   `GET /fhir/Patient`

### D. Presentation Layer (UI)
*   **Project:** `src/Web/MedConnect.Dashboard`: Blazor WebAssembly.
*   **Features:**
    *   **Live Monitor:** SignalR client receiving real-time pushes from `TransformService`.
    *   **Charts:** `Chart.js` or `MudBlazor` charts for visualizing telemetry.
    *   **Twin View:** 3 Cards representing the machines. Color-coded status (Green/Yellow/Red).
    *   **FHIR Inspector:** JSON syntax highlighted view of the generated FHIR resources.

---

## 2. Implementation Roadmap

### Phase 1: The FHIR Backbone (Core)
1.  Initialize `MedConnectHub.sln` with the 4-layer structure.
2.  Implement `MedConnect.FhirApi`.
    *   Define EF Core entities for `Patient`, `Device`, `Observation`.
    *   Implement Seed Data (3 Patients, 3 Devices).
    *   Enable Swagger/OpenAPI.
3.  **Verification:** `curl` to GET a patient and POST an observation.

### Phase 2: The Industrial Edge (Simulation)
1.  Implement `DeviceSimulator` with `NModbus`.
    *   Create a `MachineProfile` class to randomize data slightly so devices look distinct.
2.  Implement `EdgeGateway`.
    *   Connect to Simulator via Modbus TCP.
    *   Serialize to JSON.
    *   Publish to local Mosquitto MQTT.
3.  **Verification:** Use MQTT Explorer (or logs) to see JSON payloads flowing.

### Phase 3: Intelligence & Integration (The "Hub")
1.  Implement `TransformService`.
    *   Bridge MQTT -> FHIR API (HttpClient).
    *   **Crucial:** Implement the Anomaly Detector Service.
        *   *Interface:* `IAnomalyDetector { AnalysisResult Analyze(Telemetry t); }`
        *   *Implementation:* Statistical (Z-Score) implementation for the demo to ensure 100% reliability without needing external API keys during the interview, but structure it to easily swap in Semantic Kernel.
2.  Add SignalR Hub to `FhirApi` (or separate hub) to broadcast updates to UI.

### Phase 4: The Experience (Dashboard)
1.  Build Blazor WASM layout.
2.  Create "Live Vitals" component using Chart.js interop.
3.  Implement "Scenario Control": A button on the UI that calls an endpoint on the Simulator (via the API->Gateway->Sim path or direct) to "Trigger Hypotension Event".

---

## 3. Technical Constraints & Standards
*   **Containerization:** `docker-compose.yml` MUST orchestrate: Mosquitto, Simulator, Gateway, API, Dashboard (served via Nginx or standard .NET container).
*   **Code Quality:**
    *   Use `.editorconfig` (enforce file-scoped namespaces).
    *   Structured Logging (Serilog) outputting to Console.
    *   Unit Tests for the *Transformation Logic* (Mapper tests).
*   **Documentation:** `README.md` must contain a MermaidJS sequence diagram showing the data flow: `Machine -> Modbus -> Gateway -> MQTT -> Transformer -> FHIR DB -> SignalR -> Browser`.

## 4. Success Criteria for Agent
*   **Zero-Config Start:** Running `docker-compose up -d` is the ONLY step required to start the full system.
*   **Self-Healing:** If the Simulator restarts, the Gateway must automatically reconnect.
*   **Visual Proof:** The Dashboard must immediately show moving charts upon load.

**Immediate Next Step:** Scaffold the solution structure and the FHIR API project.
