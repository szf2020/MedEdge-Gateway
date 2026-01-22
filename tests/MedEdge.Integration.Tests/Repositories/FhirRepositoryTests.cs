using FluentAssertions;
using MedEdge.Core.DTOs;
using MedEdge.FhirApi.Data;
using MedEdge.FhirApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MedEdge.Integration.Tests.Repositories;

public class FhirRepositoryTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CreatePatient_WithValidRequest_CreatesPatient()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new FhirRepository(context);
        var request = new CreatePatientRequest(
            "MRN001",
            "Jane",
            "Smith",
            new DateTime(1980, 1, 1),
            "female"
        );

        // Act
        var result = await repository.CreatePatientAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Mrn.Should().Be("MRN001");
        result.GivenName.Should().Be("Jane");
    }

    [Fact]
    public async Task GetPatientById_WithExistingPatient_ReturnsPatient()
    {
        // Arrange
        var context = CreateDbContext();
        var repository = new FhirRepository(context);
        var request = new CreatePatientRequest("MRN001", "John", "Doe", new DateTime(1980, 1, 1), "male");
        var created = await repository.CreatePatientAsync(request);

        // Act
        var result = await repository.GetPatientByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Mrn.Should().Be("MRN001");
    }

    [Fact]
    public async Task GetAllPatients_WithSeededData_ReturnsAllPatients()
    {
        // Arrange
        var context = CreateDbContext();
        await context.Database.MigrateAsync();
        var repository = new FhirRepository(context);

        // Act
        var result = await repository.GetAllPatientsAsync();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task CreateObservation_WithValidRequest_CreatesObservation()
    {
        // Arrange
        var context = CreateDbContext();
        await context.Database.MigrateAsync();
        var repository = new FhirRepository(context);

        var request = new CreateObservationRequest(
            "P001",
            "Device-001",
            "85354-9",
            "Blood Pressure",
            120.0,
            "mmHg",
            DateTime.UtcNow
        );

        // Act
        var result = await repository.CreateObservationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("85354-9");
        result.Value.Should().Be(120.0);
    }

    [Fact]
    public async Task GetObservationsByPatient_WithValidPatient_ReturnsObservations()
    {
        // Arrange
        var context = CreateDbContext();
        await context.Database.MigrateAsync();
        var repository = new FhirRepository(context);

        var request = new CreateObservationRequest(
            "P001",
            "Device-001",
            "85354-9",
            "Blood Pressure",
            120.0,
            "mmHg",
            DateTime.UtcNow
        );
        await repository.CreateObservationAsync(request);

        // Act
        var result = await repository.GetObservationsByPatientAsync("P001");

        // Assert
        result.Should().NotBeEmpty();
    }
}
