using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class WagonService
    {
        private readonly WagonRepository _wagonRepository = new WagonRepository();

        public List<WagonModel> GetWagons()
        {
            DatabaseHelper.InitializeDatabase();
            return _wagonRepository.GetAll();
        }

        public int AddWagon(WagonModel wagon)
        {
            DatabaseHelper.InitializeDatabase();
            return _wagonRepository.Add(wagon);
        }
    }
}
