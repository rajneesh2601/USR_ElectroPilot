using System;
using System.Linq;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class DashboardService
    {
        private readonly TankRepository _tankRepository = new TankRepository();
        private readonly AlarmRepository _alarmRepository = new AlarmRepository();
        private readonly LoadRepository _loadRepository = new LoadRepository();
        private readonly RectifierRepository _rectifierRepository = new RectifierRepository();
        private readonly ProcessStepRepository _processStepRepository = new ProcessStepRepository();
        private readonly HoistRepository _hoistRepository = new HoistRepository();
        private readonly JobRepository _jobRepository = new JobRepository();

        public PlantStatusModel GetPlantStatus()
        {
            DatabaseHelper.InitializeDatabase();

            var tanks = _tankRepository.GetAll();
            var alarms = _alarmRepository.GetActive();
            var loads = _loadRepository.GetAll();
            var rectifiers = _rectifierRepository.GetAll();
            var steps = _processStepRepository.GetActive();
            var hoists = _hoistRepository.GetAll();
            var jobs = _jobRepository.GetActive();
            var activeJob = jobs.FirstOrDefault();
            var activeHoist = hoists.FirstOrDefault(h => activeJob != null && h.CurrentJobId == activeJob.JobId) ?? hoists.FirstOrDefault();
            var currentStep = activeJob == null ? null : steps.FirstOrDefault(s => s.StepNo == activeJob.CurrentStep);

            return new PlantStatusModel
            {
                TotalTankCount = tanks.Count,
                ActiveTankCount = tanks.Count(t => t.IsActive),
                RunningTankCount = tanks.Count(t => string.Equals(t.Status, "Running", StringComparison.OrdinalIgnoreCase)),
                FaultTankCount = tanks.Count(t => string.Equals(t.Status, "Fault", StringComparison.OrdinalIgnoreCase)),
                ActiveAlarmCount = alarms.Count,
                QueuedLoadCount = loads.Count(l => string.Equals(l.Status, "Queued", StringComparison.OrdinalIgnoreCase)),
                RunningRectifierCount = rectifiers.Count(r => r.IsRunning),
                CurrentProcessStep = currentStep == null ? "Idle" : currentStep.ProcessName,
                HoistPosition = activeHoist == null ? "No Hoist" : activeHoist.HoistName + " T" + activeHoist.CurrentTankNo,
                HoistState = activeHoist == null || string.IsNullOrEmpty(activeHoist.Status) ? "Idle" : activeHoist.Status,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
