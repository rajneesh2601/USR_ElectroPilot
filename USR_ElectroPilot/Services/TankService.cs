using System.Collections.Generic;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class TankService
    {
        private readonly TankRepository _tankRepository = new TankRepository();

        public List<TankModel> GetTanks()
        {
            DatabaseHelper.InitializeDatabase();
            return _tankRepository.GetAll();
        }

        public int AddTank(TankModel tank)
        {
            DatabaseHelper.InitializeDatabase();
            return _tankRepository.Add(tank);
        }

        public void UpdateTank(TankModel tank)
        {
            _tankRepository.Update(tank);
        }

        public void DeleteTank(int id)
        {
            _tankRepository.Delete(id);
        }

        public void StartTank(TankModel tank)
        {
            SetStatus(tank, Constants.StatusRunning, true);
        }

        public void StopTank(TankModel tank)
        {
            SetStatus(tank, Constants.StatusNormal, true);
        }

        public void MarkTankFault(TankModel tank)
        {
            SetStatus(tank, Constants.StatusFault, true);
        }

        public void ResetTank(TankModel tank)
        {
            SetStatus(tank, Constants.StatusNormal, true);
        }

        public void StartAllTanks()
        {
            foreach (var tank in GetTanks())
            {
                StartTank(tank);
            }
        }

        public void StopAllTanks()
        {
            foreach (var tank in GetTanks())
            {
                StopTank(tank);
            }
        }

        private void SetStatus(TankModel tank, string status, bool isActive)
        {
            if (tank == null)
            {
                return;
            }

            tank.Status = status;
            tank.IsActive = isActive;
            UpdateTank(tank);
        }
    }
}
