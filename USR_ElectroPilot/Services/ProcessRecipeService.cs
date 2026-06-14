using System.Collections.Generic;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class ProcessRecipeService
    {
        private readonly ProcessStepRepository _processStepRepository = new ProcessStepRepository();

        public List<ProcessStepModel> GetRecipeSteps()
        {
            DatabaseHelper.InitializeDatabase();
            return _processStepRepository.GetActive();
        }
    }
}
