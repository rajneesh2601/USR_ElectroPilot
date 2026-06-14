using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class HoistRepository
    {
        public List<HoistModel> GetAll()
        {
            var hoists = new List<HoistModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Hoists ORDER BY HoistId;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hoists.Add(MapHoist(reader));
                    }
                }
            }

            return hoists;
        }

        public void Update(HoistModel hoist)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Hoists
                  SET HoistName = @HoistName,
                      CurrentTankNo = @CurrentTankNo,
                      TargetTankNo = @TargetTankNo,
                      Status = @Status,
                      CurrentJobId = @CurrentJobId,
                      LineNo = @LineNo,
                      IsAuto = @IsAuto,
                      PositionIndex = @PositionIndex,
                      StateTicks = @StateTicks
                  WHERE HoistId = @HoistId;",
                connection))
            {
                AddParameters(command, hoist);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(SQLiteCommand command, HoistModel hoist)
        {
            command.Parameters.AddWithValue("@HoistId", hoist.HoistId);
            command.Parameters.AddWithValue("@HoistName", hoist.HoistName);
            command.Parameters.AddWithValue("@CurrentTankNo", hoist.CurrentTankNo);
            command.Parameters.AddWithValue("@TargetTankNo", hoist.TargetTankNo);
            command.Parameters.AddWithValue("@Status", hoist.Status);
            command.Parameters.AddWithValue("@CurrentJobId", hoist.CurrentJobId.HasValue ? (object)hoist.CurrentJobId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@LineNo", hoist.LineNo);
            command.Parameters.AddWithValue("@IsAuto", hoist.IsAuto ? 1 : 0);
            command.Parameters.AddWithValue("@PositionIndex", hoist.PositionIndex);
            command.Parameters.AddWithValue("@StateTicks", hoist.StateTicks);
        }

        private static HoistModel MapHoist(SQLiteDataReader reader)
        {
            return new HoistModel
            {
                HoistId = Convert.ToInt32(reader["HoistId"]),
                HoistName = Convert.ToString(reader["HoistName"]),
                CurrentTankNo = Convert.ToInt32(reader["CurrentTankNo"]),
                TargetTankNo = Convert.ToInt32(reader["TargetTankNo"]),
                Status = Convert.ToString(reader["Status"]),
                CurrentJobId = reader["CurrentJobId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CurrentJobId"]),
                LineNo = Convert.ToInt32(reader["LineNo"]),
                IsAuto = Convert.ToInt32(reader["IsAuto"]) == 1,
                PositionIndex = Convert.ToDouble(reader["PositionIndex"]),
                StateTicks = Convert.ToInt32(reader["StateTicks"])
            };
        }
    }
}
