using System;
using System.Collections.Generic;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class SimulatorService
    {
        public PlantTelemetrySnapshot CreateEquipmentSnapshot(IList<TankModel> tanks, long sequence, DateTime capturedAtUtc)
        {
            var motors = new List<MotorTelemetryModel>();
            var pumps = new List<PumpTelemetryModel>();
            var valves = new List<ValveTelemetryModel>();
            var heaters = new List<HeaterTelemetryModel>();
            var rectifiers = new List<RectifierTelemetryModel>();

            foreach (var tank in tanks ?? new TankModel[0])
            {
                if (!tank.IsActive)
                {
                    continue;
                }

                var running = string.Equals(tank.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase);
                var fault = string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase);
                var warning = string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase);
                var station = "T" + tank.TankNo.ToString("00");

                motors.Add(new MotorTelemetryModel
                {
                    EquipmentId = "M-" + station + "-P01",
                    LineId = tank.LineId,
                    TankId = tank.Id,
                    TankNo = tank.TankNo,
                    Name = station + " circulation motor",
                    CommandOn = running,
                    RunFeedback = running && !fault,
                    Fault = fault,
                    Warning = warning,
                    CurrentAmps = running ? Math.Max(0.5, tank.CurrentAmps * 0.035) : 0,
                    CommandChangedAtUtc = capturedAtUtc,
                    LastUpdatedUtc = capturedAtUtc,
                    Quality = TelemetryQuality.Good
                });
                pumps.Add(new PumpTelemetryModel
                {
                    EquipmentId = "P-" + station + "-01",
                    MotorEquipmentId = "M-" + station + "-P01",
                    LineId = tank.LineId,
                    TankId = tank.Id,
                    TankNo = tank.TankNo,
                    Name = station + " circulation pump",
                    CommandOn = running,
                    RunFeedback = running && !fault,
                    Fault = fault,
                    Warning = warning,
                    LastUpdatedUtc = capturedAtUtc,
                    Quality = TelemetryQuality.Good
                });
                valves.Add(new ValveTelemetryModel
                {
                    EquipmentId = "V-" + station + "-01",
                    LineId = tank.LineId,
                    TankId = tank.Id,
                    TankNo = tank.TankNo,
                    Name = station + " circulation valve",
                    CommandOn = running,
                    RunFeedback = running && !fault,
                    OpenCommand = running,
                    OpenFeedback = running && !fault,
                    ClosedFeedback = !running && !fault,
                    Fault = fault,
                    Warning = warning,
                    LastUpdatedUtc = capturedAtUtc,
                    Quality = TelemetryQuality.Good
                });
                heaters.Add(new HeaterTelemetryModel
                {
                    EquipmentId = "H-" + station + "-01",
                    LineId = tank.LineId,
                    TankId = tank.Id,
                    TankNo = tank.TankNo,
                    Name = station + " heater",
                    ActualTemperatureCelsius = tank.TemperatureCelsius,
                    Fault = fault,
                    Warning = warning,
                    LastUpdatedUtc = capturedAtUtc,
                    Quality = TelemetryQuality.Good
                });

                if (tank.Voltage > 0 || tank.CurrentAmps > 0)
                {
                    rectifiers.Add(new RectifierTelemetryModel
                    {
                        EquipmentId = "R-" + station + "-01",
                        LineId = tank.LineId,
                        TankId = tank.Id,
                        TankNo = tank.TankNo,
                        Name = station + " rectifier",
                        CommandOn = running,
                        RunFeedback = running && !fault,
                        Fault = fault,
                        Warning = warning,
                        ActualVoltage = tank.Voltage,
                        ActualCurrentAmps = tank.CurrentAmps,
                        LastUpdatedUtc = capturedAtUtc,
                        Quality = TelemetryQuality.Good
                    });
                }
            }

            return new PlantTelemetrySnapshot(sequence, capturedAtUtc, motors, pumps, valves, heaters, rectifiers);
        }

        public void SimulateTanks(IList<TankModel> tanks)
        {
            if (tanks == null)
            {
                return;
            }

            foreach (var tank in tanks)
            {
                if (!tank.IsActive || string.Equals(tank.Status, Constants.StatusFault, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.Equals(tank.Status, Constants.StatusRunning, System.StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(tank.Status, Constants.StatusWarning, System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                tank.CurrentLevelLiters = Clamp(tank.CurrentLevelLiters + RandomGenerator.NextDouble(-5, 5), 0, tank.CapacityLiters);
                tank.TemperatureCelsius = Clamp(tank.TemperatureCelsius + RandomGenerator.NextDouble(-0.3, 0.3), 15, 80);
                tank.Voltage = Clamp(tank.Voltage + RandomGenerator.NextDouble(-0.2, 0.2), 0, 24);
                tank.CurrentAmps = Clamp(tank.CurrentAmps + RandomGenerator.NextDouble(-2, 2), 0, 500);
                if (RandomGenerator.Chance(0.01))
                {
                    tank.Status = Constants.StatusFault;
                }
                else if (tank.TemperatureCelsius > 70 || tank.CurrentLevelLiters < tank.CapacityLiters * 0.45)
                {
                    tank.Status = Constants.StatusWarning;
                }
                else
                {
                    tank.Status = Constants.StatusRunning;
                }
            }
        }

        public void SimulateRectifiers(IList<RectifierModel> rectifiers)
        {
            if (rectifiers == null)
            {
                return;
            }

            foreach (var rectifier in rectifiers)
            {
                rectifier.Voltage = Clamp(rectifier.Voltage + RandomGenerator.NextDouble(-0.5, 0.5), 0, 24);
                rectifier.CurrentAmps = Clamp(rectifier.CurrentAmps + RandomGenerator.NextDouble(-5, 5), 0, 500);
                rectifier.FaultCode = RandomGenerator.Chance(0.01) ? "FLT" : null;
            }
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}
