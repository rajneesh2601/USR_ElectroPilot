using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace USR_ElectroPilot.Models
{
    public sealed class PlantTelemetrySnapshot
    {
        public PlantTelemetrySnapshot(
            long sequence,
            DateTime capturedAtUtc,
            IEnumerable<MotorTelemetryModel> motors,
            IEnumerable<PumpTelemetryModel> pumps,
            IEnumerable<ValveTelemetryModel> valves,
            IEnumerable<HeaterTelemetryModel> heaters,
            IEnumerable<RectifierTelemetryModel> rectifiers)
        {
            Sequence = sequence;
            CapturedAtUtc = capturedAtUtc;
            Motors = ToReadOnly(motors);
            Pumps = ToReadOnly(pumps);
            Valves = ToReadOnly(valves);
            Heaters = ToReadOnly(heaters);
            Rectifiers = ToReadOnly(rectifiers);
        }

        public long Sequence { get; private set; }
        public DateTime CapturedAtUtc { get; private set; }
        public ReadOnlyCollection<MotorTelemetryModel> Motors { get; private set; }
        public ReadOnlyCollection<PumpTelemetryModel> Pumps { get; private set; }
        public ReadOnlyCollection<ValveTelemetryModel> Valves { get; private set; }
        public ReadOnlyCollection<HeaterTelemetryModel> Heaters { get; private set; }
        public ReadOnlyCollection<RectifierTelemetryModel> Rectifiers { get; private set; }

        public static PlantTelemetrySnapshot Empty
        {
            get
            {
                return new PlantTelemetrySnapshot(
                    0,
                    DateTime.MinValue,
                    null,
                    null,
                    null,
                    null,
                    null);
            }
        }

        private static ReadOnlyCollection<T> ToReadOnly<T>(IEnumerable<T> values)
        {
            return new List<T>(values ?? new T[0]).AsReadOnly();
        }
    }

    public sealed class PlantTelemetryHealthModel
    {
        public TelemetryQuality Quality { get; set; }
        public TimeSpan Age { get; set; }
        public long Sequence { get; set; }
    }
}
