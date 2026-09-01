using System;
using System.Collections.Generic;
using System.Linq;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public interface IPlantTelemetryProvider
    {
        PlantTelemetrySnapshot GetLatestSnapshot();
    }

    public sealed class SimulationTelemetryProvider : IPlantTelemetryProvider
    {
        private readonly object _sync = new object();
        private PlantTelemetrySnapshot _latest = PlantTelemetrySnapshot.Empty;

        public void Publish(PlantTelemetrySnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException("snapshot");
            }

            lock (_sync)
            {
                if (snapshot.Sequence < _latest.Sequence)
                {
                    throw new InvalidOperationException("Telemetry sequence cannot move backwards.");
                }

                _latest = snapshot;
            }
        }

        public PlantTelemetrySnapshot GetLatestSnapshot()
        {
            lock (_sync)
            {
                return _latest;
            }
        }
    }

    public static class EquipmentTelemetryStateResolver
    {
        public static EquipmentOperatingState Resolve(EquipmentTelemetryModel equipment, DateTime utcNow, TimeSpan staleAfter)
        {
            if (equipment == null || equipment.Quality == TelemetryQuality.Unknown)
            {
                return EquipmentOperatingState.Unknown;
            }

            if (equipment.Quality == TelemetryQuality.Bad)
            {
                return EquipmentOperatingState.BadData;
            }

            if (equipment.Quality == TelemetryQuality.Stale ||
                equipment.LastUpdatedUtc == DateTime.MinValue ||
                utcNow - equipment.LastUpdatedUtc > staleAfter)
            {
                return EquipmentOperatingState.Stale;
            }

            if (HasConfirmedFault(equipment))
            {
                return EquipmentOperatingState.Fault;
            }

            if (equipment.IsLocalMode)
            {
                return EquipmentOperatingState.Local;
            }

            if (equipment.Warning || equipment.CommandOn != equipment.RunFeedback || HasProcessWarning(equipment))
            {
                return EquipmentOperatingState.Warning;
            }

            return equipment.RunFeedback ? EquipmentOperatingState.Running : EquipmentOperatingState.Stopped;
        }

        private static bool HasConfirmedFault(EquipmentTelemetryModel equipment)
        {
            if (equipment.Fault)
            {
                return true;
            }

            var motor = equipment as MotorTelemetryModel;
            if (motor != null && (motor.Overload || !string.IsNullOrWhiteSpace(motor.DriveFaultCode)))
            {
                return true;
            }

            var heater = equipment as HeaterTelemetryModel;
            if (heater != null && heater.OverTemperature)
            {
                return true;
            }

            var rectifier = equipment as RectifierTelemetryModel;
            return rectifier != null && !string.IsNullOrWhiteSpace(rectifier.FaultCode);
        }

        private static bool HasProcessWarning(EquipmentTelemetryModel equipment)
        {
            var pump = equipment as PumpTelemetryModel;
            if (pump != null && pump.DryRun)
            {
                return true;
            }

            var valve = equipment as ValveTelemetryModel;
            return valve != null && valve.OpenCommand && !valve.OpenFeedback;
        }
    }

    public static class PlantTelemetryHealthResolver
    {
        public static PlantTelemetryHealthModel Resolve(PlantTelemetrySnapshot snapshot, DateTime utcNow, TimeSpan staleAfter)
        {
            if (snapshot == null || snapshot.Sequence <= 0 || snapshot.CapturedAtUtc == DateTime.MinValue)
            {
                return new PlantTelemetryHealthModel { Quality = TelemetryQuality.Unknown, Sequence = 0, Age = TimeSpan.Zero };
            }

            var age = utcNow - snapshot.CapturedAtUtc;
            if (age < TimeSpan.Zero)
            {
                age = TimeSpan.Zero;
            }

            var equipment = GetEquipment(snapshot).ToList();
            var quality = equipment.Count == 0 ? TelemetryQuality.Unknown : TelemetryQuality.Good;
            if (equipment.Any(e => e.Quality == TelemetryQuality.Bad))
            {
                quality = TelemetryQuality.Bad;
            }
            else if (age > staleAfter || equipment.Any(e => e.Quality == TelemetryQuality.Stale))
            {
                quality = TelemetryQuality.Stale;
            }
            else if (equipment.Any(e => e.Quality == TelemetryQuality.Unknown))
            {
                quality = TelemetryQuality.Unknown;
            }

            return new PlantTelemetryHealthModel
            {
                Quality = quality,
                Age = age,
                Sequence = snapshot.Sequence
            };
        }

        private static IEnumerable<EquipmentTelemetryModel> GetEquipment(PlantTelemetrySnapshot snapshot)
        {
            return snapshot.Motors.Cast<EquipmentTelemetryModel>()
                .Concat(snapshot.Pumps)
                .Concat(snapshot.Valves)
                .Concat(snapshot.Heaters)
                .Concat(snapshot.Rectifiers);
        }
    }
}
