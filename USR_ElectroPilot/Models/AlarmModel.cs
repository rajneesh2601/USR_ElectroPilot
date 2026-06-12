using System;

namespace USR_ElectroPilot.Models
{
    public class AlarmModel
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
        public DateTime RaisedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? ShelvedUntil { get; set; }
    }
}
