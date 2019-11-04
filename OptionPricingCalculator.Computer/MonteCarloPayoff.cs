using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionPricingCalculator.Computer
{
    public static class MonteCarloPayoff
    {
        public static double[,] MonteCarloPayOff(List<Tuple<double, double[]>> mcPriceMatrix, double strike, string optionType)
        {
            return GenerateMcPayOffValues(mcPriceMatrix, strike, optionType);
        }

        private static double[,] GenerateMcPayOffValues(List<Tuple<double, double[]>> mcPriceMatrix, double strike, string optionType)
        {
            var MCPayOff = new double[mcPriceMatrix.Count, mcPriceMatrix[0].Item2.Length];
            double sign = optionType == "put" ? 1.0 : -1.0;

            for (var i = 0; i < mcPriceMatrix.Count; i++)
            {
                var bufferMatrix = mcPriceMatrix[i];

                Parallel.For(0, bufferMatrix.Item2.Length,
                    (k) => { MCPayOff[i, k] = Math.Max(sign * (strike - bufferMatrix.Item2[k]), 0.0); });
            }

            return MCPayOff;
        }
    }
}
