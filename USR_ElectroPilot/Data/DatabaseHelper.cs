using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace USR_ElectroPilot.Data
{
    public static class DatabaseHelper
    {
        public static void InitializeDatabase()
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            {
                connection.Open();

                using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var statement in GetSchemaStatements())
                    {
                        ExecuteNonQuery(connection, transaction, statement);
                    }

                    SeedDefaultUsers(connection, transaction);
                    SeedSystemSettings(connection, transaction);
                    SeedDefaultTanks(connection, transaction);

                    transaction.Commit();
                }
            }
        }

        private static IEnumerable<string> GetSchemaStatements()
        {
            return new[]
            {
                @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    DisplayName TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FailedLoginCount INTEGER NOT NULL DEFAULT 0,
                    LockoutUntil TEXT NULL,
                    LastLoginAt TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS UserActivity (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NULL,
                    Username TEXT NOT NULL,
                    ActivityType TEXT NOT NULL,
                    Details TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS AuditLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    EntityType TEXT NULL,
                    EntityId TEXT NULL,
                    OldValue TEXT NULL,
                    NewValue TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS SystemSettings (
                    SettingKey TEXT PRIMARY KEY,
                    SettingValue TEXT NOT NULL,
                    Description TEXT NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS Tanks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TankNumber INTEGER NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    ChemicalName TEXT NOT NULL,
                    CapacityLiters REAL NOT NULL DEFAULT 0,
                    CurrentLevelLiters REAL NOT NULL DEFAULT 0,
                    TemperatureCelsius REAL NOT NULL DEFAULT 25,
                    Voltage REAL NOT NULL DEFAULT 0,
                    CurrentAmps REAL NOT NULL DEFAULT 0,
                    Status TEXT NOT NULL DEFAULT 'Normal',
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS TankHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TankId INTEGER NOT NULL,
                    LevelLiters REAL NOT NULL,
                    TemperatureCelsius REAL NOT NULL,
                    Voltage REAL NOT NULL,
                    CurrentAmps REAL NOT NULL,
                    Status TEXT NOT NULL,
                    RecordedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    FOREIGN KEY (TankId) REFERENCES Tanks(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS Wagons (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WagonCode TEXT NOT NULL UNIQUE,
                    State TEXT NOT NULL DEFAULT 'Ready',
                    CurrentTankId INTEGER NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    FOREIGN KEY (CurrentTankId) REFERENCES Tanks(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS Rectifiers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Voltage REAL NOT NULL DEFAULT 0,
                    CurrentAmps REAL NOT NULL DEFAULT 0,
                    IsRunning INTEGER NOT NULL DEFAULT 0,
                    FaultCode TEXT NULL,
                    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS Alarms (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Source TEXT NOT NULL,
                    Severity TEXT NOT NULL,
                    Message TEXT NOT NULL,
                    State TEXT NOT NULL DEFAULT 'Active',
                    RaisedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    AcknowledgedAt TEXT NULL,
                    AcknowledgedBy TEXT NULL,
                    ShelvedUntil TEXT NULL
                );",
                @"CREATE TABLE IF NOT EXISTS Recipes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT NULL,
                    TargetVoltage REAL NOT NULL DEFAULT 0,
                    TargetCurrentAmps REAL NOT NULL DEFAULT 0,
                    DurationMinutes INTEGER NOT NULL DEFAULT 0,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );",
                @"CREATE TABLE IF NOT EXISTS Loads (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LoadNumber TEXT NOT NULL UNIQUE,
                    RecipeId INTEGER NULL,
                    WagonId INTEGER NULL,
                    Status TEXT NOT NULL DEFAULT 'Queued',
                    StartedAt TEXT NULL,
                    CompletedAt TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    FOREIGN KEY (RecipeId) REFERENCES Recipes(Id),
                    FOREIGN KEY (WagonId) REFERENCES Wagons(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS ShiftReports (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShiftName TEXT NOT NULL,
                    OperatorUsername TEXT NOT NULL,
                    StartedAt TEXT NOT NULL,
                    EndedAt TEXT NULL,
                    TotalLoads INTEGER NOT NULL DEFAULT 0,
                    AlarmCount INTEGER NOT NULL DEFAULT 0,
                    Notes TEXT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
                );"
            };
        }

        private static void SeedDefaultUsers(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var users = new[]
            {
                new { Username = "admin", Password = "admin123", Role = "Admin", DisplayName = "Administrator" },
                new { Username = "sadmin", Password = "sadmin123", Role = "Supervisor", DisplayName = "Supervisor" },
                new { Username = "operator", Password = "op123", Role = "Operator", DisplayName = "Operator" },
                new { Username = "viewer", Password = "view123", Role = "Operator", DisplayName = "Viewer" }
            };

            foreach (var user in users)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT OR IGNORE INTO Users (Username, PasswordHash, Role, DisplayName)
                      VALUES (@Username, @PasswordHash, @Role, @DisplayName);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(user.Password));
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedSystemSettings(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var settings = new[]
            {
                new { Key = "PlantName", Value = "USR ElectroPilot", Description = "Displayed plant name" },
                new { Key = "AlarmRefreshSeconds", Value = "5", Description = "Alarm grid refresh interval" },
                new { Key = "LoginLockoutMinutes", Value = "15", Description = "Lockout duration after failed logins" },
                new { Key = "MaxFailedLoginAttempts", Value = "5", Description = "Failed login attempts before lockout" }
            };

            foreach (var setting in settings)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT OR IGNORE INTO SystemSettings (SettingKey, SettingValue, Description)
                      VALUES (@SettingKey, @SettingValue, @Description);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@SettingKey", setting.Key);
                    command.Parameters.AddWithValue("@SettingValue", setting.Value);
                    command.Parameters.AddWithValue("@Description", setting.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedDefaultTanks(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var countCommand = new SQLiteCommand("SELECT COUNT(1) FROM Tanks;", connection, transaction))
            {
                if (Convert.ToInt32(countCommand.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            for (var tankNumber = 1; tankNumber <= 10; tankNumber++)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT INTO Tanks (TankNumber, Name, ChemicalName, CapacityLiters, CurrentLevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, IsActive)
                      VALUES (@TankNumber, @Name, @ChemicalName, @CapacityLiters, @CurrentLevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, @Status, @IsActive);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@TankNumber", tankNumber);
                    command.Parameters.AddWithValue("@Name", "T" + tankNumber);
                    command.Parameters.AddWithValue("@ChemicalName", "Process Chemical");
                    command.Parameters.AddWithValue("@CapacityLiters", 1000);
                    command.Parameters.AddWithValue("@CurrentLevelLiters", 750);
                    command.Parameters.AddWithValue("@TemperatureCelsius", 35);
                    command.Parameters.AddWithValue("@Voltage", 12);
                    command.Parameters.AddWithValue("@CurrentAmps", 100);
                    command.Parameters.AddWithValue("@Status", "Normal");
                    command.Parameters.AddWithValue("@IsActive", 1);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, SQLiteTransaction transaction, string statement)
        {
            using (var command = new SQLiteCommand(statement, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(bytes.Length * 2);

                foreach (var value in bytes)
                {
                    builder.Append(value.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
