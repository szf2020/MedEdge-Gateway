namespace MedEdge.Core.DTOs;

public record PatientDto(
    string Id,
    string Mrn,
    string GivenName,
    string FamilyName,
    DateTime BirthDate,
    string Gender
);

public record CreatePatientRequest(
    string Mrn,
    string GivenName,
    string FamilyName,
    DateTime BirthDate,
    string Gender
);
