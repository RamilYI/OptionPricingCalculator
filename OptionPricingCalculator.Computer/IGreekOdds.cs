namespace OptionPricingCalculator.Computer
{
    public interface IGreekOdds
    {
        double Delta();

        double Gamma();

        double Rho();

        double Vega();

        double Theta();
    }
}