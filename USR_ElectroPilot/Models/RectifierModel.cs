using System;

namespace USR_ElectroPilot.Models
{
    public class RectifierModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Voltage { get; set; }
        public double CurrentAmps { get; set; }
        public bool IsRunning { get; set; }
        public string FaultCode { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
