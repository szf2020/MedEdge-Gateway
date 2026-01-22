# FHIR Mapping Guide - MedEdge Gateway

## Overview

This document describes the mapping between raw dialysis machine telemetry and FHIR R4 resources.

## Telemetry to FHIR Observation Mapping

| Measurement | LOINC Code | Display Name | Unit | Min | Max | CRITICAL | WARNING |
|-------------|-----------|--------------|------|-----|-----|----------|---------|
| Blood Flow | 33438-3 | Blood Flow Rate | mL/min | 200 | 400 | <150 | N/A |
| Arterial Pressure | 75992-9 | Arterial Pressure | mmHg | 50 | 200 | <80 | N/A |
| Venous Pressure | 60956-0 | Venous Pressure | mmHg | 50 | 200 | >250 | N/A |
| Temperature | 8310-5 | Body Temperature | °C | 35.0 | 38.0 | N/A | >38.5 |
| Conductivity | 2164-2 | Conductivity | mS/cm | 13.5 | 14.5 | <13.0 or >15.0 | N/A |
| Treatment Time | - | Duration | seconds | 0 | 14400 | N/A | N/A |

## FHIR Resource Structure

### 1. Patient Resource

Maps to `FhirPatientEntity`

```json
{
  "resourceType": "Patient",
  "id": "P001",
  "identifier": [
    {
      "system": "http://example.org/mrn",
      "value": "P001"
    }
  ],
  "name": [
    {
      "use": "official",
      "given": ["John"],
      "family": "Doe"
    }
  ],
  "birthDate": "1965-03-15",
  "gender": "male",
  "contact": [
    {
      "relationship": [
        {
          "coding": [
            {
              "system": "http://terminology.hl7.org/CodeSystem/v2-0131",
              "code": "N"
            }
          ]
        }
      ]
    }
  ]
}
```

### 2. Device Resource

Maps to `FhirDeviceEntity`

```json
{
  "resourceType": "Device",
  "id": "Device-001",
  "identifier": [
    {
      "system": "http://bbraun.com/device-id",
      "value": "Device-001"
    },
    {
      "system": "http://bbraun.com/serial",
      "value": "DG001"
    }
  ],
  "status": "active",
  "manufacturer": "Medical Device",
  "modelNumber": "Dialog+",
  "type": {
    "coding": [
      {
        "system": "http://snomed.info/sct",
        "code": "108662001",
        "display": "Hemodialysis machine"
      }
    ]
  }
}
```

### 3. Observation Resource

Maps to `FhirObservationEntity`

#### Blood Flow Observation
```json
{
  "resourceType": "Observation",
  "id": "obs-bf-001",
  "status": "final",
  "code": {
    "coding": [
      {
        "system": "http://loinc.org",
        "code": "33438-3",
        "display": "Blood Flow Rate"
      }
    ]
  },
  "subject": {
    "reference": "Patient/P001"
  },
  "device": {
    "reference": "Device/Device-001"
  },
  "effectiveDateTime": "2026-01-16T10:30:00Z",
  "value": {
    "value": 320,
    "unit": "mL/min",
    "system": "http://unitsofmeasure.org",
    "code": "mL/min"
  },
  "interpretation": [
    {
      "coding": [
        {
          "system": "http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation",
          "code": "N",
          "display": "Normal"
        }
      ]
    }
  ]
}
```

#### Arterial Pressure Observation
```json
{
  "resourceType": "Observation",
  "id": "obs-ap-001",
  "status": "final",
  "code": {
    "coding": [
      {
        "system": "http://loinc.org",
        "code": "75992-9",
        "display": "Arterial Pressure"
      }
    ]
  },
  "subject": {
    "reference": "Patient/P001"
  },
  "device": {
    "reference": "Device/Device-001"
  },
  "effectiveDateTime": "2026-01-16T10:30:00Z",
  "value": {
    "value": 120,
    "unit": "mmHg",
    "system": "http://unitsofmeasure.org",
    "code": "mm[Hg]"
  }
}
```

### 4. DeviceRequest Resource (Command)

For bi-directional control:

```json
{
  "resourceType": "DeviceRequest",
  "id": "dreq-001",
  "status": "active",
  "intent": "order",
  "code": {
    "coding": [
      {
        "system": "http://bbraun.com/device-commands",
        "code": "emergency-stop",
        "display": "Emergency Stop"
      }
    ]
  },
  "subject": {
    "reference": "Patient/P001"
  },
  "device": {
    "reference": "Device/Device-001"
  },
  "authoredOn": "2026-01-16T10:35:00Z"
}
```

## Unit Conversion Reference

| Source Unit | UCUM Code | Display |
|------------|-----------|---------|
| mL/min | mL/min | milliliters per minute |
| mmHg | mm[Hg] | millimeters of mercury |
| °C | Cel | degrees Celsius |
| mS/cm | mS/cm | millisiemens per centimeter |
| bpm | /min | beats per minute |

## USCDI v3 Compliance

MedEdge Gateway supports the following US Core Data for Interoperability v3 data classes:

1. **Patient Demographics** ✅
   - Name, gender, birthDate, MRN

2. **Vital Signs** ✅
   - Blood pressure, heart rate, body temperature

3. **Laboratory Values** ✅
   - Via Observation with LOINC codes

4. **Procedures** ✅
   - Hemodialysis procedure tracking via Device+Observation

5. **Medications** (Future)
   - Medication resource for dialysis medications

6. **Allergies & Adverse Reactions** (Future)
   - AllergyIntolerance resource

## Search Parameters

The FHIR API supports the following search parameters:

### Patient Search
```
GET /fhir/Patient?identifier=P001
GET /fhir/Patient?family=Doe&given=John
GET /fhir/Patient?birthdate=ge1965-03-15
```

### Device Search
```
GET /fhir/Device?type=http://snomed.info/sct|108662001
GET /fhir/Device?manufacturer=B.+Braun
```

### Observation Search
```
GET /fhir/Observation?patient=P001
GET /fhir/Observation?device=Device-001
GET /fhir/Observation?code=33438-3
GET /fhir/Observation?date=ge2026-01-16&date=le2026-01-17
GET /fhir/Observation?code=http://loinc.org|33438-3&value-quantity=ge300&value-quantity=le400
```

## Date and Time Handling

- All timestamps are in UTC (ISO 8601 format)
- Effective datetime from telemetry Timestamp
- Created/Updated timestamps for audit trail
- Server processes observations in chronological order

## Error Handling

Invalid FHIR operations return OperationOutcome:

```json
{
  "resourceType": "OperationOutcome",
  "issue": [
    {
      "severity": "error",
      "code": "not-found",
      "diagnostics": "Patient P999 not found"
    }
  ]
}
```

## Future Extensions

- FHIR R4 Subscriptions for real-time alerts
- Bulk Data API ($export endpoint)
- SMART on FHIR for patient authorization
- Custom extensions for medical device specific data
- Multi-patient treatment correlations

---

**Last Updated:** 2026-01-16
**Status:** Phase 1-3 Complete
