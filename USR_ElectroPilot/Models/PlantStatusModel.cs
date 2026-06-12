using System;

namespace USR_ElectroPilot.Models
{
    public class PlantStatusModel
    {
        public int ActiveTankCount { get; set; }
        public int ActiveAlarmCount { get; set; }
        public int RunningRectifierCount { get; set; }
        public int QueuedLoadCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
