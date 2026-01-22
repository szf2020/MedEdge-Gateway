namespace MedEdge.Core.Domain.Entities;

public class FhirObservationEntity
{
    public string Id { get; set; } = default!;
    public string PatientId { get; set; } = default!;
    public string DeviceId { get; set; } = default!;
    public string Code { get; set; } = default!; // LOINC code
    public string CodeDisplay { get; set; } = default!;
    public double Value { get; set; }
    public string Unit { get; set; } = default!;
    public string Status { get; set; } = "final"; // preliminary, final, amended, cancelled
    public DateTime ObservationTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public FhirPatientEntity? Patient { get; set; }
    public FhirDeviceEntity? Device { get; set; }
}
