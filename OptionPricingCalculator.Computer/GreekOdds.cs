using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionPricingCalculator.Computer
{
    public class GreekOdds
    {
        public double Delta(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        {
            var diff = initialStock * 0.01;
            var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
            var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
            return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        }

        //public double Gamma(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        //{
        //    var diff = initialStock * 0.01;
        //    var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        //}

        //public double Delta(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        //{
        //    var diff = initialStock * 0.01;
        //    var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        //}

        //public double Delta(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        //{
        //    var diff = initialStock * 0.01;
        //    var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        //}

        //public double Delta(double initialStock, double strike, double T, double gridForTime, double volatility, double riskFreeOptionPrice, int simulations, string optionType)
        //{
        //    var diff = initialStock * 0.01;
        //    var myCall_1 = new LeastSquareMC(initialStock + diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    var myCall_2 = new LeastSquareMC(initialStock - diff, strike, T, gridForTime, volatility, riskFreeOptionPrice, simulations, optionType);
        //    return (myCall_1.ReturnPrice() - myCall_2.ReturnPrice()) / (2 * diff);
        //}
    }
}
