using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class LoadRepository
    {
        public List<LoadModel> GetAll()
        {
            var loads = new List<LoadModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Loads ORDER BY CreatedAt DESC;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loads.Add(new LoadModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            LoadNumber = Convert.ToString(reader["LoadNumber"]),
                            RecipeId = ReadNullableInt(reader, "RecipeId"),
                            WagonId = ReadNullableInt(reader, "WagonId"),
                            Status = Convert.ToString(reader["Status"]),
                            StartedAt = ReadNullableDate(reader, "StartedAt"),
                            CompletedAt = ReadNullableDate(reader, "CompletedAt"),
                            CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
                        });
                    }
                }
            }

            return loads;
        }

        public int Add(LoadModel load)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Loads (LoadNumber, RecipeId, WagonId, Status)
                  VALUES (@LoadNumber, @RecipeId, @WagonId, @Status);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@LoadNumber", load.LoadNumber);
                command.Parameters.AddWithValue("@RecipeId", load.RecipeId.HasValue ? (object)load.RecipeId.Value : DBNull.Value);
                command.Parameters.AddWithValue("@WagonId", load.WagonId.HasValue ? (object)load.WagonId.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Status", load.Status);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private static int? ReadNullableInt(SQLiteDataReader reader, string name)
        {
            return reader[name] == DBNull.Value ? (int?)null : Convert.ToInt32(reader[name]);
        }

        private static DateTime? ReadNullableDate(SQLiteDataReader reader, string name)
        {
            return reader[name] == DBNull.Value ? (DateTime?)null : DateTime.Parse(Convert.ToString(reader[name]));
        }
    }
}
