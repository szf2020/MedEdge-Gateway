# Learning .NET and C# - Beginner's Guide to Understanding MedEdge Gateway

**Target Audience:** Developers new to .NET/C# who want to understand MedEdge Gateway
**Learning Path:** 4-8 weeks (part-time)
**Prerequisites:** Basic programming concepts (variables, functions, loops)

---

## 📚 Table of Contents

1. [Phase 1: C# Fundamentals (Weeks 1-2)](#phase-1-c-fundamentals-weeks-1-2)
2. [Phase 2: Object-Oriented Programming (Weeks 2-3)](#phase-2-object-oriented-programming-weeks-2-3)
3. [Phase 3: .NET Essentials (Weeks 3-4)](#phase-3-net-essentials-weeks-3-4)
4. [Phase 4: ASP.NET Core & Web APIs (Weeks 4-5)](#phase-4-aspnet-core--web-apis-weeks-4-5)
5. [Phase 5: Applying to MedEdge (Weeks 5-8)](#phase-5-applying-to-medge-weeks-5-8)
6. [Resources & Next Steps](#resources--next-steps)

---

## Phase 1: C# Fundamentals (Weeks 1-2)

### Week 1: Basic C# Syntax

**Learning Goals:**
- Understand C# syntax and basic data types
- Write simple programs
- Use variables, operators, and control flow

**Key Concepts:**

#### 1.1 Data Types
```csharp
// Primitive types
int bloodFlow = 320;                    // Whole number (MedEdge: blood flow rate in mL/min)
double arterialPressure = 120.5;        // Decimal number (MedEdge: pressure in mmHg)
string deviceId = "Device-001";         // Text (MedEdge: device identifier)
bool isOnline = true;                   // Boolean (MedEdge: device status)

// Collections
List<int> measurements = new List<int> { 100, 110, 120 };  // MedEdge: vital sign readings
Dictionary<string, double> vitals = new()  // MedEdge: map vital names to values
{
    { "BloodFlow", 320 },
    { "ArterialPressure", 120 }
};
```

**Real MedEdge Example:**
In [ApplicationDbContext.cs:50-80](src/Cloud/MedEdge.FhirApi/Data/ApplicationDbContext.cs#L50-L80), you see data types defining patient and device information.

#### 1.2 Methods (Functions)
```csharp
// Method that calculates if pressure is normal
public bool IsPressureNormal(double pressure)
{
    return pressure >= 50 && pressure <= 200;  // Normal range for MedEdge
}

// Calling the method
bool isNormal = IsPressureNormal(120);  // Returns true
```

**Real MedEdge Example:**
[StatisticalAnomalyDetector.cs](src/Cloud/MedEdge.AiEngine/Services/StatisticalAnomalyDetector.cs) has methods like `DetectAnomaly()` that check if vital signs are abnormal.

#### 1.3 Control Flow (if/else, loops)
```csharp
// If statement - checking vital signs
if (arterialPressure < 80)
{
    Alert("Hypotension detected!");  // MedEdge: critical alert
}
else if (arterialPressure > 200)
{
    Alert("Hypertension detected!");
}
else
{
    Alert("Pressure normal");
}

// Loop - processing multiple device readings
foreach (var reading in measurements)
{
    if (reading < threshold)
    {
        ProcessAnomalyAlert(reading);
    }
}
```

**Real MedEdge Example:**
In [StatisticalAnomalyDetector.cs:45-65](src/Cloud/MedEdge.AiEngine/Services/StatisticalAnomalyDetector.cs), you see if statements checking clinical thresholds.

**Practice Exercise:**
```csharp
// TODO: Write a method that checks if temperature is safe
// Safe range: 35°C to 38°C
// If < 35: return "Hypothermia"
// If > 38: return "Fever"
// Otherwise: return "Normal"
```

### Week 2: Classes and Objects

**Learning Goals:**
- Understand classes (blueprints for objects)
- Create and use objects
- Understand properties and encapsulation

**Key Concepts:**

#### 2.1 Classes (Blueprints)
```csharp
// Blueprint for a vital sign measurement
public class VitalSignReading
{
    // Properties (characteristics)
    public string VitalType { get; set; }        // "Blood Flow", "Pressure", etc.
    public double Value { get; set; }            // The actual measurement
    public string Unit { get; set; }             // "mL/min", "mmHg", etc.
    public DateTime RecordedAt { get; set; }    // When it was measured
}

// Using the class (creating objects)
var bloodFlowReading = new VitalSignReading()
{
    VitalType = "Blood Flow",
    Value = 320,
    Unit = "mL/min",
    RecordedAt = DateTime.Now
};

// Accessing properties
Console.WriteLine($"Blood Flow: {bloodFlowReading.Value} {bloodFlowReading.Unit}");
// Output: Blood Flow: 320 mL/min
```

**Real MedEdge Example:**
[FhirObservationEntity.cs](src/Shared/MedEdge.Core/Domain/Entities/FhirObservationEntity.cs) is a class representing vital sign observations with properties like `Code`, `Value`, `Unit`, `ObservationTime`.

#### 2.2 Methods Inside Classes
```csharp
public class Device
{
    public string DeviceId { get; set; }
    public string Status { get; set; }  // "Online", "Offline", "Warning"

    // Method
    public bool IsHealthy()
    {
        return Status == "Online";
    }
}

// Using the method
var device = new Device { DeviceId = "Device-001", Status = "Online" };
if (device.IsHealthy())
{
    Console.WriteLine("Device is healthy");
}
```

**Real MedEdge Example:**
[FhirPatientEntity.cs](src/Shared/MedEdge.Core/Domain/Entities/FhirPatientEntity.cs) defines patient properties (Name, Gender, BirthDate, etc.).

#### 2.3 Encapsulation (Access Modifiers)
```csharp
public class ClinicalThreshold
{
    // Public: Anyone can see/change this
    public string Name { get; set; }

    // Private: Only this class can access (hidden from outside)
    private double _warningValue;
    private double _criticalValue;

    // Public method to safely get the warning threshold
    public double GetWarningThreshold()
    {
        return _warningValue;
    }
}
```

**Why This Matters for MedEdge:**
- Medical data is sensitive → need to control access
- Private fields protect internal logic
- Public methods expose safe interfaces

---

## Phase 2: Object-Oriented Programming (Weeks 2-3)

### Understanding Interfaces (Contracts)

**Key Concept:** An interface is a **contract** - it says "any class using me must implement these methods."

```csharp
// Interface: Contract for anomaly detection
public interface IAnomalyDetector
{
    // Any class implementing this must have these methods
    AnomalyResult DetectAnomaly(VitalSign vital);
}

// Class 1: Statistical detector (deterministic)
public class StatisticalAnomalyDetector : IAnomalyDetector
{
    public AnomalyResult DetectAnomaly(VitalSign vital)
    {
        // Check clinical thresholds
        if (vital.ArterialPressure < 80)
            return new AnomalyResult { Severity = "CRITICAL", Finding = "Hypotension" };

        return new AnomalyResult { Severity = "NORMAL" };
    }
}

// Class 2: LLM detector (AI-based)
public class LlmAnomalyDetector : IAnomalyDetector
{
    public AnomalyResult DetectAnomaly(VitalSign vital)
    {
        // Call LLM API for analysis
        var response = CallOpenAiApi(vital);
        return response;
    }
}

// Using both interchangeably
IAnomalyDetector detector1 = new StatisticalAnomalyDetector();
IAnomalyDetector detector2 = new LlmAnomalyDetector();

// Both work the same way from outside
var result1 = detector1.DetectAnomaly(vitalSign);
var result2 = detector2.DetectAnomaly(vitalSign);
```

**Real MedEdge Example:**
[MedEdge.AiEngine](src/Cloud/MedEdge.AiEngine) has both `StatisticalAnomalyDetector` and `SemanticKernelDetector` implementing the same detection interface.

**Why This Matters:**
- **Flexibility:** Easy to swap implementations without changing other code
- **Testing:** Can test with fake implementations
- **Scalability:** New detectors can be added without breaking existing code

### Inheritance (IS-A Relationship)

```csharp
// Base class
public class Service
{
    protected Logger logger;  // "protected" = subclasses can use it

    public Service()
    {
        logger = new Logger();
    }
}

// Derived class
public class TransformService : Service
{
    public void TransformMqttToFhir()
    {
        logger.Log("Transforming MQTT to FHIR");  // Can use inherited logger
    }
}
```

---

## Phase 3: .NET Essentials (Weeks 3-4)

### Understanding .NET Architecture

**What is .NET?**
- A **framework** (collection of pre-built tools and libraries)
- Runs on Windows, Linux, macOS
- C# code compiles to IL (Intermediate Language)
- IL runs in CLR (Common Language Runtime) - the "virtual machine"

```
Your C# Code → Compiler → IL Code → CLR (Runtime) → Machine Code
   (you write)         (dotnet)      (IL language)  (.NET runtime)  (executes)
```

### Key .NET Concepts

#### 3.1 Namespaces (Organization)
```csharp
// Namespace: organizes related code
namespace MedEdge.Cloud.Services
{
    public class FhirTransformService
    {
        // This class belongs to MedEdge.Cloud.Services namespace
    }
}

// Using the class
using MedEdge.Cloud.Services;

var transformer = new FhirTransformService();
```

**Real MedEdge Structure:**
```
MedEdge.Core                    (shared models)
├── Domain.Entities             (database entities)
├── DTOs                        (data transfer objects)

MedEdge.FhirApi                 (cloud API)
├── Controllers                 (handle HTTP requests)
├── Services                    (business logic)
├── Data                        (database access)

MedEdge.TransformService        (message processing)
├── Services                    (MQTT → FHIR transformation)

MedEdge.AiEngine                (anomaly detection)
├── Services                    (detection algorithms)
```

#### 3.2 Async/Await (Non-Blocking Operations)

**Problem:** If something takes time (database query, network call), your app freezes.

```csharp
// Bad: Blocks execution (freezes the app)
public void GetPatientSync()
{
    var patient = database.Query("SELECT * FROM Patients").Result;  // WAIT!
    Console.WriteLine(patient.Name);
}

// Good: Async (doesn't block)
public async Task GetPatientAsync()
{
    var patient = await database.QueryAsync("SELECT * FROM Patients");  // Don't wait, continue
    Console.WriteLine(patient.Name);
}
```

**Real MedEdge Example:**
In [TelemetryHub.cs](src/Cloud/MedEdge.FhirApi/Hubs/TelemetryHub.cs), SignalR methods are async:
```csharp
public async Task SubscribeToDevice(string deviceId)
{
    await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
}
```

**Why This Matters for MedEdge:**
- Multiple devices sending telemetry simultaneously
- Can't block on one device while waiting for another
- Async allows handling thousands of concurrent connections

#### 3.3 Dependency Injection (DI)

**Problem:** Classes depend on each other, making testing hard.

```csharp
// Bad: Logger is hardcoded (hard to test, hard to change)
public class DeviceService
{
    private Logger logger = new Logger();

    public void ProcessDevice()
    {
        logger.Log("Processing device");
    }
}

// Good: Inject dependency (flexible, testable)
public class DeviceService
{
    private ILogger logger;

    public DeviceService(ILogger logger)  // Inject from outside
    {
        this.logger = logger;
    }

    public void ProcessDevice()
    {
        logger.Log("Processing device");
    }
}

// Usage
var logger = new ConsoleLogger();
var service = new DeviceService(logger);
service.ProcessDevice();

// Testing with fake logger
var fakeLogger = new MockLogger();
var testService = new DeviceService(fakeLogger);
testService.ProcessDevice();
// fakeLogger.Messages contains what was logged
```

**Real MedEdge Example:**
In [Program.cs](src/Cloud/MedEdge.FhirApi/Program.cs), services are registered:
```csharp
builder.Services.AddScoped<IFhirRepository, FhirRepository>();
builder.Services.AddScoped<ITransformService, TransformService>();
```

---

## Phase 4: ASP.NET Core & Web APIs (Weeks 4-5)

### Understanding REST APIs

**What is an API?**
- **Interface** for applications to talk to each other
- Uses HTTP requests and responses
- REST = Representational State Transfer

#### 4.1 HTTP Verbs and MedEdge Endpoints

```csharp
// GET: Retrieve data (no side effects)
GET /fhir/Patient                    // Get all patients
GET /fhir/Patient/P001               // Get specific patient
GET /fhir/Observation?patient=P001   // Get observations for patient

// POST: Create new data
POST /fhir/Observation               // Create new observation
{
    "patientId": "P001",
    "deviceId": "Device-001",
    "code": "75992-9",
    "value": 120,
    "unit": "mmHg"
}

// PUT: Update existing data
PUT /fhir/Patient/P001
{
    "name": "John Doe Updated",
    "birthDate": "1965-03-15"
}

// DELETE: Remove data
DELETE /fhir/Patient/P001
```

**Real MedEdge Example:**
In [Program.cs](src/Cloud/MedEdge.FhirApi/Program.cs), endpoints are defined:
```csharp
app.MapGet("/fhir/Patient", GetPatients);
app.MapPost("/fhir/Observation", CreateObservation);
app.MapGet("/fhir/Observation", GetObservations);
```

#### 4.2 Controllers and Minimal APIs

```csharp
// Traditional: Controller class
[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDevice(string id)
    {
        var device = await _deviceService.GetDeviceAsync(id);
        return Ok(device);
    }
}

// Modern: Minimal API (MedEdge uses this)
app.MapGet("/api/devices/{id}", async (string id, IDeviceService service) =>
{
    var device = await service.GetDeviceAsync(id);
    return Results.Ok(device);
});
```

**MedEdge Uses Minimal APIs** - simpler, more direct.

#### 4.3 Understanding JSON and FHIR

**JSON:** Human-readable data format
```json
{
  "resourceType": "Observation",
  "id": "obs-001",
  "status": "final",
  "code": {
    "coding": [{
      "system": "http://loinc.org",
      "code": "75992-9",
      "display": "Arterial Pressure"
    }]
  },
  "value": {
    "value": 120,
    "unit": "mmHg"
  }
}
```

**FHIR (Fast Healthcare Interoperability Resources):**
- **Standard format** for healthcare data
- Healthcare systems understand this format
- Can exchange data between hospitals, devices, etc.

**Real MedEdge Example:**
In [FhirTransformService.cs](src/Cloud/MedEdge.TransformService/Services/FhirTransformService.cs), Modbus values are transformed to FHIR JSON:
```
Modbus Register (40003 = 120) → FHIR Observation JSON
```

#### 4.4 Database Access with Entity Framework Core

**What is EF Core?**
- **ORM** (Object-Relational Mapping)
- Maps C# classes to database tables
- Lets you write C# instead of SQL

```csharp
// C# class
public class Patient
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Mrn { get; set; }
}

// Becomes SQL table:
// CREATE TABLE Patients (
//     Id VARCHAR(50) PRIMARY KEY,
//     Name VARCHAR(255),
//     Mrn VARCHAR(50) UNIQUE
// )

// Using EF Core (no SQL needed)
var patient = await dbContext.Patients.FindAsync("P001");
patient.Name = "John Updated";
await dbContext.SaveChangesAsync();  // Executes UPDATE in database

// With Linq (Language Integrated Query)
var observations = dbContext.Observations
    .Where(o => o.PatientId == "P001")
    .OrderByDescending(o => o.ObservationTime)
    .ToList();
```

**Real MedEdge Example:**
In [ApplicationDbContext.cs](src/Cloud/MedEdge.FhirApi/Data/ApplicationDbContext.cs):
```csharp
public DbSet<FhirPatientEntity> Patients { get; set; }
public DbSet<FhirObservationEntity> Observations { get; set; }
```

---

## Phase 5: Applying to MedEdge (Weeks 5-8)

### Understanding MedEdge Architecture Through Code

#### 5.1 The Complete Flow: Device to Dashboard

Let me walk through the exact flow with code locations:

**Step 1: Device Simulator Generates Telemetry**
📍 [ModbusServerService.cs](src/Edge/MedEdge.DeviceSimulator/Services/ModbusServerService.cs)
```csharp
// Every 500ms, simulator generates vital signs
public class ModbusServerService : IHostedService
{
    public async Task ExecuteAsync()
    {
        while (true)
        {
            var bloodFlow = GenerateRealisticValue(320, 20);      // ~320 mL/min
            var arterialPressure = GenerateRealisticValue(120, 10);  // ~120 mmHg

            // Write to Modbus registers
            _modbusServer.SetRegister(40001, (ushort)bloodFlow);
            _modbusServer.SetRegister(40003, (ushort)arterialPressure);

            await Task.Delay(500);  // Update every 500ms
        }
    }
}
```

**Step 2: Edge Gateway Polls Modbus**
📍 [ModbusPollingService.cs](src/Edge/MedEdge.EdgeGateway/Services/ModbusPollingService.cs)
```csharp
public class ModbusPollingService : IHostedService
{
    public async Task ExecuteAsync()
    {
        while (true)
        {
            // Read registers from simulator
            var registers = await _modbusClient.ReadHoldingRegistersAsync(
                slaveAddress: 1,
                startAddress: 40001,
                quantity: 12
            );

            // Transform to engineering units
            var bloodFlow = ConvertRegisterToBloodFlow(registers[0]);        // 320
            var arterialPressure = ConvertRegisterToPressure(registers[2]); // 120

            // Create telemetry message
            var telemetry = new TelemetryMessage
            {
                DeviceId = "Device-001",
                Timestamp = DateTime.UtcNow,
                Measurements = new()
                {
                    { "BloodFlow", bloodFlow },
                    { "ArterialPressure", arterialPressure }
                }
            };

            // Publish to MQTT
            await _mqttPublisher.PublishAsync("bbraun/dialysis/Device-001/telemetry",
                JsonConvert.SerializeObject(telemetry));

            await Task.Delay(500);
        }
    }
}
```

**Step 3: Transform Service Receives MQTT**
📍 [MqttSubscriberService.cs](src/Cloud/MedEdge.TransformService/Services/MqttSubscriberService.cs)
```csharp
public class MqttSubscriberService : IHostedService
{
    public async Task ExecuteAsync()
    {
        // Subscribe to telemetry topic
        await _mqttClient.SubscribeAsync("bbraun/dialysis/+/telemetry");

        _mqttClient.ApplicationMessageReceived += async (args) =>
        {
            // Parse received JSON
            var json = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
            var telemetry = JsonConvert.DeserializeObject<TelemetryMessage>(json);

            // Forward to Transform Service for FHIR conversion
            await _transformService.TransformTelemetryAsync(telemetry);
        };
    }
}
```

**Step 4: Transform to FHIR Observations**
📍 [FhirTransformService.cs](src/Cloud/MedEdge.TransformService/Services/FhirTransformService.cs)
```csharp
public class FhirTransformService
{
    public async Task TransformTelemetryAsync(TelemetryMessage telemetry)
    {
        var observations = new List<Observation>();

        // Create FHIR Observation for each vital sign
        foreach (var (vitalName, value) in telemetry.Measurements)
        {
            var observation = new Observation
            {
                Status = PublicationStatus.Final,
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding("http://loinc.org", GetLoincCode(vitalName))
                    }
                },
                Subject = new ResourceReference($"Patient/{GetPatientId(telemetry.DeviceId)}"),
                Device = new ResourceReference($"Device/{telemetry.DeviceId}"),
                Value = new Quantity
                {
                    Value = value,
                    Unit = GetUnit(vitalName)  // "mL/min", "mmHg", etc.
                },
                Issued = telemetry.Timestamp.ToDateTimeOffset()
            };

            observations.Add(observation);
        }

        // Save to database via FHIR API
        foreach (var obs in observations)
        {
            await _fhirApiClient.PostAsync("/fhir/Observation", obs);
        }
    }
}
```

**Step 5: AI Engine Detects Anomalies**
📍 [StatisticalAnomalyDetector.cs](src/Cloud/MedEdge.AiEngine/Services/StatisticalAnomalyDetector.cs)
```csharp
public class StatisticalAnomalyDetector
{
    public AnomalyResult DetectAnomaly(Observation observation)
    {
        // Get the LOINC code to know which vital it is
        var loincCode = observation.Code.Coding[0].Code;
        var value = (double)observation.Value?.Value;

        // Check against clinical thresholds
        switch (loincCode)
        {
            case "75992-9":  // Arterial Pressure
                if (value < 80)
                    return new AnomalyResult
                    {
                        Severity = RiskLevel.Critical,
                        Finding = "Hypotension detected - Arterial Pressure < 80 mmHg",
                        Recommendation = new[]
                        {
                            "Verify arterial needle position",
                            "Check access pressure limits",
                            "Consider reducing ultrafiltration"
                        }
                    };
                break;

            case "33438-3":  // Blood Flow Rate
                if (value < 150)
                    return new AnomalyResult
                    {
                        Severity = RiskLevel.Critical,
                        Finding = "Critical low blood flow",
                        Recommendation = new[] { "Check for clotting", "Reposition needle" }
                    };
                break;
        }

        return new AnomalyResult { Severity = RiskLevel.Normal };
    }
}
```

**Step 6: Broadcast to Dashboard via SignalR**
📍 [TelemetryHub.cs](src/Cloud/MedEdge.FhirApi/Hubs/TelemetryHub.cs)
```csharp
public class TelemetryHub : Hub
{
    public async Task BroadcastVitalSignUpdate(string deviceId, Observation observation)
    {
        // Send to all clinicians watching this device
        await Clients.Group(deviceId).SendAsync("VitalSignUpdate", observation);
    }

    public async Task BroadcastAlerts(string deviceId, AnomalyResult alert)
    {
        // Push alert immediately to dashboard
        await Clients.Group(deviceId).SendAsync("AlertsReceived", alert);
    }
}
```

**Step 7: Dashboard Receives Real-Time Update**
📍 [VitalsMonitor.razor](src/Web/MedEdge.Dashboard/Pages/VitalsMonitor.razor)
```csharp
@page "/vitals"
@using Microsoft.AspNetCore.SignalR.Client

@code {
    private HubConnection hubConnection;
    private Observation latestObservation;
    private AnomalyResult latestAlert;

    protected override async Task OnInitializedAsync()
    {
        // Connect to SignalR Hub
        hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5001/hubs/telemetry")
            .WithAutomaticReconnect()
            .Build();

        // Listen for vital sign updates from server
        hubConnection.On<Observation>("VitalSignUpdate", (observation) =>
        {
            latestObservation = observation;
            StateHasChanged();  // Refresh UI
        });

        // Listen for alerts
        hubConnection.On<AnomalyResult>("AlertsReceived", (alert) =>
        {
            latestAlert = alert;
            StateHasChanged();  // Show alert on dashboard
        });

        await hubConnection.StartAsync();
    }
}
```

#### 5.2 Key Patterns Used in MedEdge

**Pattern 1: Repository Pattern (Data Access)**
```
Controller → Service → Repository → Database
              ↑
         (business logic)   (data access)
```

**Pattern 2: Service Pattern (Business Logic)**
```csharp
// Repository handles database
var observations = await _observationRepository.GetByPatientAsync(patientId);

// Service handles business logic
var alerts = await _anomalyDetectionService.AnalyzeAsync(observations);

// Controller orchestrates
return Ok(new { observations, alerts });
```

**Pattern 3: Dependency Injection (Loose Coupling)**
```csharp
// Constructor injection
public class DeviceController
{
    private readonly IDeviceService _service;

    public DeviceController(IDeviceService service)  // Injected
    {
        _service = service;
    }
}
```

#### 5.3 Reading MedEdge Code: Step-by-Step Guide

**Start Here (Entry Points):**

1. **Understanding the Entry Point**
   📍 [Program.cs](src/Cloud/MedEdge.FhirApi/Program.cs) - Where everything starts
   - Line 1-10: Service registration (Dependency Injection)
   - Line 15-30: Endpoint definitions (GET /fhir/Patient, POST /fhir/Observation)
   - Line 40-45: SignalR configuration

2. **Understanding Data Models**
   📍 [FhirObservationEntity.cs](src/Shared/MedEdge.Core/Domain/Entities/FhirObservationEntity.cs)
   - Look at properties: Code, Value, Unit, PatientId, DeviceId, ObservationTime
   - These map to FHIR Observation resource

3. **Understanding Database**
   📍 [ApplicationDbContext.cs](src/Cloud/MedEdge.FhirApi/Data/ApplicationDbContext.cs)
   - Shows how classes map to database tables
   - Seed data shows sample patients and devices

4. **Understanding FHIR Transformation**
   📍 [FhirTransformService.cs](src/Cloud/MedEdge.TransformService/Services/FhirTransformService.cs)
   - Shows how Modbus values become FHIR resources
   - See the LOINC code mapping

5. **Understanding AI Detection**
   📍 [StatisticalAnomalyDetector.cs](src/Cloud/MedEdge.AiEngine/Services/StatisticalAnomalyDetector.cs)
   - Shows clinical thresholds
   - Shows how anomalies are detected

6. **Understanding Real-Time Communication**
   📍 [TelemetryHub.cs](src/Cloud/MedEdge.FhirApi/Hubs/TelemetryHub.cs)
   - Shows how server pushes data to clients
   - Group broadcasting for device subscriptions

7. **Understanding Dashboard**
   📍 [VitalsMonitor.razor](src/Web/MedEdge.Dashboard/Pages/VitalsMonitor.razor)
   - Shows how to connect to SignalR
   - Shows how to display real-time data

---

## Resources & Next Steps

### Free Online Resources

**Beginner C# and .NET:**
1. **Microsoft Learn** (Official - Free)
   - https://learn.microsoft.com/en-us/dotnet/csharp/
   - C# Fundamentals for Absolute Beginners
   - Introduction to .NET

2. **YouTube Channels:**
   - Microsoft .NET on YouTube (official tutorials)
   - Bro Code - Complete C# Tutorial (3-4 hours)
   - Programming with Mosh - C# Basics

3. **Interactive Practice:**
   - https://www.codewars.com/ (practice with real problems)
   - https://www.leetcode.com/ (algorithm practice)
   - https://dotnetfiddle.net/ (try C# code online)

**ASP.NET Core & Web APIs:**
1. **Microsoft Learn Path**
   - Build web APIs with ASP.NET Core
   - https://learn.microsoft.com/training/paths/build-web-api-minimal-api/

2. **YouTube:**
   - Programming with Mosh - ASP.NET Core for Beginners
   - Code with Ado - ASP.NET Core REST API Tutorial

**Healthcare/FHIR:**
1. **FHIR Basics**
   - https://www.hl7.org/fhir/overview.html (official)
   - https://youtu.be/hOmUDfgRrn0 (FHIR explainer video)

2. **Modbus/IoT:**
   - https://modbus.org/docs/Modbus_Application_Protocol_V1_1b3.pdf
   - NModbus library documentation

### Recommended Learning Order

```
Week 1-2:   C# Basics (data types, methods, classes)
            ↓
Week 2-3:   OOP Concepts (interfaces, inheritance, encapsulation)
            ↓
Week 3-4:   .NET Fundamentals (namespaces, async/await, DI)
            ↓
Week 4-5:   ASP.NET Core & APIs (HTTP, REST, Controllers)
            ↓
Week 5-6:   Entity Framework Core (databases, LINQ)
            ↓
Week 6-7:   Blazor & Real-time (SignalR, WebAssembly)
            ↓
Week 7-8:   Applying to MedEdge (read and understand full project)
            ↓
Week 8+:    Contribute to MedEdge (add features, fix bugs, improve code)
```

### Practice Projects to Build

After each phase, build a mini-project:

**Phase 1-2: Simple Fitness Tracker**
```csharp
// Track heart rate and blood pressure measurements
// Calculate averages
// Detect if values are abnormal
// (Similar to MedEdge vital sign tracking)
```

**Phase 3-4: Weather API**
```csharp
// REST API that returns weather data
// Use EF Core to store history
// Use async methods
// Use dependency injection
```

**Phase 5: Device Monitoring Dashboard**
```csharp
// Build a simple monitoring system
// Receive telemetry via MQTT or HTTP
// Store in database
// Display in Blazor dashboard with real-time updates
// (Simplified version of MedEdge)
```

---

## Understanding MedEdge-Specific Concepts

### Medical Concepts (Don't Need to Know, But Helpful)

**Dialysis:**
- Medical procedure for patients with kidney failure
- Cleans blood by removing excess water and waste
- Done on Dialog+ or Dialog iQ machines
- Vital signs must be monitored continuously

**Key Vital Signs in MedEdge:**
1. **Blood Flow Rate** (Qb) - 200-400 mL/min
   - Too low = clotting risk
   - Too high = hemolysis risk

2. **Arterial Pressure** (Pa) - 50-200 mmHg
   - < 80 = Hypotension (dangerous - alert critical)
   - > 200 = Hypertension

3. **Venous Pressure** (Pv) - 50-200 mmHg
   - Indicates access function

4. **Temperature** - 35-38°C
   - > 38.5 = Fever warning

5. **Conductivity** - 13.5-14.5 mS/cm
   - Measure of dialysate solution

### Industrial IoT Concepts

**Modbus TCP:**
- Protocol for reading/writing equipment registers
- Used in industrial machines
- Simple request/response (read registers 40001-40012)
- MedEdge simulator acts as Modbus server

**MQTT:**
- Lightweight messaging protocol for IoT
- Pub/Sub model (Publisher-Subscriber)
- Topics: `bbraun/dialysis/Device-001/telemetry`
- Used by Edge Gateway to send telemetry to cloud

**FHIR (Healthcare Standard):**
- Standardized format for healthcare data
- Can exchange data between different hospital systems
- Uses JSON/XML with defined structure
- Every FHIR resource has specific required fields

### Real-Time Concepts

**SignalR:**
- Microsoft technology for real-time communication
- Establishes WebSocket connection
- Server can push data to clients (not polling)
- Clients grouped by device for targeted broadcast

---

## Cheat Sheet: Quick Reference

### C# Syntax Quick Reference
```csharp
// Variables
var x = 5;                          // Inferred type
int bloodFlow = 320;
string deviceId = "Device-001";

// Collections
List<int> numbers = new() { 1, 2, 3 };
Dictionary<string, double> vitals = new() { { "BP", 120 } };

// Methods
public int Add(int a, int b) => a + b;          // Expression body
public void Log(string msg) => Console.WriteLine(msg);

// Classes
public class Patient
{
    public string Name { get; set; }
}

// Interfaces
public interface IService
{
    Task<string> GetDataAsync();
}

// Async
public async Task<string> FetchDataAsync()
{
    var result = await someService.GetAsync();
    return result;
}

// LINQ
var adults = patients.Where(p => p.Age >= 18).ToList();
```

### .NET Project Structure
```
MyProject/
├── Program.cs                  (Main entry point)
├── appsettings.json           (Configuration)
├── Controllers/               (HTTP endpoints)
├── Services/                  (Business logic)
├── Data/                      (Database)
└── Models/                    (Data models)
```

### Common MedEdge Patterns
```csharp
// 1. Dependency Injection
public MyService(IRepository repo)  // Injected via constructor
{
    _repo = repo;
}

// 2. Async/Await
var data = await _service.GetDataAsync();

// 3. Repository Pattern
var observation = await _observationRepository.GetByIdAsync(id);

// 4. SignalR Broadcasting
await Clients.Group(deviceId).SendAsync("Update", data);

// 5. LINQ Query
var alerts = observations
    .Where(o => IsAbnormal(o))
    .OrderByDescending(o => o.Timestamp)
    .ToList();
```

---

## Next Steps After Learning

### Week 8+: Contribute to MedEdge

**Easy Contributions:**
1. Add new clinical threshold
2. Add new vital sign type
3. Improve error messages
4. Add documentation
5. Fix typos in code

**Medium Contributions:**
1. Add new dashboard page
2. Add new API endpoint
3. Add unit tests
4. Improve performance

**Hard Contributions:**
1. Implement OAuth 2.0 authentication
2. Add multi-tenancy support
3. Implement Kubernetes deployment
4. Add PostgreSQL migration

---

## Troubleshooting: Common Beginner Issues

### Issue 1: "I don't understand async/await"
**Solution:** Think of it as delegation:
- Normal: "I'll wait here until you finish"
- Async: "You go do that, I'll work on something else, tell me when you're done"

### Issue 2: "I don't understand Dependency Injection"
**Solution:** Instead of creating dependencies inside a class:
```csharp
// Bad: Hardcoded
public class Service
{
    private Logger logger = new Logger();
}

// Good: Injected (flexible)
public class Service
{
    public Service(ILogger logger) => this.logger = logger;
}
```

### Issue 3: "I don't understand Interfaces"
**Solution:** Interface = contract = "promise to implement these methods"
```csharp
// Contract
public interface IPaymentProcessor
{
    bool Process(decimal amount);
}

// Implementation 1
public class CreditCardProcessor : IPaymentProcessor
{
    public bool Process(decimal amount) { /* ... */ }
}

// Implementation 2
public class PayPalProcessor : IPaymentProcessor
{
    public bool Process(decimal amount) { /* ... */ }
}

// Either works the same way from outside
IPaymentProcessor processor = new CreditCardProcessor();
processor.Process(100);
```

---

## Conclusion

Learning .NET and C# is a journey, not a destination. MedEdge Gateway is a complex, real-world project that uses advanced patterns. Don't try to understand everything at once.

**Recommended approach:**
1. **Week 1-2:** Learn C# basics (can read simple code)
2. **Week 3-4:** Learn .NET (can understand structure)
3. **Week 5-6:** Learn ASP.NET (can read web APIs)
4. **Week 7+:** Study MedEdge (start reading actual code)
5. **Month 2+:** Contribute to MedEdge (make changes confidently)

**Remember:** Every expert was once a beginner. The fact that you're learning shows you're on the right path! 🚀

---

**Last Updated:** 2026-01-16
**MedEdge Version:** 5.0 (Phase 5 Complete)
**Difficulty Level:** Beginner to Intermediate

