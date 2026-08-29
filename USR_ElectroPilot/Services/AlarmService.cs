using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class AlarmService
    {
        private readonly AlarmRepository _alarmRepository = new AlarmRepository();

        public List<AlarmModel> GetAlarms()
        {
            DatabaseHelper.InitializeDatabase();
            return _alarmRepository.GetAll();
        }

        public List<AlarmModel> GetActiveAlarms()
        {
            DatabaseHelper.InitializeDatabase();
            return _alarmRepository.GetActive();
        }

        public int RaiseAlarm(string source, string severity, string message)
        {
            DatabaseHelper.InitializeDatabase();

            if (_alarmRepository.HasActive(source, message))
            {
                return 0;
            }

            return _alarmRepository.Add(new AlarmModel
            {
                Source = source,
                Severity = severity,
                Message = message,
                State = Constants.AlarmActive
            });
        }

        public void AcknowledgeAlarm(int id)
        {
            _alarmRepository.Acknowledge(id, AppSession.Username);
        }

        public void AcknowledgeAlarm(int id, string username)
        {
            _alarmRepository.Acknowledge(id, username ?? string.Empty);
        }

        public void ResetActiveAlarms()
        {
            DatabaseHelper.InitializeDatabase();
            _alarmRepository.AcknowledgeActive(AppSession.Username);
        }

        public void ResetActiveAlarms(string username)
        {
            DatabaseHelper.InitializeDatabase();
            _alarmRepository.AcknowledgeActive(username ?? string.Empty);
        }
    }
}
