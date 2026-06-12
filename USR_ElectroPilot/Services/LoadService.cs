using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class LoadService
    {
        private readonly LoadRepository _loadRepository = new LoadRepository();

        public List<LoadModel> GetLoads()
        {
            DatabaseHelper.InitializeDatabase();
            return _loadRepository.GetAll();
        }

        public int AddLoad(LoadModel load)
        {
            DatabaseHelper.InitializeDatabase();
            return _loadRepository.Add(load);
        }
    }
}
