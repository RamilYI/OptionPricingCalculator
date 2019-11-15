using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OptionPricingCalculator.Models
{
    [Serializable]
    public class OptionModel
    {
        public string OptionType { get; set; }

        public double InitialStock { get; set; }

        public double Volatility { get; set; }

        public double StrikeValue { get; set; }

        public double MaturityTime { get; set; }

        public double RiskFreeInterestRate { get; set; }

        public double DividendYield { get; set; }

        public double NumberOfAssets { get; set; }

        [field: NonSerialized]
        public string CalculationDuration { get; set; }

        [field: NonSerialized]
        public string Status { get; set; }

    }
}