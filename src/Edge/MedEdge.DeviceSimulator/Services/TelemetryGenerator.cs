namespace MedEdge.DeviceSimulator.Services;

public record TelemetrySnapshot(
    ushort BloodFlow,
    ushort ArterialPressure,
    ushort VenousPressure,
    ushort TemperatureDecimal,
    ushort ConductivityDecimal,
    ushort TreatmentTime,
    bool HasAlarm
);

public class TelemetryGenerator
{
    private readonly Random _random = new();
    private ushort _treatmentTime = 0;
    private bool _hypotensionMode = false;

    // Baseline values
    private readonly ushort _baseBloodFlow = 300;
    private readonly ushort _baseArterialPressure = 120;
    private readonly ushort _baseVenousPressure = 80;
    private readonly decimal _baseTemperature = 36.5m;
    private readonly decimal _baseConductivity = 14.0m;

    public TelemetrySnapshot GenerateTelemetry()
    {
        // Generate realistic waveforms with small variance
        var bloodFlow = (ushort)(_baseBloodFlow + _random.Next(-20, 20));
        var arterialPressure = _hypotensionMode
            ? (ushort)(_baseArterialPressure - 40 + _random.Next(-10, 10))
            : (ushort)(_baseArterialPressure + _random.Next(-5, 5));
        var venousPressure = (ushort)(_baseVenousPressure + _random.Next(-5, 5));
        var temperature = _baseTemperature + (decimal)_random.Next(-10, 10) / 100;
        var conductivity = _baseConductivity + (decimal)_random.Next(-5, 5) / 100;

        _treatmentTime += 5; // 5 seconds per generation

        // Clamp values to ranges
        bloodFlow = Math.Min(Math.Max(bloodFlow, Models.DialysisRegisterMap.BloodFlowMin), Models.DialysisRegisterMap.BloodFlowMax);
        arterialPressure = Math.Min(Math.Max(arterialPressure, Models.DialysisRegisterMap.ArterialPressureMin), Models.DialysisRegisterMap.ArterialPressureMax);
        venousPressure = Math.Min(Math.Max(venousPressure, Models.DialysisRegisterMap.VenousPressureMin), Models.DialysisRegisterMap.VenousPressureMax);
        temperature = Math.Min(Math.Max(temperature, Models.DialysisRegisterMap.TemperatureMin), Models.DialysisRegisterMap.TemperatureMax);
        conductivity = Math.Min(Math.Max(conductivity, Models.DialysisRegisterMap.ConductivityMin), Models.DialysisRegisterMap.ConductivityMax);

        // Convert to register format (multiply decimals by 100 to preserve 2 decimal places)
        var temperatureReg = (ushort)(temperature * 100);
        var conductivityReg = (ushort)(conductivity * 100);

        var hasAlarm = arterialPressure < 80 || _hypotensionMode;

        return new TelemetrySnapshot(
            bloodFlow,
            arterialPressure,
            venousPressure,
            temperatureReg,
            conductivityReg,
            _treatmentTime,
            hasAlarm
        );
    }

    public void InjectAnomalyHypotension()
    {
        _hypotensionMode = true;
    }

    public void ClearAnomalies()
    {
        _hypotensionMode = false;
    }

    public void Reset()
    {
        _treatmentTime = 0;
        _hypotensionMode = false;
    }
}
