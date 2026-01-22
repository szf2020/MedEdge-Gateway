namespace MedEdge.DeviceSimulator.Models;

public class DialysisRegisterMap
{
    // Holding Register Addresses (40001-40012 in Modbus spec maps to 0-11 in code)
    public const ushort BloodFlowRateRegister = 0;          // 40001
    public const ushort ArterialPressureRegister = 2;       // 40003
    public const ushort VenousPressureRegister = 4;         // 40005
    public const ushort DialysateTemperatureRegister = 6;   // 40007
    public const ushort ConductivityRegister = 8;           // 40009
    public const ushort TreatmentTimeRegister = 10;         // 40011
    public const ushort StopFlagRegister = 100;             // 40101

    // Discrete Input Addresses (10001-10016 in Modbus spec maps to 0-15 in code)
    public const ushort AlarmRegister = 0;

    // Value ranges
    public const ushort BloodFlowMin = 200;
    public const ushort BloodFlowMax = 400;
    public const ushort ArterialPressureMin = 50;
    public const ushort ArterialPressureMax = 200;
    public const ushort VenousPressureMin = 50;
    public const ushort VenousPressureMax = 200;
    public const decimal TemperatureMin = 35.0m;
    public const decimal TemperatureMax = 38.0m;
    public const decimal ConductivityMin = 13.5m;
    public const decimal ConductivityMax = 14.5m;
}
