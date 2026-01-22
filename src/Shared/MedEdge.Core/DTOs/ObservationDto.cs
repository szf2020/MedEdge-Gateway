namespace MedEdge.Core.DTOs;

public record ObservationDto(
    string Id,
    string PatientId,
    string DeviceId,
    string Code,
    string CodeDisplay,
    double Value,
    string Unit,
    string Status,
    DateTime ObservationTime
);

public record CreateObservationRequest(
    string PatientId,
    string DeviceId,
    string Code,
    string CodeDisplay,
    double Value,
    string Unit,
    DateTime ObservationTime
);
