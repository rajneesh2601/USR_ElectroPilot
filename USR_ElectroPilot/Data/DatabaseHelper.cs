using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

namespace USR_ElectroPilot.Data
{
    public static class DatabaseHelper
    {
        private const int DefaultMainLineTankCount = 18;
        private static readonly object _initializeLock = new object();
        private static bool _initialized;

        public static void InitializeDatabase()
        {
            if (_initialized)
            {
                return;
            }

            lock (_initializeLock)
            {
                if (_initialized)
                {
                    return;
                }

                using (var connection = SqliteConnectionFactory.CreateConnection())
                {
                    connection.Open();

                    using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    if (IsDatabaseReady(connection))
                    {
                        _initialized = true;
                        return;
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var statement in GetSchemaStatements())
                        {
                            ExecuteNonQuery(connection, transaction, statement);
                        }

                        MigrateProcessSteps(connection, transaction);

                        MigratePlantConfiguration(connection, transaction);

                        SeedDefaultUsers(connection, transaction);
                        SeedSystemSettings(connection, transaction);
                        SeedDefaultLines(connection, transaction);
                        SeedDefaultTanks(connection, transaction);
                        SeedDefaultProcessSteps(connection, transaction);
                        SeedDefaultHoists(connection, transaction);
                        SeedDefaultHoistStatus(connection, transaction);

                        transaction.Commit();
                    }

                    _initialized = true;
                }
            }
        }

        private static bool IsDatabaseReady(SQLiteConnection connection)
        {
            return TableExists(connection, "Users") &&
                TableExists(connection, "SystemSettings") &&
                TableExists(connection, "Lines") &&
                TableExists(connection, "Tanks") &&
                TableExists(connection, "ProcessSteps") &&
                TableExists(connection, "Hoists") &&
                TableExists(connection, "Jobs") &&
                TableExists(connection, "HoistStatus") &&
                TableExists(connection, "Alarms") &&
                ColumnExists(connection, "Tanks", "LineId") &&
                ColumnExists(connection, "Tanks", "TankNo") &&
                ColumnExists(connection, "ProcessSteps", "LineId") &&
                ColumnExists(connection, "ProcessSteps", "TankNo") &&
                ColumnExists(connection, "ProcessSteps", "ProcessName") &&
                ColumnExists(connection, "Hoists", "PositionIndex") &&
                ColumnExists(connection, "Hoists", "StateTicks") &&
                ColumnExists(connection, "Jobs", "StepDirection") &&
                HasRows(connection, "Users") &&
                HasRows(connection, "Lines") &&
                HasMinimumMainLineTanks(connection, DefaultMainLineTankCount) &&
                HasRows(connection, "ProcessSteps") &&
                HasRows(connection, "Hoists");
        }

