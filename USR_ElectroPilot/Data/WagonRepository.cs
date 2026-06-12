using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class WagonRepository
    {
        public List<WagonModel> GetAll()
        {
            var wagons = new List<WagonModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Wagons ORDER BY WagonCode;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        wagons.Add(new WagonModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            WagonCode = Convert.ToString(reader["WagonCode"]),
                            State = Convert.ToString(reader["State"]),
                            CurrentTankId = reader["CurrentTankId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CurrentTankId"]),
                            UpdatedAt = DateTime.Parse(Convert.ToString(reader["UpdatedAt"]))
                        });
                    }
                }
            }

            return wagons;
        }

        public int Add(WagonModel wagon)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Wagons (WagonCode, State, CurrentTankId)
                  VALUES (@WagonCode, @State, @CurrentTankId);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@WagonCode", wagon.WagonCode);
                command.Parameters.AddWithValue("@State", wagon.State);
                command.Parameters.AddWithValue("@CurrentTankId", wagon.CurrentTankId.HasValue ? (object)wagon.CurrentTankId.Value : DBNull.Value);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
