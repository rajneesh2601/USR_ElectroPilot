using System;
using System.Collections.Generic;
using System.Linq;
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
            var firstStep = recipe != null && recipe.Count > 0 ? recipe[0] : null;
            return StartLineJob(recipe, firstStep == null ? 1 : firstStep.LineId);
        }

        public JobModel StartLineJob(IList<ProcessStepModel> recipe, int lineId)
        {
            DatabaseHelper.InitializeDatabase();

            var activeJob = _jobRepository.GetActive()
                .FirstOrDefault(j => j.LineId == lineId && !string.Equals(j.Status, "Complete", StringComparison.OrdinalIgnoreCase));
            if (activeJob != null)
            {
                if (string.Equals(activeJob.Status, "Paused", StringComparison.OrdinalIgnoreCase))
                {
                    activeJob.Status = "Queued";
                    _jobRepository.Update(activeJob);
                }

                return activeJob;
            }

            var firstStep = recipe == null
                ? null
                : recipe.Where(s => s.LineId == lineId).OrderBy(s => s.StepNo).FirstOrDefault();
            var job = new JobModel
            {
                JobNumber = "L" + lineId + "-JOB-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                LineId = lineId,
                CurrentStep = firstStep == null ? 0 : firstStep.StepNo,
                StepDirection = 1,
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

        public void PauseAllActiveJobs()
        {
            DatabaseHelper.InitializeDatabase();

            foreach (var job in _jobRepository.GetActive())
            {
                if (string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                job.Status = "Paused";
                job.RemainingSeconds = 0;
                _jobRepository.Update(job);
            }
        }

        public void PauseLineJob(int lineId)
        {
            DatabaseHelper.InitializeDatabase();

            foreach (var job in _jobRepository.GetActive().Where(j => j.LineId == lineId))
            {
                if (string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                job.Status = "Paused";
                job.RemainingSeconds = 0;
                _jobRepository.Update(job);
            }
        }

        public void StopAllActiveJobs()
        {
            DatabaseHelper.InitializeDatabase();

            foreach (var job in _jobRepository.GetActive())
            {
                if (string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                job.Status = "Complete";
                job.RemainingSeconds = 0;
                job.CompletedAt = DateTime.Now;
                _jobRepository.Update(job);
            }
        }

        public void StopLineActiveJobs(int lineId)
        {
            DatabaseHelper.InitializeDatabase();

            foreach (var job in _jobRepository.GetActive().Where(j => j.LineId == lineId))
            {
                if (string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                job.Status = "Complete";
                job.RemainingSeconds = 0;
                job.CompletedAt = DateTime.Now;
                _jobRepository.Update(job);
            }
        }

        public bool HasRunningLineJob(int lineId)
        {
            DatabaseHelper.InitializeDatabase();

            return _jobRepository.GetActive().Any(job =>
                job.LineId == lineId &&
                !string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(job.Status, "Paused", StringComparison.OrdinalIgnoreCase));
        }
    }
}
