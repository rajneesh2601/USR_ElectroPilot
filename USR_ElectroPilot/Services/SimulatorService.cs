using System.Collections.Generic;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class SimulatorService
    {
        public void SimulateTanks(IList<TankModel> tanks)
        {
            if (tanks == null)
            {
                return;
            }

            foreach (var tank in tanks)
            {
                tank.CurrentLevelLiters = Clamp(tank.CurrentLevelLiters + RandomGenerator.NextDouble(-5, 5), 0, tank.CapacityLiters);
                tank.TemperatureCelsius = Clamp(tank.TemperatureCelsius + RandomGenerator.NextDouble(-0.3, 0.3), 15, 80);
                tank.Voltage = Clamp(tank.Voltage + RandomGenerator.NextDouble(-0.2, 0.2), 0, 24);
                tank.CurrentAmps = Clamp(tank.CurrentAmps + RandomGenerator.NextDouble(-2, 2), 0, 500);
                tank.Status = RandomGenerator.Chance(0.02) ? Constants.StatusWarning : Constants.StatusNormal;
            }
        }

        public void SimulateRectifiers(IList<RectifierModel> rectifiers)
        {
            if (rectifiers == null)
            {
                return;
            }

            foreach (var rectifier in rectifiers)
            {
                rectifier.Voltage = Clamp(rectifier.Voltage + RandomGenerator.NextDouble(-0.5, 0.5), 0, 24);
                rectifier.CurrentAmps = Clamp(rectifier.CurrentAmps + RandomGenerator.NextDouble(-5, 5), 0, 500);
                rectifier.FaultCode = RandomGenerator.Chance(0.01) ? "FLT" : null;
            }
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}
