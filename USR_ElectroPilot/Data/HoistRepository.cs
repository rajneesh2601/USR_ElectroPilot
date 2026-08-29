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
            using (var command = new SQLiteCommand(
                @"SELECT h.*, l.LineName
                  FROM Hoists h
                  LEFT JOIN Lines l ON l.LineId = h.LineId
                  ORDER BY h.LineId, h.HoistId;",
                connection))
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

        public int Add(HoistModel hoist)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Hoists (LineId, HoistName, CurrentTankNo, TargetTankNo, Direction, HomeTank, FromTank, ToTank, Status, CurrentJobId, LineNo, IsAuto, PositionIndex, StateTicks)
                  VALUES (@LineId, @HoistName, @CurrentTankNo, @TargetTankNo, @Direction, @HomeTank, @FromTank, @ToTank, @Status, @CurrentJobId, @LineId, @IsAuto, @PositionIndex, @StateTicks);
                  SELECT last_insert_rowid();",
                connection))
            {
                AddParameters(command, hoist);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(HoistModel hoist)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Hoists
                  SET LineId = @LineId,
                      HoistName = @HoistName,
                      CurrentTankNo = @CurrentTankNo,
                      TargetTankNo = @TargetTankNo,
                      Direction = @Direction,
                      HomeTank = @HomeTank,
                      FromTank = @FromTank,
                      ToTank = @ToTank,
                      Status = @Status,
                      CurrentJobId = @CurrentJobId,
                      LineNo = @LineId,
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

        public void Delete(int hoistId)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("DELETE FROM Hoists WHERE HoistId = @HoistId;", connection))
            {
                command.Parameters.AddWithValue("@HoistId", hoistId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(SQLiteCommand command, HoistModel hoist)
        {
            command.Parameters.AddWithValue("@HoistId", hoist.HoistId);
            command.Parameters.AddWithValue("@LineId", hoist.LineId <= 0 ? 1 : hoist.LineId);
            command.Parameters.AddWithValue("@HoistName", hoist.HoistName);
            command.Parameters.AddWithValue("@CurrentTankNo", hoist.CurrentTankNo);
            command.Parameters.AddWithValue("@TargetTankNo", hoist.TargetTankNo);
            command.Parameters.AddWithValue("@Direction", string.IsNullOrEmpty(hoist.Direction) ? "LeftToRight" : hoist.Direction);
            command.Parameters.AddWithValue("@HomeTank", hoist.HomeTank <= 0 ? hoist.CurrentTankNo : hoist.HomeTank);
            command.Parameters.AddWithValue("@FromTank", hoist.FromTank <= 0 ? 1 : hoist.FromTank);
            command.Parameters.AddWithValue("@ToTank", hoist.ToTank <= 0 ? (hoist.HomeTank <= 0 ? hoist.CurrentTankNo : hoist.HomeTank) : hoist.ToTank);
            command.Parameters.AddWithValue("@Status", hoist.Status);
            command.Parameters.AddWithValue("@CurrentJobId", hoist.CurrentJobId.HasValue ? (object)hoist.CurrentJobId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@IsAuto", hoist.IsAuto ? 1 : 0);
            command.Parameters.AddWithValue("@PositionIndex", hoist.PositionIndex);
            command.Parameters.AddWithValue("@StateTicks", hoist.StateTicks);
        }

        private static HoistModel MapHoist(SQLiteDataReader reader)
        {
            return new HoistModel
            {
                HoistId = Convert.ToInt32(reader["HoistId"]),
                LineId = ReadInt(reader, "LineId", ReadInt(reader, "LineNo", 1)),
                LineName = ReadString(reader, "LineName", string.Empty),
                HoistName = Convert.ToString(reader["HoistName"]),
                CurrentTankNo = Convert.ToInt32(reader["CurrentTankNo"]),
                TargetTankNo = Convert.ToInt32(reader["TargetTankNo"]),
                Direction = ReadString(reader, "Direction", "LeftToRight"),
                HomeTank = ReadInt(reader, "HomeTank", Convert.ToInt32(reader["CurrentTankNo"])),
                FromTank = ReadInt(reader, "FromTank", 1),
                ToTank = ReadInt(reader, "ToTank", ReadInt(reader, "HomeTank", Convert.ToInt32(reader["CurrentTankNo"]))),
                Status = Convert.ToString(reader["Status"]),
                CurrentJobId = reader["CurrentJobId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CurrentJobId"]),
                IsAuto = Convert.ToInt32(reader["IsAuto"]) == 1,
                PositionIndex = Convert.ToDouble(reader["PositionIndex"]),
                StateTicks = Convert.ToInt32(reader["StateTicks"])
            };
        }

        private static int ReadInt(SQLiteDataReader reader, string columnName, int fallback)
        {
            try
            {
                var value = reader[columnName];
                return value == DBNull.Value ? fallback : Convert.ToInt32(value);
            }
            catch (IndexOutOfRangeException)
            {
                return fallback;
            }
        }

        private static string ReadString(SQLiteDataReader reader, string columnName, string fallback)
        {
            try
            {
                var value = reader[columnName];
                return value == DBNull.Value ? fallback : Convert.ToString(value);
            }
            catch (IndexOutOfRangeException)
            {
                return fallback;
            }
        }
    }
}
