using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class RecipeRepository
    {
        public List<RecipeModel> GetAll()
        {
            var recipes = new List<RecipeModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand("SELECT * FROM Recipes ORDER BY Name;", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recipes.Add(new RecipeModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = Convert.ToString(reader["Name"]),
                            Description = reader["Description"] == DBNull.Value ? null : Convert.ToString(reader["Description"]),
                            TargetVoltage = Convert.ToDouble(reader["TargetVoltage"]),
                            TargetCurrentAmps = Convert.ToDouble(reader["TargetCurrentAmps"]),
                            DurationMinutes = Convert.ToInt32(reader["DurationMinutes"]),
                            IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                            CreatedAt = DateTime.Parse(Convert.ToString(reader["CreatedAt"]))
                        });
                    }
                }
            }

            return recipes;
        }

        public int Add(RecipeModel recipe)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Recipes (Name, Description, TargetVoltage, TargetCurrentAmps, DurationMinutes, IsActive)
                  VALUES (@Name, @Description, @TargetVoltage, @TargetCurrentAmps, @DurationMinutes, @IsActive);
                  SELECT last_insert_rowid();",
                connection))
            {
                command.Parameters.AddWithValue("@Name", recipe.Name);
                command.Parameters.AddWithValue("@Description", (object)recipe.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@TargetVoltage", recipe.TargetVoltage);
                command.Parameters.AddWithValue("@TargetCurrentAmps", recipe.TargetCurrentAmps);
                command.Parameters.AddWithValue("@DurationMinutes", recipe.DurationMinutes);
                command.Parameters.AddWithValue("@IsActive", recipe.IsActive ? 1 : 0);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
