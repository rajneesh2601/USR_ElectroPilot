using System;
using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class JobService
    {
        private readonly JobRepository _jobRepository = new JobRepository();

        public List<JobModel> GetActiveJobs()
        {
            DatabaseHelper.InitializeDatabase();
            return _jobRepository.GetActive();
        }

        public JobModel StartJob(IList<ProcessStepModel> recipe)
        {
            DatabaseHelper.InitializeDatabase();

            var activeJobs = _jobRepository.GetActive();
            if (activeJobs.Count > 0)
            {
                return activeJobs[0];
            }

            var firstStep = recipe != null && recipe.Count > 0 ? recipe[0] : null;
            var job = new JobModel
            {
                JobNumber = "JOB-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                CurrentStep = firstStep == null ? 0 : firstStep.StepNo,
                CurrentTank = firstStep == null ? 0 : firstStep.TankNo,
                Status = "Queued",
                RemainingSeconds = 0,
                StartedAt = DateTime.Now
            };

            job.JobId = _jobRepository.Add(job);
            return job;
        }

        public void Save(JobModel job)
        {
            DatabaseHelper.InitializeDatabase();
            _jobRepository.Update(job);
        }
    }
}
