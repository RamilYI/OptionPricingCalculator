using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using OptionPricingCalculator.Common.Settings;

namespace OptionPricingCalculator.Computer
{
    public static class JumpDiffusionSimulation
    {
        public static List<Tuple<double, double[]>> JumpDiffusionCreator(double volatility, double riskFreeOptionPrice, int simulations, double T, double initialStock, double jumpLambda, double lambdaSize, double lambdaStd, double timeIntervals)
        {
            var dT = T / timeIntervals;
            var random = new MersenneTwister();
            var jumpDrift = jumpLambda * (Math.Exp(lambdaSize + 0.5 * Math.Pow(lambdaStd, 2)) - 1);
            var jdPriceMatrix = new List<Tuple<double, double[]>>
            {
                new Tuple<double, double[]>(0, Enumerable.Repeat(initialStock, simulations).ToArray())
            };

            GenerateJD_PriceMatrixValues(volatility, riskFreeOptionPrice, simulations, jumpLambda, lambdaSize, lambdaStd, timeIntervals, random, dT, jdPriceMatrix, jumpDrift);

            return jdPriceMatrix;
        }

        private static void GenerateJD_PriceMatrixValues(double volatility, double riskFreeOptionPrice, int simulations,
            double jumpLambda, double lambdaSize, double lambdaStd, double timeIntervals, MersenneTwister random, double dT,
            List<Tuple<double, double[]>> jdPriceMatrix, double jumpDrift)
        {
            for (var i = 1; i < timeIntervals + 1; i++)
            {
                var gaussPrice = Normal.WithMeanStdDev(0.0, 1.0, random).Samples().Take(simulations).ToArray();
                var gaussJump = Normal.WithMeanStdDev(0.0, 1.0, random).Samples().Take(simulations).ToArray();
                var poissonJump = Poisson.Samples(jumpLambda * dT).Take(simulations).ToArray();

                var S = Simulation_JD(simulations, riskFreeOptionPrice, dT, volatility, gaussPrice, gaussJump,
                    poissonJump, jdPriceMatrix[i - 1], jumpDrift, lambdaSize, lambdaStd);

                jdPriceMatrix.Add(new Tuple<double, double[]>(i * dT, S));
            }
        }

        private static double[] Simulation_JD(int simulations, double riskFreeOptionPrice, double dT, double volatility,
            double[] gaussPrice, double[] gaussJump, int[] poissonJump, Tuple<double, double[]> jdMatrix,
            double jumpDrift,
            double lambdaSize, double lambdaStd)
        {
            var S = new double[simulations];

            if (EnvironmentSettings.Instance.IsParallel)
            {
                Parallel.For(0, simulations, (index) =>
                {
                    S[index] = jdMatrix.Item2[index] *
                               (Math.Exp((riskFreeOptionPrice - jumpDrift - 0.5 * Math.Pow(volatility, 2)) * dT +
                                         volatility * Math.Sqrt(dT) * gaussPrice[index]) +
                                (Math.Exp(lambdaSize + lambdaStd * gaussJump[index]) - 1) * poissonJump[index]);
                });
            }
            else
            {
                for (int index = 0; index < simulations; index++)
                {
                    S[index] = jdMatrix.Item2[index] *
                               (Math.Exp((riskFreeOptionPrice - jumpDrift - 0.5 * Math.Pow(volatility, 2)) * dT +
                                         volatility * Math.Sqrt(dT) * gaussPrice[index]) +
                                (Math.Exp(lambdaSize + lambdaStd * gaussJump[index]) - 1) * poissonJump[index]);
                }
            }

            return S;
        }
    }
}