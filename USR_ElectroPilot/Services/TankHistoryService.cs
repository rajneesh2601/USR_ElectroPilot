using System;
using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class TankHistoryService
    {
        private readonly TankHistoryRepository _tankHistoryRepository = new TankHistoryRepository();

        public void RecordSnapshot(TankModel tank)
        {
            if (tank == null)
            {
                return;
            }

            DatabaseHelper.InitializeDatabase();
            _tankHistoryRepository.Add(new TankHistoryModel
            {
                TankId = tank.Id,
                LevelLiters = tank.CurrentLevelLiters,
                TemperatureCelsius = tank.TemperatureCelsius,
                Voltage = tank.Voltage,
                CurrentAmps = tank.CurrentAmps,
                Status = tank.Status,
                RecordedAt = DateTime.Now
            });
        }

        public List<TankHistoryModel> GetHistory(int tankId, DateTime from, DateTime to)
        {
            DatabaseHelper.InitializeDatabase();
            return _tankHistoryRepository.GetForTank(tankId, from, to);
        }
    }
}
