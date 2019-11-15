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
        public static List<Tuple<double, double[]>> MonteCarloCreator(double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, double T, double initialStock, bool isParallel = true)
        {
            var timeUnit = T / gridForTime;
            var random = new MersenneTwister();
            var mcPriceMatrix = new List<Tuple<double, double[]>>
            {
                new Tuple<double, double[]>(0, Enumerable.Repeat(initialStock, simulations).ToArray())
            };


            GenerateMC_PriceMatrixValues(gridForTime, volatility, riskFreeOptionPrice, simulations, random, timeUnit, mcPriceMatrix, isParallel);

            return mcPriceMatrix;
        }

        private static void GenerateMC_PriceMatrixValues(double gridForTime, double volatility, double riskFreeOptionPrice,
            int simulations, MersenneTwister random, double timeUnit, List<Tuple<double, double[]>> MC_PriceMatrix, bool isParallel = true)
        {
            for (var i = 1; i < gridForTime + 1; i++)
            {
                var Z = Normal.WithMeanStdDev(0.0, 1.0, random).Samples().Take(simulations).ToArray();
                var S = Simulation_MC(simulations, riskFreeOptionPrice, timeUnit, volatility, Z, MC_PriceMatrix[i - 1], isParallel);

                MC_PriceMatrix.Add(new Tuple<double, double[]>(i * timeUnit, S));
            }
        }

        private static double[] Simulation_MC(int simulations, double riskFreeOptionPrice, double timeUnit,
            double volatility, double[] Z, Tuple<double, double[]> mcMatrix, bool isParallel = true)
        {
            double[] S = new double[simulations];

            if (isParallel)
            {
                Parallel.For(0, simulations,
                    (index) =>
                    {
                        S[index] = mcMatrix.Item2[index] *
                                   Math.Exp((riskFreeOptionPrice - Math.Pow(volatility, 2) / 2.0) * timeUnit +
                                            volatility * Math.Sqrt(timeUnit) * Z[index]);
                    });
            }
            else
            {
                for (int i = 0; i < simulations; i++)
                {
                    S[i] = mcMatrix.Item2[i] *
                           Math.Exp((riskFreeOptionPrice - Math.Pow(volatility, 2) / 2.0) * timeUnit +
                                    volatility * Math.Sqrt(timeUnit) * Z[i]);
                }
            }

            return S;
        }

    }
}
