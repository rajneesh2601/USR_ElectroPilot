using System.Collections.Generic;
using System.Linq;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class ProcessRecipeService
    {
        private readonly ProcessStepRepository _processStepRepository = new ProcessStepRepository();
        private readonly TankRepository _tankRepository = new TankRepository();

        public List<ProcessStepModel> GetRecipeSteps()
        {
            DatabaseHelper.InitializeDatabase();
            var explicitSteps = NormalizePerLine(_processStepRepository.GetActive());
            var configuredTanks = _tankRepository.GetAll();
            var recipeLines = new HashSet<int>(explicitSteps.Select(s => s.LineId));

            foreach (var tankGroup in configuredTanks.GroupBy(t => t.LineId).OrderBy(g => g.Key))
            {
                if (recipeLines.Contains(tankGroup.Key))
                {
                    continue;
                }

                var stepNo = 1;
                foreach (var tank in tankGroup.OrderBy(t => t.TankNo))
                {
                    explicitSteps.Add(new ProcessStepModel
                    {
                        StepNo = stepNo++,
                        LineId = tank.LineId,
                        TankId = tank.Id,
                        TankNo = tank.TankNo,
                        StepName = tank.ChemicalName,
                        ProcessName = tank.ChemicalName,
                        DurationSeconds = 10,
                        IsActive = true
                    });
                }
            }

            return explicitSteps
                .OrderBy(s => s.LineId)
                .ThenBy(s => s.StepNo)
                .ToList();
        }

        private static List<ProcessStepModel> NormalizePerLine(IList<ProcessStepModel> steps)
        {
            var normalized = new List<ProcessStepModel>();
            if (steps == null)
            {
                return normalized;
            }

            foreach (var group in steps.GroupBy(s => s.LineId).OrderBy(g => g.Key))
            {
                var stepNo = 1;
                foreach (var step in group.OrderBy(s => s.StepNo))
                {
                    normalized.Add(new ProcessStepModel
                    {
                        StepId = step.StepId,
                        StepNo = stepNo++,
                        LineId = step.LineId,
                        TankId = step.TankId,
                        TankNo = step.TankNo,
                        StepName = step.StepName,
                        ProcessName = step.ProcessName,
                        DurationSeconds = step.DurationSeconds,
                        IsActive = step.IsActive
                    });
                }
            }

            return normalized;
        }
    }
}