        private static bool TableExists(SQLiteConnection connection, string tableName)
        {
            using (var command = new SQLiteCommand("SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = @TableName;", connection))
            {
                command.Parameters.AddWithValue("@TableName", tableName);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static bool ColumnExists(SQLiteConnection connection, string tableName, string columnName)
        {
            using (var command = new SQLiteCommand("PRAGMA table_info(" + tableName + ");", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(Convert.ToString(reader["name"]), columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool HasRows(SQLiteConnection connection, string tableName)
        {
            using (var command = new SQLiteCommand("SELECT COUNT(1) FROM " + tableName + ";", connection))
            {
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static bool HasMinimumMainLineTanks(SQLiteConnection connection, int minimumTankCount)
        {
            using (var command = new SQLiteCommand("SELECT COUNT(1) FROM Tanks WHERE LineId = 1 AND IsActive = 1;", connection))
            {
                return Convert.ToInt32(command.ExecuteScalar()) >= minimumTankCount;
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
                @"CREATE TABLE IF NOT EXISTS Lines (
                    LineId INTEGER PRIMARY KEY AUTOINCREMENT,
                    LineName TEXT NOT NULL UNIQUE,
                    IsActive INTEGER NOT NULL DEFAULT 1
                );",
                @"CREATE TABLE IF NOT EXISTS Tanks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LineId INTEGER NOT NULL DEFAULT 1,
                    TankNo INTEGER NOT NULL DEFAULT 1,
                    TankNumber INTEGER NULL UNIQUE,
                    Name TEXT NOT NULL,
                    ChemicalName TEXT NOT NULL,
                    CapacityLiters REAL NOT NULL DEFAULT 0,
                    CurrentLevelLiters REAL NOT NULL DEFAULT 0,
                    TemperatureCelsius REAL NOT NULL DEFAULT 25,
                    Voltage REAL NOT NULL DEFAULT 0,
                    CurrentAmps REAL NOT NULL DEFAULT 0,
                    Status TEXT NOT NULL DEFAULT 'Normal',
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                    UNIQUE(LineId, TankNo),
                    FOREIGN KEY (LineId) REFERENCES Lines(LineId)
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
                    LineId INTEGER NOT NULL DEFAULT 1,
                    TankId INTEGER NOT NULL,
                    TankNo INTEGER NULL,
                    StepName TEXT NOT NULL,
                    ProcessName TEXT NULL,
                    DurationSeconds INTEGER NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    FOREIGN KEY (LineId) REFERENCES Lines(LineId),
                    FOREIGN KEY (TankId) REFERENCES Tanks(Id)
                );",
                @"CREATE TABLE IF NOT EXISTS Hoists (
                    HoistId INTEGER PRIMARY KEY AUTOINCREMENT,
                    LineId INTEGER NOT NULL DEFAULT 1,
                    HoistName TEXT,
                    CurrentTankNo INTEGER,
                    TargetTankNo INTEGER,
                    Direction TEXT NOT NULL DEFAULT 'LeftToRight',
                    HomeTank INTEGER NOT NULL DEFAULT 1,
                    FromTank INTEGER NOT NULL DEFAULT 1,
                    ToTank INTEGER NOT NULL DEFAULT 1,
                    Status TEXT,
                    CurrentJobId INTEGER NULL,
                    LineNo INTEGER NULL,
                    IsAuto INTEGER,
                    PositionIndex REAL NOT NULL DEFAULT 0,
                    StateTicks INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (LineId) REFERENCES Lines(LineId)
                );",
                @"CREATE TABLE IF NOT EXISTS Jobs (
                    JobId INTEGER PRIMARY KEY AUTOINCREMENT,
                    JobNumber TEXT,
                    LineId INTEGER NOT NULL DEFAULT 1,
                    CurrentStep INTEGER,
                    StepDirection INTEGER NOT NULL DEFAULT 1,
                    CurrentTank INTEGER,
                    Status TEXT,
                    RemainingSeconds INTEGER NOT NULL DEFAULT 0,
                    StartedAt TEXT,
                    CompletedAt TEXT NULL
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

        private static void SeedDefaultLines(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var countCommand = new SQLiteCommand("SELECT COUNT(1) FROM Lines;", connection, transaction))
            {
                if (Convert.ToInt32(countCommand.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            var lines = new[] { "Main Line" };

            foreach (var lineName in lines)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT OR IGNORE INTO Lines (LineName, IsActive)
                      VALUES (@LineName, 1);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@LineName", lineName);
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
                "Unloading Station",
                "Process Chemical"
            };

            var lineId = 1;
            for (var tankNo = 1; tankNo <= tankNames.Length; tankNo++)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT INTO Tanks (LineId, TankNo, TankNumber, Name, ChemicalName, CapacityLiters, CurrentLevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, IsActive)
                      VALUES (@LineId, @TankNo, @TankNumber, @Name, @ChemicalName, @CapacityLiters, @CurrentLevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, @Status, @IsActive);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@LineId", lineId);
                    command.Parameters.AddWithValue("@TankNo", tankNo);
                    command.Parameters.AddWithValue("@TankNumber", GetLegacyTankNumber(lineId, tankNo));
                    command.Parameters.AddWithValue("@Name", "T" + tankNo);
                    command.Parameters.AddWithValue("@ChemicalName", tankNames[tankNo - 1]);
                    command.Parameters.AddWithValue("@CapacityLiters", 1000);
                    command.Parameters.AddWithValue("@CurrentLevelLiters", 750);
                    command.Parameters.AddWithValue("@TemperatureCelsius", 32 + tankNo);
                    command.Parameters.AddWithValue("@Voltage", tankNo == 6 ? 12 : 0);
                    command.Parameters.AddWithValue("@CurrentAmps", tankNo == 6 ? 140 : 0);
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
                new { StepNo = 1, LineId = 1, TankNo = 1, StepName = "Loading", DurationSeconds = 8 },
                new { StepNo = 2, LineId = 1, TankNo = 2, StepName = "Cleaning", DurationSeconds = 18 },
                new { StepNo = 3, LineId = 1, TankNo = 3, StepName = "Rinse 1", DurationSeconds = 10 },
                new { StepNo = 4, LineId = 1, TankNo = 4, StepName = "Acid", DurationSeconds = 14 },
                new { StepNo = 5, LineId = 1, TankNo = 5, StepName = "Rinse 2", DurationSeconds = 10 },
                new { StepNo = 6, LineId = 1, TankNo = 6, StepName = "Electroplating", DurationSeconds = 24 },
                new { StepNo = 7, LineId = 1, TankNo = 7, StepName = "Rinse 3", DurationSeconds = 10 },
                new { StepNo = 8, LineId = 1, TankNo = 8, StepName = "Drying", DurationSeconds = 16 },
                new { StepNo = 9, LineId = 1, TankNo = 9, StepName = "Unloading", DurationSeconds = 8 }
            };

            foreach (var step in steps)
            {
                int tankId;
                using (var tankCommand = new SQLiteCommand("SELECT Id FROM Tanks WHERE LineId = @LineId AND TankNo = @TankNo;", connection, transaction))
                {
                    tankCommand.Parameters.AddWithValue("@LineId", step.LineId);
                    tankCommand.Parameters.AddWithValue("@TankNo", step.TankNo);
                    var value = tankCommand.ExecuteScalar();
                    if (value == null || value == DBNull.Value)
                    {
                        continue;
                    }

                    tankId = Convert.ToInt32(value);
                }

                using (var command = new SQLiteCommand(
                    @"INSERT INTO ProcessSteps (StepNo, LineId, TankId, TankNo, StepName, ProcessName, DurationSeconds, IsActive)
                      VALUES (@StepNo, @LineId, @TankId, @TankNo, @StepName, @ProcessName, @DurationSeconds, 1);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@StepNo", step.StepNo);
                    command.Parameters.AddWithValue("@LineId", step.LineId);
                    command.Parameters.AddWithValue("@TankId", tankId);
                    command.Parameters.AddWithValue("@TankNo", step.TankNo);
                    command.Parameters.AddWithValue("@StepName", step.StepName);
                    command.Parameters.AddWithValue("@ProcessName", step.StepName);
                    command.Parameters.AddWithValue("@DurationSeconds", step.DurationSeconds);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedDefaultHoists(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var countCommand = new SQLiteCommand("SELECT COUNT(1) FROM Hoists;", connection, transaction))
            {
                if (Convert.ToInt32(countCommand.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            var hoists = new[]
            {
                new { Name = "H1", TankNo = 1, LineId = 1, Direction = "LeftToRight" }
            };

            foreach (var hoist in hoists)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT INTO Hoists (LineId, HoistName, CurrentTankNo, TargetTankNo, Direction, HomeTank, Status, CurrentJobId, LineNo, IsAuto, PositionIndex, StateTicks)
                      VALUES (@LineId, @HoistName, @CurrentTankNo, @TargetTankNo, @Direction, @HomeTank, 'Idle', NULL, @LineId, 1, @PositionIndex, 0);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@LineId", hoist.LineId);
                    command.Parameters.AddWithValue("@HoistName", hoist.Name);
                    command.Parameters.AddWithValue("@CurrentTankNo", hoist.TankNo);
                    command.Parameters.AddWithValue("@TargetTankNo", hoist.TankNo);
                    command.Parameters.AddWithValue("@Direction", hoist.Direction);
                    command.Parameters.AddWithValue("@HomeTank", hoist.TankNo);
                    command.Parameters.AddWithValue("@PositionIndex", hoist.TankNo - 1);
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

        private static void MigrateProcessSteps(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            EnsureColumn(connection, transaction, "ProcessSteps", "TankNo", "INTEGER NULL");
            EnsureColumn(connection, transaction, "ProcessSteps", "ProcessName", "TEXT NULL");

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE ProcessSteps
                  SET TankNo = COALESCE(TankNo, (SELECT TankNumber FROM Tanks WHERE Tanks.Id = ProcessSteps.TankId)),
                      ProcessName = COALESCE(ProcessName, StepName)
                  WHERE TankNo IS NULL OR ProcessName IS NULL;");
        }

        private static void MigratePlantConfiguration(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            SeedDefaultLines(connection, transaction);
            NormalizeLines(connection, transaction);

            EnsureColumn(connection, transaction, "Tanks", "LineId", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Tanks", "TankNo", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "ProcessSteps", "LineId", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Hoists", "LineId", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Hoists", "Direction", "TEXT NOT NULL DEFAULT 'LeftToRight'");
            EnsureColumn(connection, transaction, "Hoists", "HomeTank", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Hoists", "FromTank", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Hoists", "ToTank", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Hoists", "LineNo", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Jobs", "LineId", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(connection, transaction, "Jobs", "StepDirection", "INTEGER NOT NULL DEFAULT 1");

            NormalizeMainLineTanks(connection, transaction);

            NormalizeLines(connection, transaction);

            EnsureMinimumMainLineTanks(connection, transaction, DefaultMainLineTankCount);

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE ProcessSteps
                  SET LineId = COALESCE((SELECT LineId FROM Tanks WHERE Tanks.Id = ProcessSteps.TankId), LineId),
                      TankNo = COALESCE((SELECT TankNo FROM Tanks WHERE Tanks.Id = ProcessSteps.TankId), TankNo)
                  WHERE TankId IS NOT NULL;");

            if (ColumnExists(connection, transaction, "Hoists", "LineNo"))
            {
                ExecuteNonQuery(
                    connection,
                    transaction,
                    @"UPDATE Hoists
                      SET CurrentTankNo = CASE
                              WHEN LineNo IS NOT NULL AND LineNo > 1 AND LineNo < 1000 THEN ((LineNo - 1) * 5) + CurrentTankNo
                              WHEN LineId > 1 AND LineId < 1000 THEN ((LineId - 1) * 5) + CurrentTankNo
                              ELSE CurrentTankNo
                          END,
                          TargetTankNo = CASE
                              WHEN LineNo IS NOT NULL AND LineNo > 1 AND LineNo < 1000 THEN ((LineNo - 1) * 5) + TargetTankNo
                              WHEN LineId > 1 AND LineId < 1000 THEN ((LineId - 1) * 5) + TargetTankNo
                              ELSE TargetTankNo
                          END,
                          HomeTank = CASE
                              WHEN LineNo IS NOT NULL AND LineNo > 1 AND LineNo < 1000 THEN ((LineNo - 1) * 5) + HomeTank
                              WHEN LineId > 1 AND LineId < 1000 THEN ((LineId - 1) * 5) + HomeTank
                              ELSE HomeTank
                          END,
                          FromTank = CASE
                              WHEN LineNo IS NOT NULL AND LineNo > 1 AND LineNo < 1000 THEN ((LineNo - 1) * 5) + FromTank
                              WHEN LineId > 1 AND LineId < 1000 THEN ((LineId - 1) * 5) + FromTank
                              ELSE FromTank
                          END,
                          ToTank = CASE
                              WHEN LineNo IS NOT NULL AND LineNo > 1 AND LineNo < 1000 THEN ((LineNo - 1) * 5) + ToTank
                              WHEN LineId > 1 AND LineId < 1000 THEN ((LineId - 1) * 5) + ToTank
                              ELSE ToTank
                          END
                      WHERE LineNo IS NOT NULL AND LineNo > 1 OR LineId > 1;");
            }

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE Hoists
                  SET LineId = 1,
                      LineNo = 1,
                      HomeTank = CASE WHEN HomeTank IS NULL OR HomeTank <= 0 THEN CurrentTankNo ELSE HomeTank END,
                      FromTank = CASE WHEN FromTank IS NULL OR FromTank <= 0 THEN 1 ELSE FromTank END,
                      ToTank = CASE WHEN ToTank IS NULL OR ToTank <= 0 THEN HomeTank ELSE ToTank END,
                      Direction = COALESCE(Direction, 'LeftToRight'),
                      PositionIndex = CASE WHEN PositionIndex < 0 THEN 0 ELSE PositionIndex END;");

            NormalizeMainLineHoists(connection, transaction);

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE ProcessSteps
                  SET LineId = 1;");

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE Jobs
                  SET LineId = 1;");

            EnsureMainLineHoists(connection, transaction);
            KeepSingleMainHoist(connection, transaction);
            NormalizeMainLineHoists(connection, transaction);
        }

        private static void NormalizeLines(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(
                @"INSERT OR IGNORE INTO Lines (LineId, LineName, IsActive)
                  VALUES (1, '__Main Line Pending__', 1);",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                @"UPDATE Lines
                  SET LineName = '__Inactive Line ' || LineId,
                      IsActive = 0
                  WHERE LineId <> 1;",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                @"UPDATE Lines
                  SET LineName = 'Main Line',
                      IsActive = 1
                  WHERE LineId = 1;",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void NormalizeMainLineTanks(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            var rows = new List<TankMigrationRow>();
            using (var command = new SQLiteCommand("SELECT Id, LineId, TankNo, TankNumber, Name FROM Tanks ORDER BY LineId, TankNo, Id;", connection, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add(new TankMigrationRow
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        LineId = ReadInt(reader, "LineId", 1),
                        TankNo = ReadInt(reader, "TankNo", 1),
                        TankNumber = ReadInt(reader, "TankNumber", 0),
                        Name = Convert.ToString(reader["Name"])
                    });
                }
            }

            rows.Sort(delegate(TankMigrationRow left, TankMigrationRow right)
            {
                var compare = GetTankMigrationSortKey(left).CompareTo(GetTankMigrationSortKey(right));
                return compare != 0 ? compare : left.Id.CompareTo(right.Id);
            });

            ExecuteNonQuery(
                connection,
                transaction,
                @"UPDATE Tanks
                  SET LineId = 1,
                      TankNo = Id + 100000,
                      TankNumber = -2140000000 + Id;");

            for (var index = 0; index < rows.Count; index++)
            {
                var tankNo = index + 1;
                using (var command = new SQLiteCommand(
                    @"UPDATE Tanks
                      SET LineId = 1,
                          TankNo = @TankNo,
                          TankNumber = @TankNumber,
                          Name = @Name
                      WHERE Id = @Id;",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@Id", rows[index].Id);
                    command.Parameters.AddWithValue("@TankNo", tankNo);
                    command.Parameters.AddWithValue("@TankNumber", GetLegacyTankNumber(1, tankNo));
                    command.Parameters.AddWithValue("@Name", IsGeneratedTankName(rows[index].Name) ? "T" + tankNo : rows[index].Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static int GetTankMigrationSortKey(TankMigrationRow row)
        {
            if (row.TankNumber > 0 && row.TankNumber < 1000)
            {
                return row.TankNumber;
            }

            if (row.TankNumber >= 1000 && row.TankNumber < 100000)
            {
                return ((row.TankNumber / 1000) - 1) * 5 + (row.TankNumber % 1000);
            }

            if (row.LineId > 1 && row.LineId < 1000)
            {
                return ((row.LineId - 1) * 5) + row.TankNo;
            }

            return row.TankNo <= 0 ? row.Id : row.TankNo;
        }

        private static bool IsGeneratedTankName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return true;
            }

            var trimmed = name.Trim();
            if (trimmed.Length < 2 || trimmed[0] != 'T')
            {
                return false;
            }

            for (var i = 1; i < trimmed.Length; i++)
            {
                if (!char.IsDigit(trimmed[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private static int ReadInt(SQLiteDataReader reader, string columnName, int defaultValue)
        {
            var value = reader[columnName];
            return value == null || value == DBNull.Value ? defaultValue : Convert.ToInt32(value);
        }

        private class TankMigrationRow
        {
            public int Id { get; set; }
            public int LineId { get; set; }
            public int TankNo { get; set; }
            public int TankNumber { get; set; }
            public string Name { get; set; }
        }

        private static void EnsureMinimumMainLineTanks(SQLiteConnection connection, SQLiteTransaction transaction, int minimumTankNo)
        {
            int maxTankNo;
            using (var command = new SQLiteCommand("SELECT COALESCE(MAX(TankNo), 0) FROM Tanks WHERE LineId = 1;", connection, transaction))
            {
                maxTankNo = Convert.ToInt32(command.ExecuteScalar());
            }

            for (var tankNo = maxTankNo + 1; tankNo <= minimumTankNo; tankNo++)
            {
                using (var command = new SQLiteCommand(
                    @"INSERT INTO Tanks (LineId, TankNo, TankNumber, Name, ChemicalName, CapacityLiters, CurrentLevelLiters, TemperatureCelsius, Voltage, CurrentAmps, Status, IsActive)
                      VALUES (1, @TankNo, @TankNumber, @Name, @ChemicalName, @CapacityLiters, @CurrentLevelLiters, @TemperatureCelsius, @Voltage, @CurrentAmps, 'Normal', 1);",
                    connection,
                    transaction))
                {
                    command.Parameters.AddWithValue("@TankNo", tankNo);
                    command.Parameters.AddWithValue("@TankNumber", GetLegacyTankNumber(1, tankNo));
                    command.Parameters.AddWithValue("@Name", "T" + tankNo);
                    command.Parameters.AddWithValue("@ChemicalName", GetDefaultChemicalName(tankNo));
                    command.Parameters.AddWithValue("@CapacityLiters", 1000);
                    command.Parameters.AddWithValue("@CurrentLevelLiters", 750);
                    command.Parameters.AddWithValue("@TemperatureCelsius", 32 + tankNo);
                    command.Parameters.AddWithValue("@Voltage", tankNo == 6 ? 12 : 0);
                    command.Parameters.AddWithValue("@CurrentAmps", tankNo == 6 ? 140 : 0);
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void EnsureMainLineHoists(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(
                @"INSERT INTO Hoists (LineId, HoistName, CurrentTankNo, TargetTankNo, Direction, HomeTank, Status, CurrentJobId, LineNo, IsAuto, PositionIndex, StateTicks)
                  SELECT 1, 'H1', 1, 1, 'LeftToRight', 1, 'Idle', NULL, 1, 1, 0, 0
                  WHERE NOT EXISTS (SELECT 1 FROM Hoists WHERE HoistName = 'H1');",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void KeepSingleMainHoist(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            using (var command = new SQLiteCommand(
                @"DELETE FROM Hoists
                  WHERE HoistId NOT IN (
                      SELECT HoistId
                      FROM Hoists
                      ORDER BY CASE WHEN HoistName = 'H1' THEN 0 ELSE 1 END, HoistId
                      LIMIT 1
                  );",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                @"UPDATE Hoists
                  SET HoistName = 'H1',
                      LineId = 1,
                      LineNo = 1,
                      Direction = 'LeftToRight',
                      FromTank = 1
                  WHERE HoistId IN (SELECT HoistId FROM Hoists ORDER BY HoistId LIMIT 1);",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void NormalizeMainLineHoists(SQLiteConnection connection, SQLiteTransaction transaction)
        {
            int maxTankNo;
            using (var command = new SQLiteCommand("SELECT COALESCE(MAX(TankNo), 1) FROM Tanks WHERE LineId = 1;", connection, transaction))
            {
                maxTankNo = Math.Max(1, Convert.ToInt32(command.ExecuteScalar()));
            }

            using (var command = new SQLiteCommand(
                @"UPDATE Hoists
                  SET LineId = 1,
                      LineNo = 1,
                      CurrentTankNo = CASE
                          WHEN CurrentTankNo IS NULL OR CurrentTankNo <= 0 THEN 1
                          WHEN CurrentTankNo > @MaxTankNo THEN ((CurrentTankNo - 1) % @MaxTankNo) + 1
                          ELSE CurrentTankNo
                      END,
                      TargetTankNo = CASE
                          WHEN TargetTankNo IS NULL OR TargetTankNo <= 0 THEN 1
                          WHEN TargetTankNo > @MaxTankNo THEN ((TargetTankNo - 1) % @MaxTankNo) + 1
                          ELSE TargetTankNo
                      END,
                      HomeTank = CASE
                          WHEN HomeTank IS NULL OR HomeTank <= 0 THEN 1
                          WHEN HomeTank > @MaxTankNo THEN ((HomeTank - 1) % @MaxTankNo) + 1
                          ELSE HomeTank
                      END,
                      FromTank = CASE
                          WHEN FromTank IS NULL OR FromTank <= 0 THEN 1
                          WHEN FromTank > @MaxTankNo THEN ((FromTank - 1) % @MaxTankNo) + 1
                          ELSE FromTank
                      END,
                      ToTank = CASE
                          WHEN ToTank IS NULL OR ToTank <= 0 THEN @MaxTankNo
                          WHEN ToTank > @MaxTankNo THEN @MaxTankNo
                          ELSE ToTank
                      END;",
                connection,
                transaction))
            {
                command.Parameters.AddWithValue("@MaxTankNo", maxTankNo);
                command.ExecuteNonQuery();
            }

            using (var command = new SQLiteCommand(
                @"UPDATE Hoists
                  SET PositionIndex = CurrentTankNo - 1;",
                connection,
                transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private static bool ColumnExists(SQLiteConnection connection, SQLiteTransaction transaction, string tableName, string columnName)
        {
            using (var command = new SQLiteCommand("PRAGMA table_info(" + tableName + ");", connection, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (string.Equals(Convert.ToString(reader["name"]), columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void EnsureColumn(SQLiteConnection connection, SQLiteTransaction transaction, string tableName, string columnName, string definition)
        {
            if (ColumnExists(connection, transaction, tableName, columnName))
            {
                return;
            }

            ExecuteNonQuery(connection, transaction, "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + definition + ";");
        }

        private static int GetLegacyTankNumber(int lineId, int tankNo)
        {
            return (lineId * 1000) + tankNo;
        }

        private static string GetDefaultChemicalName(int tankNo)
        {
            switch (tankNo)
            {
                case 1:
                    return "Loading Station";
                case 2:
                    return "Cleaning Tank";
                case 3:
                    return "Rinse Tank 1";
                case 4:
                    return "Acid Tank";
                case 5:
                    return "Rinse Tank 2";
                case 6:
                    return "Electroplating Tank";
                case 7:
                    return "Rinse Tank 3";
                case 8:
                    return "Drying Tank";
                case 9:
                    return "Unloading Station";
                default:
                    return "Process Chemical";
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
