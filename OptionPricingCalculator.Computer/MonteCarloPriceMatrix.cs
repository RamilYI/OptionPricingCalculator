using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace OptionPricingCalculator.Computer
{
    public static class MonteCarloPriceMatrix
    {
        public static List<Tuple<double, double[]>> MonteCarloCreator(double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, double T, double initialStock)
        {
            var timeUnit = T / gridForTime;
            var random = new MersenneTwister();
            var mcPriceMatrix = new List<Tuple<double, double[]>>
            {
                new Tuple<double, double[]>(0, Enumerable.Repeat(initialStock, simulations).ToArray())
            };


            GenerateMC_PriceMatrixValues(gridForTime, volatility, riskFreeOptionPrice, simulations, random, timeUnit, mcPriceMatrix);

            return mcPriceMatrix;
        }

        private static void GenerateMC_PriceMatrixValues(double gridForTime, double volatility, double riskFreeOptionPrice,
            int simulations, MersenneTwister random, double timeUnit, List<Tuple<double, double[]>> MC_PriceMatrix)
        {
            for (var i = 1; i < gridForTime + 1; i++)
            {
                var Z = Normal.WithMeanStdDev(0.0, 1.0, random).Samples().Take(simulations).ToArray();
                var S = Simulation_MC(simulations, riskFreeOptionPrice, timeUnit, volatility, Z, MC_PriceMatrix[i - 1]);

                MC_PriceMatrix.Add(new Tuple<double, double[]>(i * timeUnit, S));
            }
        }

        private static double[] Simulation_MC(int simulations, double riskFreeOptionPrice, double timeUnit,
            double volatility, double[] Z, Tuple<double, double[]> mcMatrix)
        {
            double[] S = new double[simulations];

            Parallel.For(0, simulations,
                (index) =>
                {
                    S[index] = mcMatrix.Item2[index] *
                               Math.Exp((riskFreeOptionPrice - Math.Pow(volatility, 2) / 2.0) * timeUnit +
                                        volatility * Math.Sqrt(timeUnit) * Z[index]);
                });

            return S;
        }

    }
}
