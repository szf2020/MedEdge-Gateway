namespace MedEdge.Core.DTOs;

public record DeviceDto(
    string Id,
    string DeviceId,
    string Manufacturer,
    string Model,
    string SerialNumber,
    string Status
);

public record CreateDeviceRequest(
    string DeviceId,
    string Manufacturer,
    string Model,
    string SerialNumber
);
