namespace OptionPricingCalculator.Models
{
    public class OptionParameters
    {

        public int Paths { get; set; }

        public double Price { get; set; }

        public double Delta { get; set; }

        public double Gamma { get; set; }

        public double Vega { get; set; }

        public double Rho { get; set; }

        public double Theta { get; set; }

    }
}
