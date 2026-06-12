using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class ShiftReportService
    {
        private readonly ShiftReportRepository _shiftReportRepository = new ShiftReportRepository();

        public List<ShiftReportModel> GetShiftReports()
        {
            DatabaseHelper.InitializeDatabase();
            return _shiftReportRepository.GetAll();
        }

        public int AddShiftReport(ShiftReportModel report)
        {
            DatabaseHelper.InitializeDatabase();
            return _shiftReportRepository.Add(report);
        }
    }
}
