using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class TankRepository
    {
        public List<TankModel> GetAll()
        {
            var tanks = new List<TankModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Tanks ORDER BY TankNumber;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tanks.Add(MapTank(reader));
                    }
                }
            }

            return tanks;
        }

        public int Add(TankModel tank)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Tanks (TankNumber, Name, ChemicalName, CapacityLiters, CurrentLevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, IsActive)
                  VALUES (@TankNumber, @Name, @ChemicalName, @CapacityLiters, @CurrentLevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, @Status, @IsActive);
                  SELECT last_insert_rowid();",
                connection))
            {
                AddTankParameters(command, tank);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(TankModel tank)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Tanks
                  SET TankNumber = @TankNumber,
                      Name = @Name,
                      ChemicalName = @ChemicalName,
                      CapacityLiters = @CapacityLiters,
                      CurrentLevelLiters = @CurrentLevelLiters,
                      TemperatureCelsius = @TemperatureCelsius,
                      Voltage = @Voltage,
                      CurrentAmps = @CurrentAmps,
                      Status = @Status,
                      IsActive = @IsActive
                  WHERE Id = @Id;",
                connection))
            {
                AddTankParameters(command, tank);
                command.Parameters.AddWithValue("@Id", tank.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("DELETE FROM Tanks WHERE Id = @Id;", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddTankParameters(SQLiteCommand command, TankModel tank)
        {
            command.Parameters.AddWithValue("@TankNumber", tank.TankNumber);
            command.Parameters.AddWithValue("@Name", tank.Name);
            command.Parameters.AddWithValue("@ChemicalName", tank.ChemicalName);
            command.Parameters.AddWithValue("@CapacityLiters", tank.CapacityLiters);
            command.Parameters.AddWithValue("@CurrentLevelLiters", tank.CurrentLevelLiters);
            command.Parameters.AddWithValue("@TemperatureCelsius", tank.TemperatureCelsius);
            command.Parameters.AddWithValue("@Voltage", tank.Voltage);
            command.Parameters.AddWithValue("@CurrentAmps", tank.CurrentAmps);
            command.Parameters.AddWithValue("@Status", tank.Status);
            command.Parameters.AddWithValue("@IsActive", tank.IsActive ? 1 : 0);
        }

        private static TankModel MapTank(SQLiteDataReader reader)
        {
            return new TankModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                TankNumber = Convert.ToInt32(reader["TankNumber"]),
                Name = Convert.ToString(reader["Name"]),
                ChemicalName = Convert.ToString(reader["ChemicalName"]),
                CapacityLiters = Convert.ToDouble(reader["CapacityLiters"]),
                CurrentLevelLiters = Convert.ToDouble(reader["CurrentLevelLiters"]),
                TemperatureCelsius = Convert.ToDouble(reader["TemperatureCelsius"]),
                Voltage = Convert.ToDouble(reader["Voltage"]),
                CurrentAmps = Convert.ToDouble(reader["CurrentAmps"]),
                Status = Convert.ToString(reader["Status"]),
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
            };
        }
    }
}
