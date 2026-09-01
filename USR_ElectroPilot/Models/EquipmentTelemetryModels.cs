using System;

namespace USR_ElectroPilot.Models
{
    public enum TelemetryQuality
    {
        Unknown,
        Good,
        Stale,
        Bad
    }

    public enum EquipmentOperatingState
    {
        Unknown,
        Stopped,
        Running,
        Warning,
        Fault,
        Local,
        Stale,
        BadData
    }

    public abstract class EquipmentTelemetryModel
    {
        public string EquipmentId { get; set; }
        public int LineId { get; set; }
        public int? TankId { get; set; }
        public int? TankNo { get; set; }
        public string Name { get; set; }
        public bool CommandOn { get; set; }
        public bool RunFeedback { get; set; }
        public bool Fault { get; set; }
        public bool Warning { get; set; }
        public bool IsLocalMode { get; set; }
        public DateTime CommandChangedAtUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public TelemetryQuality Quality { get; set; }
    }

    public sealed class MotorTelemetryModel : EquipmentTelemetryModel
    {
        public double CurrentAmps { get; set; }
        public bool Overload { get; set; }
        public string DriveFaultCode { get; set; }
    }

    public sealed class PumpTelemetryModel : EquipmentTelemetryModel
    {
        public string MotorEquipmentId { get; set; }
        public double FlowLitersPerMinute { get; set; }
        public double PressureBar { get; set; }
        public bool DryRun { get; set; }
    }

    public sealed class ValveTelemetryModel : EquipmentTelemetryModel
    {
        public bool OpenCommand { get; set; }
        public bool OpenFeedback { get; set; }
        public bool ClosedFeedback { get; set; }
    }

    public sealed class HeaterTelemetryModel : EquipmentTelemetryModel
    {
        public double TargetTemperatureCelsius { get; set; }
        public double ActualTemperatureCelsius { get; set; }
        public bool OverTemperature { get; set; }
    }

    public sealed class RectifierTelemetryModel : EquipmentTelemetryModel
    {
        public double TargetVoltage { get; set; }
        public double ActualVoltage { get; set; }
        public double TargetCurrentAmps { get; set; }
        public double ActualCurrentAmps { get; set; }
        public string FaultCode { get; set; }
    }
}
