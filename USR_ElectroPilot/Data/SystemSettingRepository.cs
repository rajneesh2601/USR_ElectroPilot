using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class SystemSettingRepository
    {
        public List<SystemSettingModel> GetAll()
        {
            var settings = new List<SystemSettingModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM SystemSettings ORDER BY SettingKey;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        settings.Add(new SystemSettingModel
                        {
                            SettingKey = Convert.ToString(reader["SettingKey"]),
                            SettingValue = Convert.ToString(reader["SettingValue"]),
                            Description = reader["Description"] == DBNull.Value ? null : Convert.ToString(reader["Description"]),
                            UpdatedAt = DateTime.Parse(Convert.ToString(reader["UpdatedAt"]))
                        });
                    }
                }
            }

            return settings;
        }

        public string GetValue(string settingKey)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT SettingValue FROM SystemSettings WHERE SettingKey = @SettingKey;", connection))
            {
                command.Parameters.AddWithValue("@SettingKey", settingKey);
                connection.Open();
                var value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? null : Convert.ToString(value);
            }
        }

        public void Upsert(SystemSettingModel setting)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO SystemSettings (SettingKey, SettingValue, Description, UpdatedAt)
                  VALUES (@SettingKey, @SettingValue, @Description, datetime('now'))
                  ON CONFLICT(SettingKey) DO UPDATE SET
                      SettingValue = excluded.SettingValue,
                      Description = excluded.Description,
                      UpdatedAt = datetime('now');",
                connection))
            {
                command.Parameters.AddWithValue("@SettingKey", setting.SettingKey);
                command.Parameters.AddWithValue("@SettingValue", setting.SettingValue);
                command.Parameters.AddWithValue("@Description", (object)setting.Description ?? DBNull.Value);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
