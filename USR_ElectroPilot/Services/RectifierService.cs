using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class RectifierService
    {
        private readonly RectifierRepository _rectifierRepository = new RectifierRepository();

        public List<RectifierModel> GetRectifiers()
        {
            DatabaseHelper.InitializeDatabase();
            return _rectifierRepository.GetAll();
        }

        public int AddRectifier(RectifierModel rectifier)
        {
            DatabaseHelper.InitializeDatabase();
            return _rectifierRepository.Add(rectifier);
        }
    }
}
