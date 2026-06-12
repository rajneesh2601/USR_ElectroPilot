using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class RectifierRepository
    {
        public List<RectifierModel> GetAll()
        {
            var rectifiers = new List<RectifierModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Rectifiers ORDER BY Name;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rectifiers.Add(new RectifierModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            Voltage = Convert.ToDouble(reader["Voltage"]),
                            CurrentAmps = Convert.ToDouble(reader["CurrentAmps"]),
                            IsRunning = Convert.ToInt32(reader["IsRunning"]) == 1,
                            FaultCode = reader["FaultCode"] == DBNull.Value ? null : Convert.ToString(reader["FaultCode"]),
                            UpdatedAt = DateTime.Parse(Convert.ToString(reader["UpdatedAt"]))
                        });
                    }
                }
            }

            return rectifiers;
        }

        public int Add(RectifierModel rectifier)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Rectifiers (Name, Voltage, CurrentAmps, IsRunning, FaultCode)
                  VALUES (@Name, @Voltage, @CurrentAmps, @IsRunning, @FaultCode);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@Name", rectifier.Name);
                command.Parameters.AddWithValue("@Voltage", rectifier.Voltage);
                command.Parameters.AddWithValue("@CurrentAmps", rectifier.CurrentAmps);
                command.Parameters.AddWithValue("@IsRunning", rectifier.IsRunning ? 1 : 0);
                command.Parameters.AddWithValue("@FaultCode", (object)rectifier.FaultCode ?? DBNull.Value);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
