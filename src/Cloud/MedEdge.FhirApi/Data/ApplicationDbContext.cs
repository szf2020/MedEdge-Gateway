using MedEdge.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedEdge.FhirApi.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<FhirPatientEntity> Patients { get; set; } = default!;
    public DbSet<FhirDeviceEntity> Devices { get; set; } = default!;
    public DbSet<FhirObservationEntity> Observations { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient entity
        modelBuilder.Entity<FhirPatientEntity>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<FhirPatientEntity>()
            .HasMany(p => p.AssignedDevices)
            .WithOne(d => d.AssignedPatient)
            .HasForeignKey(d => d.AssignedPatientId);

        modelBuilder.Entity<FhirPatientEntity>()
            .HasMany(p => p.Observations)
            .WithOne(o => o.Patient)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Device entity
        modelBuilder.Entity<FhirDeviceEntity>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<FhirDeviceEntity>()
            .HasMany(d => d.Observations)
            .WithOne(o => o.Device)
            .HasForeignKey(o => o.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Observation entity
        modelBuilder.Entity<FhirObservationEntity>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<FhirObservationEntity>()
            .HasIndex(o => o.PatientId);

        modelBuilder.Entity<FhirObservationEntity>()
            .HasIndex(o => o.DeviceId);

        modelBuilder.Entity<FhirObservationEntity>()
            .HasIndex(o => o.Code);

        modelBuilder.Entity<FhirObservationEntity>()
            .HasIndex(o => o.ObservationTime);

        // Seed initial data
        SeedInitialData(modelBuilder);
    }

    private static void SeedInitialData(ModelBuilder modelBuilder)
    {
        var now = DateTime.UtcNow;

        // Seed Patients
        var patients = new[]
        {
            new FhirPatientEntity
            {
                Id = "P001",
                Mrn = "P001",
                GivenName = "John",
                FamilyName = "Doe",
                BirthDate = new DateTime(1965, 3, 15),
                Gender = "male",
                CreatedAt = now,
                UpdatedAt = now
            },
            new FhirPatientEntity
            {
                Id = "P002",
                Mrn = "P002",
                GivenName = "Jane",
                FamilyName = "Smith",
                BirthDate = new DateTime(1972, 8, 22),
                Gender = "female",
                CreatedAt = now,
                UpdatedAt = now
            },
            new FhirPatientEntity
            {
                Id = "P003",
                Mrn = "P003",
                GivenName = "Robert",
                FamilyName = "Johnson",
                BirthDate = new DateTime(1958, 11, 30),
                Gender = "male",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        modelBuilder.Entity<FhirPatientEntity>().HasData(patients);

        // Seed Devices
        var devices = new[]
        {
            new FhirDeviceEntity
            {
                Id = "Device-001",
                DeviceId = "Device-001",
                Manufacturer = "Medical Device",
                Model = "Dialog+",
                SerialNumber = "DG001",
                Status = "active",
                AssignedPatientId = "P001",
                CreatedAt = now,
                UpdatedAt = now
            },
            new FhirDeviceEntity
            {
                Id = "Device-002",
                DeviceId = "Device-002",
                Manufacturer = "Medical Device",
                Model = "Dialog iQ",
                SerialNumber = "DQ002",
                Status = "active",
                AssignedPatientId = "P002",
                CreatedAt = now,
                UpdatedAt = now
            },
            new FhirDeviceEntity
            {
                Id = "Device-003",
                DeviceId = "Device-003",
                Manufacturer = "Medical Device",
                Model = "Dialog+",
                SerialNumber = "DG003",
                Status = "active",
                AssignedPatientId = "P003",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        modelBuilder.Entity<FhirDeviceEntity>().HasData(devices);
    }
}
