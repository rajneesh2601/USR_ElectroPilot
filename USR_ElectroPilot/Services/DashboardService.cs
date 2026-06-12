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

        public PlantStatusModel GetPlantStatus()
        {
            DatabaseHelper.InitializeDatabase();

            var tanks = _tankRepository.GetAll();
            var alarms = _alarmRepository.GetActive();
            var loads = _loadRepository.GetAll();
            var rectifiers = _rectifierRepository.GetAll();

            return new PlantStatusModel
            {
                ActiveTankCount = tanks.Count(t => t.IsActive),
                ActiveAlarmCount = alarms.Count,
                QueuedLoadCount = loads.Count(l => string.Equals(l.Status, "Queued", StringComparison.OrdinalIgnoreCase)),
                RunningRectifierCount = rectifiers.Count(r => r.IsRunning),
                UpdatedAt = DateTime.Now
            };
        }
    }
}
