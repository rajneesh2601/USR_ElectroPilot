using System;
using System.Collections.Generic;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public sealed class EquipmentTelemetryAlarmService
    {
        public IList<AlarmModel> Evaluate(
            PlantTelemetrySnapshot snapshot,
            DateTime utcNow,
            TimeSpan staleAfter,
            TimeSpan startFeedbackTimeout)
        {
            var alarms = new Dictionary<string, AlarmModel>(StringComparer.OrdinalIgnoreCase);
            var health = PlantTelemetryHealthResolver.Resolve(snapshot, utcNow, staleAfter);
            if (health.Quality == TelemetryQuality.Stale || health.Quality == TelemetryQuality.Bad)
            {
                Add(alarms, "PLC/Telemetry", "Critical", health.Quality == TelemetryQuality.Bad
                    ? "Plant telemetry data is invalid"
                    : "Plant telemetry heartbeat is stale", utcNow);
                return new List<AlarmModel>(alarms.Values);
            }

            if (snapshot == null)
            {
                return new List<AlarmModel>();
            }

            foreach (var motor in snapshot.Motors)
            {
                var state = EquipmentTelemetryStateResolver.Resolve(motor, utcNow, staleAfter);
                if (state == EquipmentOperatingState.Stale || state == EquipmentOperatingState.BadData)
                {
                    Add(alarms, motor.EquipmentId, "Major", "Motor feedback data is " + state.ToString().ToLowerInvariant(), utcNow);
                    continue;
                }

                if (motor.Fault || motor.Overload || !string.IsNullOrWhiteSpace(motor.DriveFaultCode))
                {
                    Add(alarms, motor.EquipmentId, "Critical", motor.Overload ? "Motor overload/trip active" : "Motor drive fault active", utcNow);
                }

                if (motor.IsLocalMode)
                {
                    Add(alarms, motor.EquipmentId, "Major", "Motor is in local mode", utcNow);
                }

                if (motor.CommandOn && !motor.RunFeedback &&
                    motor.CommandChangedAtUtc != DateTime.MinValue &&
                    utcNow - motor.CommandChangedAtUtc >= startFeedbackTimeout)
                {
                    Add(alarms, motor.EquipmentId, "Major", "Motor failed to start after run command", utcNow);
                }
                else if (!motor.CommandOn && motor.RunFeedback)
                {
                    Add(alarms, motor.EquipmentId, "Major", "Motor run feedback remains active after stop command", utcNow);
                }
            }

            return new List<AlarmModel>(alarms.Values);
        }

        private static void Add(IDictionary<string, AlarmModel> alarms, string source, string severity, string message, DateTime raisedAt)
        {
            var key = source + "|" + message;
            if (alarms.ContainsKey(key))
            {
                return;
            }

            alarms[key] = new AlarmModel
            {
                Source = source,
                Severity = severity,
                Message = message,
                State = Constants.AlarmActive,
                RaisedAt = raisedAt
            };
        }
    }
}
