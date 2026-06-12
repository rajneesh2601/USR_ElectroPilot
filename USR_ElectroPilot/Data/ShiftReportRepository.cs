using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class ShiftReportRepository
    {
        public List<ShiftReportModel> GetAll()
        {
            var reports = new List<ShiftReportModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM ShiftReports ORDER BY StartedAt DESC;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reports.Add(new ShiftReportModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ShiftName = Convert.ToString(reader["ShiftName"]),
                            OperatorUsername = Convert.ToString(reader["OperatorUsername"]),
                            StartedAt = DateTime.Parse(Convert.ToString(reader["StartedAt"])),
                            EndedAt = reader["EndedAt"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(Convert.ToString(reader["EndedAt"])),
                            TotalLoads = Convert.ToInt32(reader["TotalLoads"]),
                            AlarmCount = Convert.ToInt32(reader["AlarmCount"]),
                            Notes = reader["Notes"] == DBNull.Value ? null : Convert.ToString(reader["Notes"]),
                            CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
                        });
                    }
                }
            }

            return reports;
        }

        public int Add(ShiftReportModel report)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO ShiftReports (ShiftName, OperatorUsername, StartedAt, EndedAt, TotalLoads, AlarmCount, Notes)
                  VALUES (@ShiftName, @OperatorUsername, @StartedAt, @EndedAt, @TotalLoads, @AlarmCount, @Notes);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@ShiftName", report.ShiftName);
                command.Parameters.AddWithValue("@OperatorUsername", report.OperatorUsername);
                command.Parameters.AddWithValue("@StartedAt", report.StartedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@EndedAt", report.EndedAt.HasValue ? (object)report.EndedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
                command.Parameters.AddWithValue("@TotalLoads", report.TotalLoads);
                command.Parameters.AddWithValue("@AlarmCount", report.AlarmCount);
                command.Parameters.AddWithValue("@Notes", (object)report.Notes ?? DBNull.Value);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
