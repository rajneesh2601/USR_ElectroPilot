using System;
using System.Collections.Generic;
using System.Linq;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class HoistService
    {
        private const int LiftTransitionTicks = 5;
        private readonly HoistRepository _hoistRepository = new HoistRepository();
        private readonly JobRepository _jobRepository = new JobRepository();
        private readonly TankRepository _tankRepository = new TankRepository();

        public List<HoistModel> GetHoists()
        {
            DatabaseHelper.InitializeDatabase();
            return GetPrimaryHoists(_hoistRepository.GetAll());
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

        public void StopLine(int lineId)
        {
            DatabaseHelper.InitializeDatabase();
            foreach (var hoist in _hoistRepository.GetAll().Where(h => h.LineId == lineId))
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

            var hoists = GetPrimaryHoists(_hoistRepository.GetAll());
            var tanks = _tankRepository.GetAll();
            if (emergencyStop)
            {
                foreach (var hoist in hoists)
                {
                    hoist.Status = "EmergencyStop";
                    _hoistRepository.Update(hoist);
                }

                return;
            }

            AssignJobs(hoists, recipe, jobs, tanks, autoMode);

            foreach (var hoist in hoists)
            {
                TickHoist(hoist, hoists, recipe, tanks, processSecond);
                _hoistRepository.Update(hoist);
            }
        }

        private void AssignJobs(IList<HoistModel> hoists, IList<ProcessStepModel> recipe, IList<JobModel> jobs, IList<TankModel> tanks, bool autoMode)
        {
            if (!autoMode || recipe == null || jobs == null)
            {
                return;
            }

            foreach (var job in jobs.Where(IsAssignableJob))
            {
                if (hoists.Any(h => h.CurrentJobId == job.JobId))
                {
                    continue;
                }

                var step = FindStep(recipe, job.LineId, job.CurrentStep);
                if (step == null)
                {
                    continue;
                }

                if (IsBlockedTank(tanks, step.LineId, step.TankNo))
                {
                    PutJobOnFaultHold(job);
                    continue;
                }

                var hoist = FindBestHoist(hoists, recipe, step.LineId, step.TankNo);
                if (hoist == null)
                {
                    continue;
                }

                hoist.CurrentJobId = job.JobId;
                hoist.LineId = step.LineId;
                hoist.TargetTankNo = step.TankNo;
                hoist.IsAuto = autoMode;
                hoist.Status = hoist.CurrentTankNo == hoist.TargetTankNo ? "Lowering" : "Moving";
                hoist.StateTicks = hoist.Status == "Lowering" ? LiftTransitionTicks : 0;
            }
        }

        private void TickHoist(HoistModel hoist, IList<HoistModel> hoists, IList<ProcessStepModel> recipe, IList<TankModel> tanks, bool processSecond)
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

            var step = FindStep(recipe, job.LineId, job.CurrentStep);
            if (step == null)
            {
                CompleteJob(job, hoist);
                return;
            }

            if (IsBlockedTank(tanks, step.LineId, step.TankNo))
            {
                PutJobOnFaultHold(job);
                ReleaseHoist(hoist);
                return;
            }

            hoist.TargetTankNo = step.TankNo;
            job.LineId = step.LineId;
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
                TickLifting(job, hoist, hoists, recipe, tanks);
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
                hoist.StateTicks = LiftTransitionTicks;
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
            job.LineId = step.LineId;
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
                hoist.StateTicks = LiftTransitionTicks;
                job.Status = "Lifting";
            }

            _jobRepository.Update(job);
        }

        private void TickLifting(JobModel job, HoistModel hoist, IList<HoistModel> hoists, IList<ProcessStepModel> recipe, IList<TankModel> tanks)
        {
            if (hoist.StateTicks > 0)
            {
                hoist.StateTicks--;
                job.Status = "Lifting";
                _jobRepository.Update(job);
                return;
            }

            var direction = job.StepDirection == 0 ? 1 : job.StepDirection;
            var nextStep = FindStep(recipe, job.LineId, job.CurrentStep + direction);
            job.RemainingSeconds = 0;

            if (nextStep == null)
            {
                direction = direction > 0 ? -1 : 1;
                nextStep = FindStep(recipe, job.LineId, job.CurrentStep + direction);
                if (nextStep == null)
                {
                    hoist.Status = "Idle";
                    hoist.CurrentJobId = null;
                    job.Status = "Queued";
                    job.StepDirection = direction;
                    _jobRepository.Update(job);
                    return;
                }
            }

            if (IsBlockedTank(tanks, nextStep.LineId, nextStep.TankNo))
            {
                PutJobOnFaultHold(job);
                ReleaseHoist(hoist);
                return;
            }

            if (ShouldTransferToAnotherHoist(hoist, hoists, recipe, nextStep))
            {
                job.CurrentStep = nextStep.StepNo;
                job.StepDirection = direction;
                job.LineId = nextStep.LineId;
                job.CurrentTank = nextStep.TankNo;
                job.Status = "Queued";
                job.RemainingSeconds = 0;
                _jobRepository.Update(job);
                ReleaseHoist(hoist);
                return;
            }

            job.CurrentStep = nextStep.StepNo;
            job.StepDirection = direction;
            job.LineId = nextStep.LineId;
            job.CurrentTank = nextStep.TankNo;
            job.Status = "Moving";
            hoist.TargetTankNo = nextStep.TankNo;
            hoist.Status = hoist.CurrentTankNo == hoist.TargetTankNo ? "Lowering" : "Moving";
            hoist.StateTicks = hoist.Status == "Lowering" ? LiftTransitionTicks : 0;
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

        private static ProcessStepModel FindStep(IList<ProcessStepModel> recipe, int lineId, int stepNo)
        {
            return recipe == null ? null : recipe.FirstOrDefault(s => s.LineId == lineId && s.StepNo == stepNo);
        }

        private static bool IsAssignableJob(JobModel job)
        {
            if (job == null)
            {
                return false;
            }

            return !string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(job.Status, "Paused", StringComparison.OrdinalIgnoreCase);
        }

        public int AddHoist(HoistModel hoist)
        {
            DatabaseHelper.InitializeDatabase();
            return _hoistRepository.Add(hoist);
        }

        public void UpdateHoist(HoistModel hoist)
        {
            DatabaseHelper.InitializeDatabase();
            _hoistRepository.Update(hoist);
        }

        public void DeleteHoist(int hoistId)
        {
            DatabaseHelper.InitializeDatabase();
            _hoistRepository.Delete(hoistId);
        }

        private static HoistModel FindBestHoist(IList<HoistModel> hoists, IList<ProcessStepModel> recipe, int lineId, int tankNo)
        {
            var idleHoists = hoists
                .Where(h => !h.CurrentJobId.HasValue && !string.Equals(h.Status, "Fault", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var lineHoists = idleHoists
                .Where(h => h.LineId == lineId)
                .ToList();

            return FindBestHoistFromCandidates(lineHoists, recipe, lineId, tankNo);
        }

        private static bool ShouldTransferToAnotherHoist(HoistModel currentHoist, IList<HoistModel> hoists, IList<ProcessStepModel> recipe, ProcessStepModel nextStep)
        {
            return false;
        }

        private static List<HoistModel> GetPrimaryHoists(IEnumerable<HoistModel> hoists)
        {
            if (hoists == null)
            {
                return new List<HoistModel>();
            }

            return hoists
                .Where(hoist => hoist.LineId <= 0 || hoist.LineId == 1)
                .OrderBy(hoist => string.Equals(hoist.HoistName, "H1", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(hoist => hoist.HoistId)
                .Take(1)
                .ToList();
        }

        private static HoistModel FindBestHoistFromCandidates(IList<HoistModel> candidates, IList<ProcessStepModel> recipe, int lineId, int tankNo)
        {
            if (candidates == null || candidates.Count == 0)
            {
                return null;
            }

            var lineHoists = candidates.Where(h => h.LineId == lineId).ToList();
            if (lineHoists.Count == 0)
            {
                return null;
            }

            var maxTankNo = GetLineMaxTankNo(recipe, lineId, tankNo);
            var zoned = lineHoists.Where(h => IsTankInHoistZone(h, tankNo, maxTankNo, lineHoists.Count)).ToList();
            var source = zoned.Count > 0 ? zoned : lineHoists;

            return source
                .OrderBy(h => GetHoistDirectionPenalty(h, tankNo, maxTankNo, lineHoists.Count))
                .ThenBy(h => Math.Abs(h.CurrentTankNo - tankNo))
                .ThenBy(h => Math.Abs(h.HomeTank - tankNo))
                .ThenBy(h => h.HoistName)
                .FirstOrDefault();
        }

        private static bool IsTankInHoistZone(HoistModel hoist, int tankNo, int maxTankNo, int hoistCount)
        {
            var fromTank = GetHoistFromTank(hoist);
            var toTank = GetHoistToTank(hoist, maxTankNo);
            if (toTank >= fromTank)
            {
                return tankNo >= fromTank && tankNo <= toTank;
            }

            if (hoistCount <= 1 || maxTankNo <= 1)
            {
                return true;
            }

            var middle = Convert.ToInt32(Math.Ceiling(maxTankNo / 2.0));
            if (string.Equals(hoist.Direction, "RightToLeft", StringComparison.OrdinalIgnoreCase))
            {
                return tankNo > middle || hoist.HomeTank <= tankNo;
            }

            if (string.Equals(hoist.Direction, "LeftToRight", StringComparison.OrdinalIgnoreCase))
            {
                return tankNo <= middle || hoist.HomeTank >= tankNo;
            }

            return true;
        }

        private static int GetHoistDirectionPenalty(HoistModel hoist, int tankNo, int maxTankNo, int hoistCount)
        {
            return IsTankInHoistZone(hoist, tankNo, maxTankNo, hoistCount) ? 0 : 1000;
        }

        private static int GetHoistFromTank(HoistModel hoist)
        {
            if (hoist == null || hoist.FromTank <= 0)
            {
                return 1;
            }

            return hoist.FromTank;
        }

        private static int GetHoistToTank(HoistModel hoist, int maxTankNo)
        {
            if (hoist == null)
            {
                return Math.Max(1, maxTankNo);
            }

            if (hoist.ToTank > 0)
            {
                return hoist.ToTank;
            }

            if (hoist.HomeTank > 0)
            {
                return hoist.HomeTank;
            }

            return Math.Max(1, maxTankNo);
        }

        private static int GetLineMaxTankNo(IList<ProcessStepModel> recipe, int lineId, int fallbackTankNo)
        {
            if (recipe == null)
            {
                return Math.Max(1, fallbackTankNo);
            }

            return Math.Max(Math.Max(1, fallbackTankNo), recipe.Where(step => step.LineId == lineId).Select(step => step.TankNo).DefaultIfEmpty(fallbackTankNo).Max());
        }

        private static bool IsBlockedTank(IList<TankModel> tanks, int lineId, int tankNo)
        {
            if (tanks == null)
            {
                return false;
            }

            var tank = tanks.FirstOrDefault(t => t.LineId == lineId && t.TankNo == tankNo);
            if (tank == null)
            {
                return false;
            }

            return string.Equals(tank.Status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(tank.Status, "Warning", StringComparison.OrdinalIgnoreCase);
        }

        private void PutJobOnFaultHold(JobModel job)
        {
            if (job == null)
            {
                return;
            }

            job.Status = "Paused";
            job.RemainingSeconds = 0;
            _jobRepository.Update(job);
        }

        private static void ReleaseHoist(HoistModel hoist)
        {
            if (hoist == null)
            {
                return;
            }

            hoist.CurrentJobId = null;
            hoist.Status = "Idle";
            hoist.StateTicks = 0;
        }
    }
}
