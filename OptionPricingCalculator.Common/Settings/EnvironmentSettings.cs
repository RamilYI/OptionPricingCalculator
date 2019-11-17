using System;
using OptionPricingCalculator.Common.Settings.Interfaces;

namespace OptionPricingCalculator.Common.Settings
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

        public int NumberOfPath { get; set; }

        public int PolynomialDegree { get; set; }

        public string StochasticProcessName { get; set; }
        
        public double JumpLambda { get; set; }
        
        public double JumpLambdaSize { get; set; }
        
        public double JumpLambdaStd { get; set; }
        
        public int TimeIntervals { get; set; }

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
            NumberOfPath = 100,
            PolynomialDegree = 6,
            StochasticProcessName = "Геометрическое броуновское движение",
            JumpLambda = 1.0,
            JumpLambdaSize = -0.2,
            JumpLambdaStd = 0.2,
            TimeIntervals = 100,
        });

        public static EnvironmentSettings Instance => lazy.Value;

        private EnvironmentSettings()
        {
        }
    }
}
