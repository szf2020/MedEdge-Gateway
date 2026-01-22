namespace MedEdge.Core.Domain.Entities;

public class FhirPatientEntity
{
    public string Id { get; set; } = default!;
    public string Mrn { get; set; } = default!; // Medical Record Number
    public string GivenName { get; set; } = default!;
    public string FamilyName { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = "unknown"; // male, female, other, unknown
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<FhirDeviceEntity> AssignedDevices { get; set; } = new List<FhirDeviceEntity>();
    public ICollection<FhirObservationEntity> Observations { get; set; } = new List<FhirObservationEntity>();
}
