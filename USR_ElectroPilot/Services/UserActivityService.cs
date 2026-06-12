using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class UserActivityService
    {
        public void RecordActivity(int? userId, string username, string activityType, string details)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO UserActivity (UserId, Username, ActivityType, Details)
                  VALUES (@UserId, @Username, @ActivityType, @Details);",
                connection))
            {
                command.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@ActivityType", activityType);
                command.Parameters.AddWithValue("@Details", (object)details ?? DBNull.Value);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<UserActivityModel> GetRecent(int limit)
        {
            var activities = new List<UserActivityModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"SELECT * FROM UserActivity
                  ORDER BY CreatedAt DESC
                  LIMIT @Limit;",
                connection))
            {
                command.Parameters.AddWithValue("@Limit", limit);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activities.Add(new UserActivityModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            UserId = reader["UserId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["UserId"]),
                            Username = Convert.ToString(reader["Username"]),
                            ActivityType = Convert.ToString(reader["ActivityType"]),
                            Details = reader["Details"] == DBNull.Value ? null : Convert.ToString(reader["Details"]),
                            CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
                        });
                    }
                }
            }

            return activities;
        }
    }
}
