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
        private readonly HoistStatusRepository _hoistStatusRepository = new HoistStatusRepository();

        public PlantStatusModel GetPlantStatus()
        {
            DatabaseHelper.InitializeDatabase();

            var tanks = _tankRepository.GetAll();
            var alarms = _alarmRepository.GetActive();
            var loads = _loadRepository.GetAll();
            var rectifiers = _rectifierRepository.GetAll();
            var steps = _processStepRepository.GetActive();
            var hoist = _hoistStatusRepository.GetCurrent();
            var hoistTank = tanks.FirstOrDefault(t => hoist.CurrentTankId.HasValue && t.Id == hoist.CurrentTankId.Value);
            var currentStep = steps.FirstOrDefault(s => hoist.CurrentTankId.HasValue && s.TankId == hoist.CurrentTankId.Value);

            return new PlantStatusModel
            {
                TotalTankCount = tanks.Count,
                ActiveTankCount = tanks.Count(t => t.IsActive),
                RunningTankCount = tanks.Count(t => string.Equals(t.Status, "Running", StringComparison.OrdinalIgnoreCase)),
                FaultTankCount = tanks.Count(t => string.Equals(t.Status, "Fault", StringComparison.OrdinalIgnoreCase)),
                ActiveAlarmCount = alarms.Count,
                QueuedLoadCount = loads.Count(l => string.Equals(l.Status, "Queued", StringComparison.OrdinalIgnoreCase)),
                RunningRectifierCount = rectifiers.Count(r => r.IsRunning),
                CurrentProcessStep = currentStep == null ? "Idle" : currentStep.StepName,
                HoistPosition = hoistTank == null ? "No Tank" : hoistTank.Name,
                HoistState = string.IsNullOrEmpty(hoist.Status) ? "Idle" : hoist.Status,
                UpdatedAt = DateTime.Now
            };
        }
    }
}
