using System;

namespace USR_ElectroPilot.Models
{
    public class TankHistoryModel
    {
        public int Id { get; set; }
        public int TankId { get; set; }
        public double LevelLiters { get; set; }
        public double TemperatureCelsius { get; set; }
        public double Voltage { get; set; }
        public double CurrentAmps { get; set; }
        public string Status { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
