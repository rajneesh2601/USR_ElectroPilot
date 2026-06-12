using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class AuditRepository
    {
        public List<AuditLogModel> GetAll()
        {
            var logs = new List<AuditLogModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM AuditLogs ORDER BY CreatedAt DESC;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new AuditLogModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = Convert.ToString(reader["Username"]),
                            Action = Convert.ToString(reader["Action"]),
                            EntityType = reader["EntityType"] == DBNull.Value ? null : Convert.ToString(reader["EntityType"]),
                            EntityId = reader["EntityId"] == DBNull.Value ? null : Convert.ToString(reader["EntityId"]),
                            OldValue = reader["OldValue"] == DBNull.Value ? null : Convert.ToString(reader["OldValue"]),
                            NewValue = reader["NewValue"] == DBNull.Value ? null : Convert.ToString(reader["NewValue"]),
                            CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
                        });
                    }
                }
            }

            return logs;
        }

        public int Add(AuditLogModel log)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO AuditLogs (Username, Action, EntityType, EntityId, OldValue, NewValue)
                  VALUES (@Username, @Action, @EntityType, @EntityId, @OldValue, @NewValue);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@Username", log.Username);
                command.Parameters.AddWithValue("@Action", log.Action);
                command.Parameters.AddWithValue("@EntityType", (object)log.EntityType ?? DBNull.Value);
                command.Parameters.AddWithValue("@EntityId", (object)log.EntityId ?? DBNull.Value);
                command.Parameters.AddWithValue("@OldValue", (object)log.OldValue ?? DBNull.Value);
                command.Parameters.AddWithValue("@NewValue", (object)log.NewValue ?? DBNull.Value);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
