using System;

namespace USR_ElectroPilot.Models
{
    public class TankModel
    {
        public int Id { get; set; }
        public int LineId { get; set; }
        public int TankNo { get; set; }
        public string Name { get; set; }
        public string LineName { get; set; }
        public string ChemicalName { get; set; }
        public double CapacityLiters { get; set; }
        public double CurrentLevelLiters { get; set; }
        public double TemperatureCelsius { get; set; }
        public double Voltage { get; set; }
        public double CurrentAmps { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public int TankNumber
        {
            get { return TankNo; }
            set { TankNo = value; }
        }

        public string TankName
        {
            get { return Name; }
            set { Name = value; }
        }

        public string DisplayCode
        {
            get { return "L" + LineId + "-T" + TankNo; }
        }
    }
}
