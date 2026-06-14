using System;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class HoistStatusService
    {
        private readonly HoistStatusRepository _hoistStatusRepository = new HoistStatusRepository();

        public HoistStatusModel GetCurrent()
        {
            DatabaseHelper.InitializeDatabase();
            return _hoistStatusRepository.GetCurrent();
        }

        public void Save(int? currentTankId, string status)
        {
            DatabaseHelper.InitializeDatabase();
            _hoistStatusRepository.Save(new HoistStatusModel
            {
                CurrentTankId = currentTankId,
                Status = status,
                LastUpdated = DateTime.Now
            });
        }
    }
}
