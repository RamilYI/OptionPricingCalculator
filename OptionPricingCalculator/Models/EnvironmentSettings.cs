using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionPricingCalculator.Models.Interfaces;

namespace OptionPricingCalculator.Models
{
    public sealed class EnvironmentSettings : IEnvironmentSettings
    {
        public string[] MonteCarloAlgorithms { get; set; }

        public int SimulationNumbers { get; set; }

        public int GridForTime { get; set; }

        public bool IsDeltaEnabled { get; set; }

        public bool IsGammaEnabled { get; set; }

        public bool IsVegaEnabled { get; set; }

        public bool IsRhoEnabled { get; set; }

        public bool IsThetaEnabled { get; set; }

        public bool IsParallel { get; set; }

        private static readonly Lazy<EnvironmentSettings> lazy = new Lazy<EnvironmentSettings>(() => new EnvironmentSettings()
        {
            MonteCarloAlgorithms = new[] { "Метод наименьших квадратов" },
            SimulationNumbers = 200000,
            GridForTime = 50,
            IsDeltaEnabled = true,
            IsGammaEnabled = true,
            IsRhoEnabled = true,
            IsVegaEnabled = true,
            IsThetaEnabled = true,
            IsParallel = true,
        });

        public static EnvironmentSettings Instance => lazy.Value;

        private EnvironmentSettings()
        {
        }
    }
}
