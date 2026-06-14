using System;

namespace USR_ElectroPilot.Models
{
    public class PlantStatusModel
    {
        public int TotalTankCount { get; set; }
        public int ActiveTankCount { get; set; }
        public int RunningTankCount { get; set; }
        public int FaultTankCount { get; set; }
        public int ActiveAlarmCount { get; set; }
        public int RunningRectifierCount { get; set; }
        public int QueuedLoadCount { get; set; }
        public string CurrentProcessStep { get; set; }
        public string HoistPosition { get; set; }
        public string HoistState { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
