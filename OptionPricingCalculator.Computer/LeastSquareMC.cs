using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using OptionPricingCalculator.Computer.Extensions;

namespace OptionPricingCalculator.Computer
{
    public class LeastSquareMC : IAlgorithm
    {
        private List<double[]> LeastSquareMatrix { get; set; }
        private double Discount { get; set; }
        private int Simulations { get; set; }

        public LeastSquareMC(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        {
            LeastSquareMatrix = new List<double[]>();
            this.Simulations = simulations;
            var MC_PriceMatrix = MonteCarloPriceMatrix.MonteCarloCreator(gridForTime, volatility, riskFreeOptionPrice, simulations, T, initialStock);
            var MC_PayOff = MonteCarloPayoff.MonteCarloPayOff(MC_PriceMatrix, strike, optionType);
            var timeUnit = T / gridForTime;
            this.Discount = Math.Exp(-riskFreeOptionPrice * timeUnit);

            CreateLeastSquareMatrix(this.Simulations, MC_PriceMatrix, LeastSquareMatrix);

            RegressionMC(gridForTime, this.Simulations, LeastSquareMatrix, MC_PayOff, MC_PriceMatrix, this.Discount);
        }

        private static void RegressionMC(double gridForTime, int simulations, List<double[]> leastSquareMatrix, double[,] MC_PayOff, List<Tuple<double, double[]>> MC_PriceMatrix, double discount)
        {
            leastSquareMatrix[(int) (gridForTime - 1)] = MC_PayOff.GetRow((int) (gridForTime - 1));

            for (var i = (int) (gridForTime - 1); i > 0; i--)
            {
                var regression = Fit.Polynomial(MC_PriceMatrix[i - 1].Item2,
                    leastSquareMatrix[i].Select(x => x * discount).ToArray(), 5);
                var continuation_value = MC_PriceMatrix[i - 1].Item2.Select(x => Polynomial.Evaluate(x, regression)).ToArray();

                var newVal = new double[simulations];
                var val2 = MC_PayOff.GetRow(i - 1);
                //for (var i1 = 0; i1 < simulations; i1++)
                //{
                //    if (val2[i1] > continuation_value[i1]) newVal[i1] = val2[i1];
                //    else newVal[i1] = leastSquareMatrix[i][i1] * discount;
                //}

                Parallel.For(0, simulations, (i1) =>
                {
                    if (val2[i1] > continuation_value[i1]) newVal[i1] = val2[i1];
                    else newVal[i1] = leastSquareMatrix[i][i1] * discount;
                });

                leastSquareMatrix[i - 1] = newVal;
            }
        }

        private static void CreateLeastSquareMatrix(int simulations, List<Tuple<double, double[]>> MC_PriceMatrix, List<double[]> leastSquareMatrix)
        {
            for (var index = 0; index < MC_PriceMatrix.Count; index++)
            {
                leastSquareMatrix.Add(Enumerable.Repeat(0.0, simulations).ToArray());
            }

            //Parallel.For(0, MC_PriceMatrix.Count, (index) =>
            //{
            //    leastSquareMatrix.Add(Enumerable.Repeat(0.0, simulations).ToArray());
            //});
        }

        public double ReturnPrice() => this.LeastSquareMatrix[1].Select(x => x * this.Discount).Sum() / this.Simulations;
    }
}
