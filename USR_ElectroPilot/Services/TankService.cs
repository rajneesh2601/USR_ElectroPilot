using System.Collections.Generic;
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
    }
}
