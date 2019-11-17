using System.ComponentModel;

namespace OptionPricingCalculator.Common.Settings.Interfaces
{
    public interface IEnvironmentSettings
    {
        int SimulationNumbers { get; set; }

        int GridForTime { get; set; }

        bool IsDeltaEnabled { get; set; }

        bool IsGammaEnabled { get; set; }

        bool IsVegaEnabled { get; set; }

        bool IsRhoEnabled { get; set; }

        bool IsThetaEnabled { get; set; }

        bool IsParallel { get; set; }

        int NumberOfPath { get; set; }

        int PolynomialDegree { get; set; }

        string StochasticProcessName { get; set; }

        double JumpLambda { get; set; }

        double JumpLambdaSize { get; set; }

        double JumpLambdaStd { get; set; }

        int TimeIntervals { get; set; }
    }
}