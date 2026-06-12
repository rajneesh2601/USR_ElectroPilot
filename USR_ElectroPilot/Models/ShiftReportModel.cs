using System;

namespace USR_ElectroPilot.Models
{
    public class ShiftReportModel
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public string OperatorUsername { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int TotalLoads { get; set; }
        public int AlarmCount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
