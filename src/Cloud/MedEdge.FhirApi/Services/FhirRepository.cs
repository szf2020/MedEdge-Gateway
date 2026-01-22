using MedEdge.Core.Domain.Entities;
using MedEdge.Core.DTOs;
using MedEdge.FhirApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MedEdge.FhirApi.Services;

public interface IFhirRepository
{
    Task<FhirPatientEntity?> GetPatientByIdAsync(string id);
    Task<List<FhirPatientEntity>> GetAllPatientsAsync();
    Task<FhirPatientEntity?> GetPatientByMrnAsync(string mrn);
    Task<FhirPatientEntity> CreatePatientAsync(CreatePatientRequest request);
    Task<FhirDeviceEntity?> GetDeviceByIdAsync(string id);
    Task<List<FhirDeviceEntity>> GetAllDevicesAsync();
    Task<FhirDeviceEntity> CreateDeviceAsync(CreateDeviceRequest request);
    Task<FhirObservationEntity?> GetObservationByIdAsync(string id);
    Task<List<FhirObservationEntity>> GetAllObservationsAsync();
    Task<List<FhirObservationEntity>> GetObservationsByPatientAsync(string patientId);
    Task<List<FhirObservationEntity>> GetObservationsByDeviceAsync(string deviceId);
    Task<List<FhirObservationEntity>> GetObservationsByCodeAsync(string code);
    Task<FhirObservationEntity> CreateObservationAsync(CreateObservationRequest request);
    Task SaveChangesAsync();
}

public class FhirRepository : IFhirRepository
{
    private readonly ApplicationDbContext _context;

    public FhirRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FhirPatientEntity?> GetPatientByIdAsync(string id)
    {
        return await _context.Patients
            .Include(p => p.Observations)
            .Include(p => p.AssignedDevices)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<FhirPatientEntity>> GetAllPatientsAsync()
    {
        return await _context.Patients
            .Include(p => p.Observations)
            .Include(p => p.AssignedDevices)
            .ToListAsync();
    }

    public async Task<FhirPatientEntity?> GetPatientByMrnAsync(string mrn)
    {
        return await _context.Patients
            .Include(p => p.Observations)
            .Include(p => p.AssignedDevices)
            .FirstOrDefaultAsync(p => p.Mrn == mrn);
    }

    public async Task<FhirPatientEntity> CreatePatientAsync(CreatePatientRequest request)
    {
        var patient = new FhirPatientEntity
        {
            Id = Guid.NewGuid().ToString(),
            Mrn = request.Mrn,
            GivenName = request.GivenName,
            FamilyName = request.FamilyName,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<FhirDeviceEntity?> GetDeviceByIdAsync(string id)
    {
        return await _context.Devices
            .Include(d => d.Observations)
            .Include(d => d.AssignedPatient)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<FhirDeviceEntity>> GetAllDevicesAsync()
    {
        return await _context.Devices
            .Include(d => d.Observations)
            .Include(d => d.AssignedPatient)
            .ToListAsync();
    }

    public async Task<FhirDeviceEntity> CreateDeviceAsync(CreateDeviceRequest request)
    {
        var device = new FhirDeviceEntity
        {
            Id = Guid.NewGuid().ToString(),
            DeviceId = request.DeviceId,
            Manufacturer = request.Manufacturer,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        return device;
    }

    public async Task<FhirObservationEntity?> GetObservationByIdAsync(string id)
    {
        return await _context.Observations
            .Include(o => o.Patient)
            .Include(o => o.Device)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<FhirObservationEntity>> GetAllObservationsAsync()
    {
        return await _context.Observations
            .Include(o => o.Patient)
            .Include(o => o.Device)
            .OrderByDescending(o => o.ObservationTime)
            .ToListAsync();
    }

    public async Task<List<FhirObservationEntity>> GetObservationsByPatientAsync(string patientId)
    {
        return await _context.Observations
            .Where(o => o.PatientId == patientId)
            .Include(o => o.Patient)
            .Include(o => o.Device)
            .OrderByDescending(o => o.ObservationTime)
            .ToListAsync();
    }

    public async Task<List<FhirObservationEntity>> GetObservationsByDeviceAsync(string deviceId)
    {
        return await _context.Observations
            .Where(o => o.DeviceId == deviceId)
            .Include(o => o.Patient)
            .Include(o => o.Device)
            .OrderByDescending(o => o.ObservationTime)
            .ToListAsync();
    }

    public async Task<List<FhirObservationEntity>> GetObservationsByCodeAsync(string code)
    {
        return await _context.Observations
            .Where(o => o.Code == code)
            .Include(o => o.Patient)
            .Include(o => o.Device)
            .OrderByDescending(o => o.ObservationTime)
            .ToListAsync();
    }

    public async Task<FhirObservationEntity> CreateObservationAsync(CreateObservationRequest request)
    {
        var observation = new FhirObservationEntity
        {
            Id = Guid.NewGuid().ToString(),
            PatientId = request.PatientId,
            DeviceId = request.DeviceId,
            Code = request.Code,
            CodeDisplay = request.CodeDisplay,
            Value = request.Value,
            Unit = request.Unit,
            Status = "final",
            ObservationTime = request.ObservationTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Observations.Add(observation);
        await _context.SaveChangesAsync();
        return observation;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
