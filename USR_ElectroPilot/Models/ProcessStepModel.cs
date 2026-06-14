namespace USR_ElectroPilot.Models
{
    public class ProcessStepModel
    {
        public int StepId { get; set; }
        public int StepNo { get; set; }
        public int TankId { get; set; }
        public string StepName { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsActive { get; set; }
    }
}
