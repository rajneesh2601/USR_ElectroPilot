using System;

namespace USR_ElectroPilot.Models
{
    public class HoistStatusModel
    {
        public int Id { get; set; }
        public int? CurrentTankId { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
