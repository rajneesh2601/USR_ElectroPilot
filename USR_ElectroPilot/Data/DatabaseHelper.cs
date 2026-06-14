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
                    SeedDefaultProcessSteps(connection, transaction);
                    SeedDefaultHoistStatus(connection, transaction);

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
                @"CREATE TABLE IF NOT EXISTS ProcessSteps (
                    StepId INTEGER PRIMARY KEY AUTOINCREMENT,
                    StepNo INTEGER NOT NULL,
                    TankId INTEGER NOT NULL,
                    StepName TEXT NOT NULL,
                    DurationSeconds INTEGER NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (TankId) REFERENCES Tanks(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS HoistStatus (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CurrentTankId INTEGER NULL,
                    Status TEXT NOT NULL,
                    LastUpdated TEXT NOT NULL,
                    FOREIGN KEY (CurrentTankId) REFERENCES Tanks(Id)
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
                new { Username = "viewer", Password = "view123", Role = "Viewer", DisplayName = "Viewer" }
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

            using (var command = new SQLiteCommand(
                "UPDATE Users SET Role = 'Viewer' WHERE Username = 'viewer' AND DisplayName = 'Viewer';",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void SeedSystemSettings(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var settings = new[]
            {
                new { Key = "PlantName", Value = "USR ElectroPilot", Description = "Displayed plant name" },
                new { Key = "AlarmRefreshSeconds", Value = "5", Description = "Alarm grid refresh interval" },
                new { Key = "LoginLockoutMinutes", Value = "15", Description = "Lockout duration after failed logins" },
                new { Key = "MaxFailedLoginAttempts", Value = "5", Description = "Failed login attempts before lockout" },
                new { Key = "ScadaTankRows", Value = "1", Description = "Number of horizontal SCADA tank process lines" }
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

            var tankNames = new[]
            {
                "Loading Station",
                "Cleaning Tank",
                "Rinse Tank 1",
                "Acid Tank",
                "Rinse Tank 2",
                "Electroplating Tank",
                "Rinse Tank 3",
                "Drying Tank",
                "Unloading Station"
            };

            for (var tankNumber = 1; tankNumber <= tankNames.Length; tankNumber++)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT INTO Tanks (TankNumber, Name, ChemicalName, CapacityLiters, CurrentLevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, IsActive)
                      VALUES (@TankNumber, @Name, @ChemicalName, @CapacityLiters, @CurrentLevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, @Status, @IsActive);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@TankNumber", tankNumber);
                    command.Parameters.AddWithValue("@Name", "T" + tankNumber);
                    command.Parameters.AddWithValue("@ChemicalName", tankNames[tankNumber - 1]);
                    command.Parameters.AddWithValue("@CapacityLiters", 1000);
                    command.Parameters.AddWithValue("@CurrentLevelLiters", 750);
                    command.Parameters.AddWithValue("@TemperatureCelsius", 32 + tankNumber);
                    command.Parameters.AddWithValue("@Voltage", tankNumber == 6 ? 12 : 0);
                    command.Parameters.AddWithValue("@CurrentAmps", tankNumber == 6 ? 140 : 0);
                    command.Parameters.AddWithValue("@Status", "Normal");
                    command.Parameters.AddWithValue("@IsActive", 1);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedDefaultProcessSteps(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var countCommand = new SQLiteCommand("SELECT COUNT(1) FROM ProcessSteps;", connection, transaction))
            {
                if (Convert.ToInt32(countCommand.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            var steps = new[]
            {
                new { StepNo = 1, TankNumber = 1, StepName = "Loading Station", DurationSeconds = 8 },
                new { StepNo = 2, TankNumber = 2, StepName = "Cleaning Tank", DurationSeconds = 18 },
                new { StepNo = 3, TankNumber = 3, StepName = "Rinse Tank 1", DurationSeconds = 10 },
                new { StepNo = 4, TankNumber = 4, StepName = "Acid Tank", DurationSeconds = 14 },
                new { StepNo = 5, TankNumber = 5, StepName = "Rinse Tank 2", DurationSeconds = 10 },
                new { StepNo = 6, TankNumber = 6, StepName = "Electroplating Tank", DurationSeconds = 24 },
                new { StepNo = 7, TankNumber = 7, StepName = "Rinse Tank 3", DurationSeconds = 10 },
                new { StepNo = 8, TankNumber = 8, StepName = "Drying Tank", DurationSeconds = 16 },
                new { StepNo = 9, TankNumber = 9, StepName = "Unloading Station", DurationSeconds = 8 }
            };

            foreach (var step in steps)
            {
                int tankId;
                using (var tankCommand = new SQLiteCommand("SELECT Id FROM Tanks WHERE TankNumber = @TankNumber;", connection, transaction))
                {
                    tankCommand.Parameters.AddWithValue("@TankNumber", step.TankNumber);
                    var value = tankCommand.ExecuteScalar();
                    if (value == null || value == DBNull.Value)
                    {
                        continue;
                    }

                    tankId = Convert.ToInt32(value);
                }

                using (var command = new SQLiteCommand(
                    @"INSERT INTO ProcessSteps (StepNo, TankId, StepName, DurationSeconds, IsActive)
                      VALUES (@StepNo, @TankId, @StepName, @DurationSeconds, 1);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@StepNo", step.StepNo);
                    command.Parameters.AddWithValue("@TankId", tankId);
                    command.Parameters.AddWithValue("@StepName", step.StepName);
                    command.Parameters.AddWithValue("@DurationSeconds", step.DurationSeconds);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedDefaultHoistStatus(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(
                @"INSERT OR IGNORE INTO HoistStatus (Id, CurrentTankId, Status, LastUpdated)
                  VALUES (1, (SELECT Id FROM Tanks ORDER BY TankNumber LIMIT 1), 'Idle', datetime('now'));",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
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
