using System;

namespace USR_ElectroPilot.Models
{
    public class WagonModel
    {
        public int Id { get; set; }
        public string WagonCode { get; set; }
        public string State { get; set; }
        public int? CurrentTankId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
