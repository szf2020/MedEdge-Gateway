namespace MedEdge.Core.Domain.Entities;

public class FhirDeviceEntity
{
    public string Id { get; set; } = default!;
    public string DeviceId { get; set; } = default!; // e.g., Device-001
    public string Manufacturer { get; set; } = default!; // Generic Medical Device
    public string Model { get; set; } = default!; // Dialog+, Dialog iQ
    public string SerialNumber { get; set; } = default!;
    public string Status { get; set; } = "active"; // active, inactive, off
    public string? AssignedPatientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public FhirPatientEntity? AssignedPatient { get; set; }
    public ICollection<FhirObservationEntity> Observations { get; set; } = new List<FhirObservationEntity>();
}
