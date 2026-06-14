using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class TankHistoryRepository
    {
        public void Add(TankHistoryModel history)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO TankHistory (TankId, LevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, RecordedAt)
                  VALUES (@TankId, @LevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, @Status, @RecordedAt);",
                connection))
            {
                command.Parameters.AddWithValue("@TankId", history.TankId);
                command.Parameters.AddWithValue("@LevelLiters", history.LevelLiters);
                command.Parameters.AddWithValue("@TemperatureCelsius", history.TemperatureCelsius);
                command.Parameters.AddWithValue("@Voltage", history.Voltage);
                command.Parameters.AddWithValue("@CurrentAmps", history.CurrentAmps);
                command.Parameters.AddWithValue("@Status", history.Status);
                command.Parameters.AddWithValue("@RecordedAt", history.RecordedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<TankHistoryModel> GetForTank(int tankId, DateTime from, DateTime to)
        {
            var history = new List<TankHistoryModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"SELECT *
                  FROM TankHistory
                  WHERE TankId = @TankId
                    AND RecordedAt >= @From
                    AND RecordedAt <= @To
                  ORDER BY RecordedAt;",
                connection))
            {
                command.Parameters.AddWithValue("@TankId", tankId);
                command.Parameters.AddWithValue("@From", from.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@To", to.ToString("yyyy-MM-dd HH:mm:ss"));
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        history.Add(new TankHistoryModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            TankId = Convert.ToInt32(reader["TankId"]),
                            LevelLiters = Convert.ToDouble(reader["LevelLiters"]),
                            TemperatureCelsius = Convert.ToDouble(reader["TemperatureCelsius"]),
                            Voltage = Convert.ToDouble(reader["Voltage"]),
                            CurrentAmps = Convert.ToDouble(reader["CurrentAmps"]),
                            Status = Convert.ToString(reader["Status"]),
                            RecordedAt = DateTime.Parse(Convert.ToString(reader["RecordedAt"]))
                        });
                    }
                }
            }

            return history;
        }
    }
}
