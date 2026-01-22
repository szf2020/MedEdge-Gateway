using MedEdge.AiEngine.Models;
using Microsoft.Extensions.Logging;

namespace MedEdge.AiEngine.Services;

public interface IAnomalyDetector
{
    AnomalyResult? Analyze(Dictionary<string, double> measurements, Dictionary<string, bool> alarms);
}

public class StatisticalAnomalyDetector : IAnomalyDetector
{
    private readonly ILogger<StatisticalAnomalyDetector> _logger;

    // Clinical thresholds
    private const ushort BloodFlowCritical = 150;
    private const ushort ArterialPressureWarning = 80;
    private const ushort VenousPressureCritical = 250;
    private const decimal TemperatureWarning = 38.5m;
    private const decimal ConductivityMin = 13.0m;
    private const decimal ConductivityMax = 15.0m;

    public StatisticalAnomalyDetector(ILogger<StatisticalAnomalyDetector> logger)
    {
        _logger = logger;
    }

    public AnomalyResult? Analyze(Dictionary<string, double> measurements, Dictionary<string, bool> alarms)
    {
        // Check critical thresholds
        if (measurements.TryGetValue("bloodFlow", out var bloodFlow))
        {
            if (bloodFlow < BloodFlowCritical)
            {
                return new AnomalyResult(
                    RiskLevel.Critical,
                    $"Blood flow critically low: {bloodFlow} mL/min (critical <{BloodFlowCritical})",
                    "Check vascular access, possible recirculation or access malfunction",
                    DateTime.UtcNow
                );
            }
        }

        if (measurements.TryGetValue("arterialPressure", out var apPressure))
        {
            if (apPressure < ArterialPressureWarning)
            {
                return new AnomalyResult(
                    RiskLevel.Critical,
                    $"Hypotension detected: AP {apPressure} mmHg (critical <{ArterialPressureWarning})",
                    "Immediate intervention required: reduce ultrafiltration, check patient status",
                    DateTime.UtcNow
                );
            }
        }

        if (measurements.TryGetValue("venousPressure", out var vpPressure))
        {
            if (vpPressure > VenousPressureCritical)
            {
                return new AnomalyResult(
                    RiskLevel.Critical,
                    $"Venous pressure elevated: VP {vpPressure} mmHg (critical >{VenousPressureCritical})",
                    "Check for venous needle malposition or venous line occlusion",
                    DateTime.UtcNow
                );
            }
        }

        if (measurements.TryGetValue("dialysateTemperature", out var temp))
        {
            if (temp > (double)TemperatureWarning)
            {
                return new AnomalyResult(
                    RiskLevel.High,
                    $"Temperature elevated: {temp:F1}°C (warning >{TemperatureWarning})",
                    "Check heater calibration and dialysate preparation",
                    DateTime.UtcNow
                );
            }
        }

        if (measurements.TryGetValue("conductivity", out var conductivity))
        {
            if (conductivity < (double)ConductivityMin || conductivity > (double)ConductivityMax)
            {
                return new AnomalyResult(
                    RiskLevel.High,
                    $"Conductivity out of range: {conductivity:F2} mS/cm (normal {ConductivityMin}-{ConductivityMax})",
                    "Check dialysate composition and conductivity probe",
                    DateTime.UtcNow
                );
            }
        }

        // Check alarm flags
        if (alarms.TryGetValue("pressureLow", out var lowPressure) && lowPressure)
        {
            return new AnomalyResult(
                RiskLevel.High,
                "Low pressure alarm triggered",
                "Verify patient hemodynamic status",
                DateTime.UtcNow
            );
        }

        if (alarms.TryGetValue("pressureHigh", out var highPressure) && highPressure)
        {
            return new AnomalyResult(
                RiskLevel.High,
                "High venous pressure alarm triggered",
                "Check venous line integrity and position",
                DateTime.UtcNow
            );
        }

        return null;
    }
}
