using FluentAssertions;
using MedEdge.Core.Domain.Entities;
using MedEdge.FhirApi.Services;
using Xunit;

namespace MedEdge.FhirApi.Tests.Services;

public class FhirMappingServiceTests
{
    private readonly FhirMappingService _service = new();

    [Fact]
    public void MapPatientEntityToFhir_WithValidPatient_ReturnsFhirPatient()
    {
        // Arrange
        var patient = new FhirPatientEntity
        {
            Id = "P001",
            Mrn = "MRN001",
            GivenName = "John",
            FamilyName = "Doe",
            BirthDate = new DateTime(1980, 1, 1),
            Gender = "male"
        };

        // Act
        var result = _service.MapPatientEntityToFhir(patient);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("P001");
        result.Name.Should().HaveCount(1);
        result.Name[0].Given.Should().Contain("John");
        result.Name[0].Family.Should().Be("Doe");
    }

    [Fact]
    public void MapDeviceEntityToFhir_WithValidDevice_ReturnsFhirDevice()
    {
        // Arrange
        var device = new FhirDeviceEntity
        {
            Id = "Device-001",
            DeviceId = "Device-001",
            Manufacturer = "Medical Device",
            Model = "Dialog+",
            SerialNumber = "SN001",
            Status = "active"
        };

        // Act
        var result = _service.MapDeviceEntityToFhir(device);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("Device-001");
        result.Manufacturer.Should().Be("Medical Device");
        result.ModelNumber.Should().Be("Dialog+");
    }

    [Fact]
    public void MapObservationEntityToFhir_WithValidObservation_ReturnsFhirObservation()
    {
        // Arrange
        var observation = new FhirObservationEntity
        {
            Id = "Obs-001",
            PatientId = "P001",
            DeviceId = "Device-001",
            Code = "85354-9",
            CodeDisplay = "Blood Pressure",
            Value = 120.0,
            Unit = "mmHg",
            Status = "final",
            ObservationTime = DateTime.UtcNow
        };

        // Act
        var result = _service.MapObservationEntityToFhir(observation);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("Obs-001");
        result.Code.Coding.Should().HaveCount(1);
        result.Code.Coding[0].Code.Should().Be("85354-9");
    }
}
