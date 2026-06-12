using System;

namespace USR_ElectroPilot.Helpers
{
    public static class RandomGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lock = new object();

        public static double NextDouble(double minValue, double maxValue)
        {
            lock (_lock)
            {
                return minValue + (_random.NextDouble() * (maxValue - minValue));
            }
        }

        public static int NextInt(int minValue, int maxValue)
        {
            lock (_lock)
            {
                return _random.Next(minValue, maxValue);
            }
        }

        public static bool Chance(double probability)
        {
            lock (_lock)
            {
                return _random.NextDouble() < probability;
            }
        }
    }
}
