using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using OptionPricingCalculator.Computer.Extensions;

namespace OptionPricingCalculator.Computer
{
    public class LeastSquareMC : IGreekOdds
    {
        private List<double[]> LeastSquareMatrix { get; }
        private double Discount { get; }
        private int Simulations { get; }
        private double InitialStock { get; }
        private double Strike { get; }
        private double T { get; }
        private double GridForTime { get; }
        private double Volatility { get; }
        private double RiskFreeOptionPrice { get; }
        private string OptionType { get; }
        private bool IsParallel { get; }

        public LeastSquareMC(double initialStock, double strike, double t, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType, bool isParallel = true)
        {
            this.IsParallel = isParallel;
            LeastSquareMatrix = new List<double[]>();
            this.InitialStock = initialStock;
            this.Strike = strike;
            this.T = t;
            this.GridForTime = gridForTime;
            this.Volatility = volatility;
            this.RiskFreeOptionPrice = riskFreeOptionPrice;
            this.OptionType = optionType;
            this.Simulations = simulations;
            var MC_PriceMatrix = MonteCarloPriceMatrix.MonteCarloCreator(gridForTime, volatility, riskFreeOptionPrice, simulations, t, initialStock, isParallel);
            var MC_PayOff = MonteCarloPayoff.MonteCarloPayOff(MC_PriceMatrix, strike, optionType, isParallel);
            var timeUnit = t / gridForTime;
            this.Discount = Math.Exp(-riskFreeOptionPrice * timeUnit);

            CreateLeastSquareMatrix(MC_PriceMatrix, LeastSquareMatrix);

            RegressionMC(gridForTime, LeastSquareMatrix, MC_PayOff, MC_PriceMatrix);
        }

        private void RegressionMC(double gridForTime, List<double[]> leastSquareMatrix, double[][] MC_PayOff, List<Tuple<double, double[]>> MC_PriceMatrix)
        {
            //leastSquareMatrix[(int) (gridForTime - 1)] = MC_PayOff.GetRow((int) (gridForTime - 1));
            leastSquareMatrix[(int) (gridForTime - 1)] = MC_PayOff[(int)(gridForTime - 1)];
            for (var i = (int) (gridForTime - 1); i > 0; i--)
            {
                var regression = Fit.Polynomial(MC_PriceMatrix[i - 1].Item2,
                    leastSquareMatrix[i].Select(x => x * this.Discount).ToArray(), 5);
                var continuation_value = MC_PriceMatrix[i - 1].Item2.Select(x => Polynomial.Evaluate(x, regression)).ToArray();

                var newVal = new double[this.Simulations];
                //var val2 = MC_PayOff[i -1];
                if (this.IsParallel)
                {
                    Parallel.For(0, this.Simulations, (i1) =>
                    {
                        if (MC_PayOff[i - 1][i1] > continuation_value[i1]) newVal[i1] = MC_PayOff[i - 1][i1];
                        else newVal[i1] = leastSquareMatrix[i][i1] * this.Discount;
                    });
                }
                else
                {
                    for (var i1 = 0; i1 < this.Simulations; i1++)
                    {
                        if (MC_PayOff[i - 1][i1] > continuation_value[i1]) newVal[i1] = MC_PayOff[i - 1][i1];
                        else newVal[i1] = leastSquareMatrix[i][i1] * this.Discount;
                    }
                }

                leastSquareMatrix[i - 1] = newVal;
            }
        }

        private void CreateLeastSquareMatrix(List<Tuple<double, double[]>> MC_PriceMatrix, List<double[]> leastSquareMatrix)
        {
            for (var index = 0; index < MC_PriceMatrix.Count; index++)
            {
                leastSquareMatrix.Add(Enumerable.Repeat(0.0, this.Simulations).ToArray());
            }
        }

        public double ReturnPrice() => this.LeastSquareMatrix[1].Select(x => x * this.Discount).Sum() / this.Simulations;

        public double Delta(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        {
            var diff = initialStock * 0.01;
            var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, this.Simulations, optionType, this.IsParallel);
            var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, this.Simulations, optionType, this.IsParallel);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Delta()
        {
            var diff = this.InitialStock * 0.01;
            var myCall_1 = new LeastSquareMC(this.InitialStock + diff, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            var myCall_2 = new LeastSquareMC(this.InitialStock - diff, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Gamma()
        {
            var diff = this.InitialStock * 0.01;
            var myCall_1 = new LeastSquareMC(this.InitialStock + diff, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            var myCall_2 = new LeastSquareMC(this.InitialStock - diff, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            return (myCall_1.Delta() - myCall_2.Delta()) / (2 * diff);
        }

        public double Vega()
        {
            var diff = this.Volatility * 0.01;
            var myCall_1 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility + diff, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            var myCall_2 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility - diff, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Rho()
        {
            var diff = this.RiskFreeOptionPrice * 0.01;
            LeastSquareMC myCall_1;
            LeastSquareMC myCall_2;
            if (this.RiskFreeOptionPrice - diff < 0)
            {
                myCall_1 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice + diff, this.Simulations, this.OptionType, this.IsParallel);
                myCall_2 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
                return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
            }
            myCall_1 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice + diff, this.Simulations, this.OptionType, this.IsParallel);
            myCall_2 = new LeastSquareMC(this.InitialStock, this.Strike, this.T, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice - diff, this.Simulations, this.OptionType, this.IsParallel);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Theta()
        {
            var diff = 1.0 / 252.0;
            var myCall_1 = new LeastSquareMC(this.InitialStock, this.Strike, this.T + diff, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            var myCall_2 = new LeastSquareMC(this.InitialStock, this.Strike, this.T - diff, this.GridForTime, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType, this.IsParallel);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }
    }
}
