namespace OptionPricingCalculator.Models.Interfaces
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
    }
}