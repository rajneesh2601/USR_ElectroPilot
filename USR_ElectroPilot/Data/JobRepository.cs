using System;
using System.Collections.Generic;
using System.Data.SQLite;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Data
{
    public class JobRepository
    {
        public List<JobModel> GetActive()
        {
            return Query("SELECT * FROM Jobs WHERE Status <> 'Complete' ORDER BY JobId;");
        }

        public int Add(JobModel job)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"INSERT INTO Jobs (JobNumber, CurrentStep, CurrentTank, Status, RemainingSeconds, StartedAt, CompletedAt)
                  VALUES (@JobNumber, @CurrentStep, @CurrentTank, @Status, @RemainingSeconds, @StartedAt, @CompletedAt);
                  SELECT last_insert_rowid();",
                connection))
            {
                AddParameters(command, job);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void Update(JobModel job)
        {
            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(
                @"UPDATE Jobs
                  SET JobNumber = @JobNumber,
                      CurrentStep = @CurrentStep,
                      CurrentTank = @CurrentTank,
                      Status = @Status,
                      RemainingSeconds = @RemainingSeconds,
                      StartedAt = @StartedAt,
                      CompletedAt = @CompletedAt
                  WHERE JobId = @JobId;",
                connection))
            {
                AddParameters(command, job);
                command.Parameters.AddWithValue("@JobId", job.JobId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static List<JobModel> Query(string sql)
        {
            var jobs = new List<JobModel>();

            using (var connection = SqliteConnectionFactory.CreateConnection())
            using (var command = new SQLiteCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jobs.Add(MapJob(reader));
                    }
                }
            }

            return jobs;
        }

        private static void AddParameters(SQLiteCommand command, JobModel job)
        {
            command.Parameters.AddWithValue("@JobNumber", job.JobNumber);
            command.Parameters.AddWithValue("@CurrentStep", job.CurrentStep);
            command.Parameters.AddWithValue("@CurrentTank", job.CurrentTank);
            command.Parameters.AddWithValue("@Status", job.Status);
            command.Parameters.AddWithValue("@RemainingSeconds", job.RemainingSeconds);
            command.Parameters.AddWithValue("@StartedAt", job.StartedAt.ToString("s"));
            command.Parameters.AddWithValue("@CompletedAt", job.CompletedAt.HasValue ? (object)job.CompletedAt.Value.ToString("s") : DBNull.Value);
        }

        private static JobModel MapJob(SQLiteDataReader reader)
        {
            return new JobModel
            {
                JobId = Convert.ToInt32(reader["JobId"]),
                JobNumber = Convert.ToString(reader["JobNumber"]),
                CurrentStep = Convert.ToInt32(reader["CurrentStep"]),
                CurrentTank = Convert.ToInt32(reader["CurrentTank"]),
                Status = Convert.ToString(reader["Status"]),
                RemainingSeconds = Convert.ToInt32(reader["RemainingSeconds"]),
                StartedAt = DateTime.Parse(Convert.ToString(reader["StartedAt"])),
                CompletedAt = reader["CompletedAt"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(Convert.ToString(reader["CompletedAt"]))
            };
        }
    }
}
