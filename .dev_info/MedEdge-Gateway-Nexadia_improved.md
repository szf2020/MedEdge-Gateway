# ENHANCED Claude Code Prompt: **"NEXADIA Evolution Platform"**

**Project Goal:** Build a production-grade prototype ("NEXADIA Evolution") to demonstrate architectural mastery for B. Braun's **IIoT Architect** and **FHIR Backend Developer** roles.

**Core Philosophy:** "Show, Don't Just Tell." The system must run locally (`docker-compose up`) and visually demonstrate the data journey from a dialysis machine to a hospital dashboard, highlighting **Bi-directional FHIR** and **AI Insights**.

---

## 1. The "NEXADIA Evolution" Architecture

### A. Edge Layer (The "Dialysis Clinic")
*   **Focus:** Reliability, Protocol Translation, Buffer/Retry.
*   **Components:**
    *   `src/Edge/DeviceSimulator`:
        *   **Role:** Simulates **Dialog+** and **Dialog iQ** machines.
        *   **Protocol:** Modbus TCP.
        *   **Behavior:** Generates realistic telemetry (Venous/Arterial Pressure, Conductivity).
        *   **Interactive:** "Chaos Mode" button to inject network failures or clinical alarms.
    *   `src/Edge/EdgeGateway`:
        *   **Tech:** .NET 8 Worker Service, `MQTTnet`, `Polly`, `NModbus`.
        *   **Role:** The "Sync" replacement. Polling Modbus -> JSON -> MQTT (TLS).
        *   **Key Feature:** "Store & Forward" (buffer messages in SQLite when MQTT broker is offline).

### B. Cloud & Intelligence Layer (The "Brain")
*   **Focus:** Interoperability (FHIR R4), Analytics.
*   **Components:**
    *   `src/Cloud/NexadiaHub` (ASP.NET Core Web API):
        *   **FHIR Server:** Implements `Patient`, `Device`, `Observation`, `DeviceMetric`.
        *   **Tech:** `Firely SDK`, EF Core (PostgreSQL for Prod / SQLite for Demo).
        *   **Innovation:** **FHIR Subscriptions (R4)**. Allow the Dashboard to "subscribe" to critical alerts via WebSockets.
    *   `src/Cloud/AiEngine` (The Innovation):
        *   **Role:** Clinical Decision Support.
        *   **Strategy:** **Hybrid AI**.
            *   *Layer 1 (Deterministic):* Standard Deviation checks for immediate "Hard Alarms" (e.g., Hypotension).
            *   *Layer 2 (Generative):* Semantic Kernel (with Mock/Azure toggle) to generate "Clinical Explanations" for the alarm (e.g., "Pressure drop correlates with ultrafiltration rate").

### C. Presentation Layer (The "Ward")
*   **Focus:** Real-time visibility, User Experience.
*   **Components:**
    *   `src/Web/NexadiaDashboard` (Blazor WebAssembly):
        *   **Visuals:** 3-Panel Layout (Fleet Status, Live Vitals, FHIR Inspector).
        *   **Tech:** `MudBlazor` for UI components, `Chart.js` for waveforms, `SignalR` for live push.

---

## 2. Strategic Implementation Roadmap

### Phase 1: The FHIR Backbone
**Objective:** A working FHIR API that accepts and retrieves clinical data.
1.  **Project Init:** Scaffold `MedEdge.sln` with Clean Architecture.
2.  **FHIR API:** Implement `POST /Observation` and `GET /Patient`.
3.  **Storage:** Setup EF Core with SQLite for zero-dep local run.
4.  **Verification:** Swagger UI is accessible and functional.

### Phase 2: The Industrial Data Pipeline
**Objective:** Real-time data flowing from simulated hardware.
1.  **Simulator:** Create the Modbus TCP server with "Dialog" profiles.
2.  **Gateway:** Implement the Modbus Poller -> MQTT Publisher loop.
3.  **Infrastructure:** Dockerize Mosquitto and link the containers.
4.  **Verification:** MQTT Explorer shows telemetry flowing.

### Phase 3: "Intelligence" & Bi-Directionality
**Objective:** The "Wow" Factor.
1.  **Transformation:** Map raw MQTT JSON to FHIR `Observation` resources.
2.  **AI Integration:** Implement the `AnomalyDetector` service.
3.  **Feedback Loop:** Implement a "Stop Pump" command (FHIR `DeviceRequest` -> MQTT -> Modbus Write) to prove bi-directionality.

### Phase 4: The Dashboard
**Objective:** Visual proof of the system.
1.  **UI Shell:** Build the Blazor layout with B. Braun colors (White/Green/Grey).
2.  **Live Charts:** Connect SignalR to visualize the waveform data.
3.  **Scenario Control:** Add the "Trigger Anomaly" button to the UI.

---

## 3. Technical Constraints & "Agent" Instructions

*   **Self-Contained:** The solution **must** run without external Azure dependencies by default. Use a "Mock AI" service implementation if no API key is provided in `appsettings.json`.
*   **Code Quality:**
    *   Use **File-Scoped Namespaces**.
    *   Use **Records** for DTOs/Messages.
    *   Use **Dependency Injection** for everything.
*   **Documentation:**
    *   `README.md` must include a **Mermaid Sequence Diagram** explaining the flow.
    *   `DEMO.md` must provide a script: "Click this, then watch that."

## 4. Success Criteria

1.  [ ] **`docker-compose up`** brings up 5 containers (Sim, Broker, Gateway, API, UI).
2.  [ ] **Dashboard** loads at `http://localhost:5000` and shows moving charts immediately.
3.  [ ] **Chaos Button** works: Clicking "Simulate Hypotension" causes a RED alert on the dashboard within 2 seconds.
4.  [ ] **FHIR Inspector** shows valid JSON `Observation` resources being created.
5.  [ ] **Bi-Directional:** Clicking "Emergency Stop" on UI stops the data stream from the simulator.

**Immediate Next Step:** Execute **Phase 1** (Project Scaffolding & FHIR API).
