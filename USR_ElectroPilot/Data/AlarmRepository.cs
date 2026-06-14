using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class AlarmRepository
    {
        public List<AlarmModel> GetAll()
        {
            return Query("SELECT * FROM Alarms ORDER BY RaisedAt DESC;");
        }

        public List<AlarmModel> GetActive()
        {
            return Query("SELECT * FROM Alarms WHERE State = 'Active' ORDER BY RaisedAt DESC;");
        }

        public int Add(AlarmModel alarm)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Alarms (Source, Severity, Message, State)
                  VALUES (@Source, @Severity, @Message, @State);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@Source", alarm.Source);
                command.Parameters.AddWithValue("@Severity", alarm.Severity);
                command.Parameters.AddWithValue("@Message", alarm.Message);
                command.Parameters.AddWithValue("@State", alarm.State);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool HasActive(string source, string message)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"SELECT COUNT(1)
                  FROM Alarms
                  WHERE State = 'Active'
                    AND Source = @Source
                    AND Message = @Message;",
                connection))
            {
                command.Parameters.AddWithValue("@Source", source);
                command.Parameters.AddWithValue("@Message", message);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        public void Acknowledge(int id, string username)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Alarms
                  SET State = 'Acknowledged',
                      AcknowledgedAt = datetime('now'),
                      AcknowledgedBy = @AcknowledgedBy
                  WHERE Id = @Id;",
                connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@AcknowledgedBy", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AcknowledgeActive(string username)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Alarms
                  SET State = 'Acknowledged',
                      AcknowledgedAt = datetime('now'),
                      AcknowledgedBy = @AcknowledgedBy
                  WHERE State = 'Active';",
                connection))
            {
                command.Parameters.AddWithValue("@AcknowledgedBy", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static List<AlarmModel> Query(string sql)
        {
            var alarms = new List<AlarmModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alarms.Add(new AlarmModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Source = Convert.ToString(reader["Source"]),
                            Severity = Convert.ToString(reader["Severity"]),
                            Message = Convert.ToString(reader["Message"]),
                            State = Convert.ToString(reader["State"]),
                            RaisedAt = DateTime.Parse(Convert.ToString(reader["RaisedAt"])),
                            AcknowledgedAt = ReadNullableDate(reader, "AcknowledgedAt"),
                            AcknowledgedBy = reader["AcknowledgedBy"] == DBNull.Value ? null : Convert.ToString(reader["AcknowledgedBy"]),
                            ShelvedUntil = ReadNullableDate(reader, "ShelvedUntil")
                        });
                    }
                }
            }

            return alarms;
        }

        private static DateTime? ReadNullableDate(SQLiteDataReader reader, string name)
        {
            return reader[name] == DBNull.Value ? (DateTime?)null : DateTime.Parse(Convert.ToString(reader[name]));
        }
    }
}
