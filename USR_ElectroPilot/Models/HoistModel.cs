namespace USR_ElectroPilot.Models
{
    public class HoistModel
    {
        public int HoistId { get; set; }
        public string HoistName { get; set; }
        public int CurrentTankNo { get; set; }
        public int TargetTankNo { get; set; }
        public string Status { get; set; }
        public int? CurrentJobId { get; set; }
        public int LineNo { get; set; }
        public bool IsAuto { get; set; }
        public double PositionIndex { get; set; }
        public int StateTicks { get; set; }
    }
}
