using System;
using System.Collections.Generic;
using System.Linq;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class HoistService
    {
        private readonly HoistRepository _hoistRepository = new HoistRepository();
        private readonly JobRepository _jobRepository = new JobRepository();

        public List<HoistModel> GetHoists()
        {
            DatabaseHelper.InitializeDatabase();
            return _hoistRepository.GetAll();
        }

        public void StopAll()
        {
            DatabaseHelper.InitializeDatabase();
            foreach (var hoist in _hoistRepository.GetAll())
            {
                hoist.Status = "Idle";
                hoist.CurrentJobId = null;
                hoist.StateTicks = 0;
                _hoistRepository.Update(hoist);
            }
        }

        public void EmergencyStop()
        {
            DatabaseHelper.InitializeDatabase();
            foreach (var hoist in _hoistRepository.GetAll())
            {
                hoist.Status = "EmergencyStop";
                hoist.StateTicks = 0;
                _hoistRepository.Update(hoist);
            }
        }

        public void Tick(IList<ProcessStepModel> recipe, IList<JobModel> jobs, bool autoMode, bool emergencyStop, bool processSecond)
        {
            DatabaseHelper.InitializeDatabase();

            var hoists = _hoistRepository.GetAll();
            if (emergencyStop)
            {
                foreach (var hoist in hoists)
                {
                    hoist.Status = "EmergencyStop";
                    _hoistRepository.Update(hoist);
                }

                return;
            }

            AssignJobs(hoists, recipe, jobs, autoMode);

            foreach (var hoist in hoists)
            {
                TickHoist(hoist, recipe, processSecond);
                _hoistRepository.Update(hoist);
            }
        }

        private void AssignJobs(IList<HoistModel> hoists, IList<ProcessStepModel> recipe, IList<JobModel> jobs, bool autoMode)
        {
            if (!autoMode || recipe == null || jobs == null)
            {
                return;
            }

            foreach (var job in jobs.Where(j => !string.Equals(j.Status, "Complete", StringComparison.OrdinalIgnoreCase)))
            {
                if (hoists.Any(h => h.CurrentJobId == job.JobId))
                {
                    continue;
                }

                var step = FindStep(recipe, job.CurrentStep);
                if (step == null)
                {
                    continue;
                }

                var hoist = FindBestHoist(hoists, step.TankNo);
                if (hoist == null)
                {
                    continue;
                }

                hoist.CurrentJobId = job.JobId;
                hoist.TargetTankNo = step.TankNo;
                hoist.IsAuto = autoMode;
                hoist.Status = hoist.CurrentTankNo == hoist.TargetTankNo ? "Lowering" : "Moving";
                hoist.StateTicks = hoist.Status == "Lowering" ? 2 : 0;
            }
        }

        private void TickHoist(HoistModel hoist, IList<ProcessStepModel> recipe, bool processSecond)
        {
            if (!hoist.CurrentJobId.HasValue)
            {
                if (!string.Equals(hoist.Status, "EmergencyStop", StringComparison.OrdinalIgnoreCase))
                {
                    hoist.Status = "Idle";
                }

                return;
            }

            var job = _jobRepository.GetActive().FirstOrDefault(j => j.JobId == hoist.CurrentJobId.Value);
            if (job == null)
            {
                hoist.CurrentJobId = null;
                hoist.Status = "Idle";
                return;
            }

            var step = FindStep(recipe, job.CurrentStep);
            if (step == null)
            {
                CompleteJob(job, hoist);
                return;
            }

            hoist.TargetTankNo = step.TankNo;
            job.CurrentTank = hoist.CurrentTankNo;

            if (string.Equals(hoist.Status, "Moving", StringComparison.OrdinalIgnoreCase))
            {
                MoveHoist(hoist);
                job.Status = "Moving";
                job.CurrentTank = hoist.CurrentTankNo;
                _jobRepository.Update(job);
                return;
            }

            if (string.Equals(hoist.Status, "Lowering", StringComparison.OrdinalIgnoreCase))
            {
                TickStateDelay(hoist, "Processing");
                job.Status = "Lowering";
                _jobRepository.Update(job);
                return;
            }

            if (string.Equals(hoist.Status, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                ProcessJob(job, hoist, step, processSecond);
                return;
            }

            if (string.Equals(hoist.Status, "Lifting", StringComparison.OrdinalIgnoreCase))
            {
                TickLifting(job, hoist, recipe);
                return;
            }

            hoist.Status = hoist.CurrentTankNo == hoist.TargetTankNo ? "Lowering" : "Moving";
        }

        private static void MoveHoist(HoistModel hoist)
        {
            var targetIndex = Math.Max(0, hoist.TargetTankNo - 1);
            var distance = targetIndex - hoist.PositionIndex;
            var step = 0.35;

            if (Math.Abs(distance) <= step)
            {
                hoist.PositionIndex = targetIndex;
                hoist.CurrentTankNo = hoist.TargetTankNo;
                hoist.Status = "Lowering";
                hoist.StateTicks = 2;
                return;
            }

            hoist.PositionIndex += distance > 0 ? step : -step;
            hoist.Status = "Moving";
        }

        private static void TickStateDelay(HoistModel hoist, string nextStatus)
        {
            if (hoist.StateTicks > 0)
            {
                hoist.StateTicks--;
                return;
            }

            hoist.Status = nextStatus;
        }

        private void ProcessJob(JobModel job, HoistModel hoist, ProcessStepModel step, bool processSecond)
        {
            job.CurrentTank = step.TankNo;
            job.Status = "Processing";
            if (job.RemainingSeconds <= 0)
            {
                job.RemainingSeconds = step.DurationSeconds;
            }
            else if (processSecond)
            {
                job.RemainingSeconds--;
            }

            if (job.RemainingSeconds <= 0)
            {
                hoist.Status = "Lifting";
                hoist.StateTicks = 2;
                job.Status = "Lifting";
            }

            _jobRepository.Update(job);
        }

        private void TickLifting(JobModel job, HoistModel hoist, IList<ProcessStepModel> recipe)
        {
            if (hoist.StateTicks > 0)
            {
                hoist.StateTicks--;
                job.Status = "Lifting";
                _jobRepository.Update(job);
                return;
            }

            var nextStep = FindStep(recipe, job.CurrentStep + 1);
            job.RemainingSeconds = 0;

            if (nextStep == null)
            {
                CompleteJob(job, hoist);
                return;
            }

            job.CurrentStep = nextStep.StepNo;
            job.CurrentTank = nextStep.TankNo;
            job.Status = "Moving";
            hoist.TargetTankNo = nextStep.TankNo;
            hoist.Status = hoist.CurrentTankNo == hoist.TargetTankNo ? "Lowering" : "Moving";
            hoist.StateTicks = hoist.Status == "Lowering" ? 2 : 0;
            _jobRepository.Update(job);
        }

        private void CompleteJob(JobModel job, HoistModel hoist)
        {
            job.Status = "Complete";
            job.CompletedAt = DateTime.Now;
            job.RemainingSeconds = 0;
            _jobRepository.Update(job);

            hoist.CurrentJobId = null;
            hoist.Status = "Idle";
            hoist.StateTicks = 0;
        }

        private static ProcessStepModel FindStep(IList<ProcessStepModel> recipe, int stepNo)
        {
            return recipe == null ? null : recipe.FirstOrDefault(s => s.StepNo == stepNo);
        }

        private static HoistModel FindBestHoist(IList<HoistModel> hoists, int tankNo)
        {
            var lineNo = tankNo <= 5 ? 1 : 2;
            var idleHoists = hoists
                .Where(h => !h.CurrentJobId.HasValue && !string.Equals(h.Status, "Fault", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return idleHoists
                .Where(h => h.LineNo == lineNo)
                .OrderBy(h => Math.Abs(h.CurrentTankNo - tankNo))
                .FirstOrDefault()
                ?? idleHoists.OrderBy(h => Math.Abs(h.CurrentTankNo - tankNo)).FirstOrDefault();
        }
    }
}
