using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionPricingCalculator.Computer
{
    public static class Payoff
    {
        public static double[][] PayOff(List<Tuple<double, double[]>> mcPriceMatrix, double strike, string optionType, bool isParallel = true)
        {
            return GenerateMcPayOffValues(mcPriceMatrix, strike, optionType, isParallel);
        }

        private static double[][] GenerateMcPayOffValues(List<Tuple<double, double[]>> mcPriceMatrix, double strike, string optionType, bool isParallel)
        {
            var MCPayOff = new double[mcPriceMatrix.Count][];
            double sign = optionType == "put" ? 1.0 : -1.0;

            for (var i = 0; i < mcPriceMatrix.Count; i++)
            {
                var bufferMatrix = mcPriceMatrix[i];
                MCPayOff[i] = new double[mcPriceMatrix[0].Item2.Length];
                if (isParallel)
                {
                    Parallel.For(0, bufferMatrix.Item2.Length,
                        (k) => { MCPayOff[i][k] = Math.Max(sign * (strike - bufferMatrix.Item2[k]), 0.0); });
                }
                else
                {
                    for (int k = 0; k < bufferMatrix.Item2.Length; k++)
                    {
                        MCPayOff[i][k] = Math.Max(sign * (strike - bufferMatrix.Item2[k]), 0.0);
                    }
                }
            }

            return MCPayOff;
        }
    }
}
