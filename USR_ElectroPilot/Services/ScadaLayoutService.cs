using System;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class ScadaLayoutService
    {
        private const string TankRowsKey = "ScadaTankRows";
        private readonly SystemSettingRepository _systemSettingRepository = new SystemSettingRepository();

        public int GetTankRows()
        {
            DatabaseHelper.InitializeDatabase();

            var value = _systemSettingRepository.GetValue(TankRowsKey);
            int rows;
            if (!int.TryParse(value, out rows))
            {
                return 1;
            }

            return ClampRows(rows);
        }

        public int AddTankRow()
        {
            var rows = ClampRows(GetTankRows() + 1);
            SaveTankRows(rows);
            return rows;
        }

        private void SaveTankRows(int rows)
        {
            DatabaseHelper.InitializeDatabase();
            _systemSettingRepository.Upsert(new SystemSettingModel
            {
                SettingKey = TankRowsKey,
                SettingValue = rows.ToString(),
                Description = "Number of horizontal SCADA tank process lines."
            });
        }

        private static int ClampRows(int rows)
        {
            return Math.Max(1, Math.Min(4, rows));
        }
    }
}
