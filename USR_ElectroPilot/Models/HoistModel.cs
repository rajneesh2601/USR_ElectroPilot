namespace USR_ElectroPilot.Models
{
    public class HoistModel
    {
        public int HoistId { get; set; }
        public int LineId { get; set; }
        public string LineName { get; set; }
        public string HoistName { get; set; }
        public int CurrentTankNo { get; set; }
        public int TargetTankNo { get; set; }
        public string Direction { get; set; }
        public int HomeTank { get; set; }
        public int FromTank { get; set; }
        public int ToTank { get; set; }
        public string Status { get; set; }
        public int? CurrentJobId { get; set; }
        public bool IsAuto { get; set; }
        public double PositionIndex { get; set; }
        public int StateTicks { get; set; }

        public int LineNo
        {
            get { return LineId; }
            set { LineId = value; }
        }
    }
}
