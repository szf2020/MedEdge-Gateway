namespace MedEdge.EdgeGateway.Models;

public record TelemetryMessage(
    string DeviceId,
    DateTime Timestamp,
    Dictionary<string, double> Measurements,
    Dictionary<string, bool> Alarms
);
