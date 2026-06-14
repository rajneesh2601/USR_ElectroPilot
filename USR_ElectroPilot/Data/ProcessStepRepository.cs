using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class ProcessStepRepository
    {
        public List<ProcessStepModel> GetActive()
        {
            var steps = new List<ProcessStepModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"SELECT ps.*, t.TankNumber AS ResolvedTankNo
                  FROM ProcessSteps ps
                  LEFT JOIN Tanks t ON t.Id = ps.TankId
                  WHERE ps.IsActive = 1
                  ORDER BY ps.StepNo;",
                connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        steps.Add(MapStep(reader));
                    }
                }
            }

            return steps;
        }

        private static ProcessStepModel MapStep(SQLiteDataReader reader)
        {
            return new ProcessStepModel
            {
                StepId = Convert.ToInt32(reader["StepId"]),
                StepNo = Convert.ToInt32(reader["StepNo"]),
                TankId = Convert.ToInt32(reader["TankId"]),
                TankNo = ReadInt(reader, "TankNo", "ResolvedTankNo"),
                StepName = Convert.ToString(reader["StepName"]),
                ProcessName = ReadString(reader, "ProcessName", "StepName"),
                DurationSeconds = Convert.ToInt32(reader["DurationSeconds"]),
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1
            };
        }

        private static int ReadInt(SQLiteDataReader reader, string preferredName, string fallbackName)
        {
            var value = reader[preferredName] == DBNull.Value ? reader[fallbackName] : reader[preferredName];
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static string ReadString(SQLiteDataReader reader, string preferredName, string fallbackName)
        {
            var value = reader[preferredName] == DBNull.Value ? reader[fallbackName] : reader[preferredName];
            return value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }
    }
}
