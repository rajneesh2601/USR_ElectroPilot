using System;

namespace USR_ElectroPilot.Models
{
    public class JobModel
    {
        public int JobId { get; set; }
        public string JobNumber { get; set; }
        public int LineId { get; set; }
        public int CurrentStep { get; set; }
        public int StepDirection { get; set; }
        public int CurrentTank { get; set; }
        public string Status { get; set; }
        public int RemainingSeconds { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
