using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using OptionPricingCalculator.Common.Models;
using OptionPricingCalculator.Common.Settings;

namespace OptionPricingCalculator.Computer
{
    public class LeastSquareMethod : IGreekOdds
    {
        private List<double[]> LeastSquareMatrix { get; }
        private double Discount { get; }
        private int Simulations { get; }
        private double InitialStock { get; }
        private double Strike { get; }
        private double T { get; }
        private double Volatility { get; }
        private double RiskFreeOptionPrice { get; }
        private string OptionType { get; }
        private double[][] MC_PayOff { get; }
        private List<Tuple<double, double[]>> MC_PriceMatrix { get; }

        public LeastSquareMethod(double initialStock, double strike, double t, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        {
            LeastSquareMatrix = new List<double[]>();
            this.InitialStock = initialStock;
            this.Strike = strike;
            this.T = t;
            this.Volatility = volatility;
            this.RiskFreeOptionPrice = riskFreeOptionPrice;
            this.OptionType = optionType;
            this.Simulations = simulations;

            MC_PriceMatrix = EnvironmentSettings.Instance.StochasticProcessName == "Геометрическое броуновское движение" ? MonteCarloPriceMatrix.MonteCarloCreator(volatility, riskFreeOptionPrice, simulations, t, initialStock) : 
                JumpDiffusionSimulation.JumpDiffusionCreator(volatility, riskFreeOptionPrice, simulations, t, initialStock);

            MC_PayOff = Payoff.PayOff(MC_PriceMatrix, strike, optionType);
            var timeUnit = t / EnvironmentSettings.Instance.GridForTime;
            this.Discount = Math.Exp(-riskFreeOptionPrice * timeUnit);

            CreateLeastSquareMatrix(MC_PriceMatrix, LeastSquareMatrix);

            RegressionMC();
        }

        private void RegressionMC()
        {
            LeastSquareMatrix[(int) (EnvironmentSettings.Instance.GridForTime - 1)] = MC_PayOff[(int)(EnvironmentSettings.Instance.GridForTime - 1)];
            for (var i = (int) (EnvironmentSettings.Instance.GridForTime - 1); i > 0; i--)
            {
                var regression = Fit.Polynomial(MC_PriceMatrix[i - 1].Item2,
                    LeastSquareMatrix[i].Select(x => x * this.Discount).ToArray(), EnvironmentSettings.Instance.PolynomialDegree);
                var continuation_value = MC_PriceMatrix[i - 1].Item2.Select(x => Polynomial.Evaluate(x, regression)).ToArray();

                var newVal = new double[this.Simulations];
                if (EnvironmentSettings.Instance.IsParallel)
                {
                    Parallel.For(0, this.Simulations, (i1) =>
                    {
                        if (MC_PayOff[i - 1][i1] > continuation_value[i1]) newVal[i1] = MC_PayOff[i - 1][i1];
                        else newVal[i1] = LeastSquareMatrix[i][i1] * this.Discount;
                    });
                }
                else
                {
                    for (var i1 = 0; i1 < this.Simulations; i1++)
                    {
                        if (MC_PayOff[i - 1][i1] > continuation_value[i1]) newVal[i1] = MC_PayOff[i - 1][i1];
                        else newVal[i1] = LeastSquareMatrix[i][i1] * this.Discount;
                    }
                }

                LeastSquareMatrix[i - 1] = newVal;
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

        public double Delta()
        {
            var diff = this.InitialStock * 0.01;
            var myCall_1 = new LeastSquareMethod(this.InitialStock + diff, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            var myCall_2 = new LeastSquareMethod(this.InitialStock - diff, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Gamma()
        {
            var diff = this.InitialStock * 0.01;
            var myCall_1 = new LeastSquareMethod(this.InitialStock + diff, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            var myCall_2 = new LeastSquareMethod(this.InitialStock - diff, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            return (myCall_1.Delta() - myCall_2.Delta()) / (2 * diff);
        }

        public double Vega()
        {
            var diff = this.Volatility * 0.01;
            var myCall_1 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility + diff, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            var myCall_2 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility - diff, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Rho()
        {
            var diff = this.RiskFreeOptionPrice * 0.01;
            LeastSquareMethod myCall_1;
            LeastSquareMethod myCall_2;
            if (this.RiskFreeOptionPrice - diff < 0)
            {
                myCall_1 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice + diff, this.Simulations, this.OptionType);
                myCall_2 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
                return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
            }
            myCall_1 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice + diff, this.Simulations, this.OptionType);
            myCall_2 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T, this.Volatility, this.RiskFreeOptionPrice - diff, this.Simulations, this.OptionType);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        public double Theta()
        {
            var diff = 1.0 / 252.0;
            var myCall_1 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T + diff, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            var myCall_2 = new LeastSquareMethod(this.InitialStock, this.Strike, this.T - diff, this.Volatility, this.RiskFreeOptionPrice, this.Simulations, this.OptionType);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }
    }
}
