using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class UserRepository
    {
        public List<UserModel> GetAll()
        {
            var users = new List<UserModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Users ORDER BY Username;", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapUser(reader));
                    }
                }
            }

            return users;
        }

        public UserModel GetByUsername(string username)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Users WHERE Username = @Username;", connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return reader.Read() ? MapUser(reader) : null;
                }
            }
        }

        public int Add(UserModel user)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Users (Username, PasswordHash, Role, DisplayName, IsActive)
                  VALUES (@Username, @PasswordHash, @Role, @DisplayName, @IsActive);
                  SELECT last_insert_rowid();",
                connection))
            {
                AddUserParameters(command, user);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(UserModel user)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Users
                  SET PasswordHash = @PasswordHash,
                      Role = @Role,
                      DisplayName = @DisplayName,
                      IsActive = @IsActive,
                      FailedLoginCount = @FailedLoginCount,
                      LockoutUntil = @LockoutUntil,
                      LastLoginAt = @LastLoginAt
                  WHERE Id = @Id;",
                connection))
            {
                AddUserParameters(command, user);
                command.Parameters.AddWithValue("@FailedLoginCount", user.FailedLoginCount);
                command.Parameters.AddWithValue("@LockoutUntil", ToDbValue(user.LockoutUntil));
                command.Parameters.AddWithValue("@LastLoginAt", ToDbValue(user.LastLoginAt));
                command.Parameters.AddWithValue("@Id", user.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddUserParameters(SQLiteCommand command, UserModel user)
        {
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Role", user.Role);
            command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            command.Parameters.AddWithValue("@IsActive", user.IsActive ? 1 : 0);
        }

        private static UserModel MapUser(SQLiteDataReader reader)
        {
            return new UserModel
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = Convert.ToString(reader["Username"]),
                PasswordHash = Convert.ToString(reader["PasswordHash"]),
                Role = Convert.ToString(reader["Role"]),
                DisplayName = Convert.ToString(reader["DisplayName"]),
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                FailedLoginCount = Convert.ToInt32(reader["FailedLoginCount"]),
                LockoutUntil = ReadNullableDate(reader, "LockoutUntil"),
                LastLoginAt = ReadNullableDate(reader, "LastLoginAt"),
                CreatedAt = ReadDate(reader, "CreatedAt")
            };
        }

        private static DateTime ReadDate(SQLiteDataReader reader, string name)
        {
            return DateTime.Parse(Convert.ToString(reader[name]));
        }

        private static DateTime? ReadNullableDate(SQLiteDataReader reader, string name)
        {
            return reader[name] == DBNull.Value ? (DateTime?)null : ReadDate(reader, name);
        }

        private static object ToDbValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value;
        }
    }
}
