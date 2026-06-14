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
            using (var command = new SQLiteCommand("SELECT * FROM ProcessSteps WHERE IsActive = 1 ORDER BY StepNo;", connection))
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
                StepName = Convert.ToString(reader["StepName"]),
                DurationSeconds = Convert.ToInt32(reader["DurationSeconds"]),
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1
            };
        }
    }
}
