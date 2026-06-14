using System;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class HoistStatusRepository
    {
        public HoistStatusModel GetCurrent()
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM HoistStatus ORDER BY Id DESC LIMIT 1;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new HoistStatusModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            CurrentTankId = reader["CurrentTankId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CurrentTankId"]),
                            Status = Convert.ToString(reader["Status"]),
                            LastUpdated = DateTime.Parse(Convert.ToString(reader["LastUpdated"]))
                        };
                    }
                }
            }

            return new HoistStatusModel { Status = "Idle", LastUpdated = DateTime.Now };
        }

        public void Save(HoistStatusModel status)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            {
                connection.Open();

                using (var update = new SQLiteCommand(
                    @"UPDATE HoistStatus
                      SET CurrentTankId = @CurrentTankId,
                          Status = @Status,
                          LastUpdated = @LastUpdated
                      WHERE Id = 1;",
                    connection))
                {
                    AddParameters(update, status);
                    if (update.ExecuteNonQuery() > 0)
                    {
                        return;
                    }
                }

                using (var insert = new SQLiteCommand(
                    @"INSERT INTO HoistStatus (Id, CurrentTankId, Status, LastUpdated)
                      VALUES (1, @CurrentTankId, @Status, @LastUpdated);",
                    connection))
                {
                    AddParameters(insert, status);
                    insert.ExecuteNonQuery();
                }
            }
        }

        private static void AddParameters(SQLiteCommand command, HoistStatusModel status)
        {
            command.Parameters.AddWithValue("@CurrentTankId", status.CurrentTankId.HasValue ? (object)status.CurrentTankId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@Status", status.Status);
            command.Parameters.AddWithValue("@LastUpdated", status.LastUpdated.ToString("s"));
        }
    }
}
