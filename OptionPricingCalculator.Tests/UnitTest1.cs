using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OptionPricingCalculator.Computer;
using MathNet.Numerics;

namespace OptionPricingCalculator.Tests
{
    [TestFixture]
    public class LeastSquareMonteCarloTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            var T = 1.0;
            var tMaturity = 50.0;
            var timeUnit = T / tMaturity;
            var strike = 40;
            var initialStock = 36;
            var riskFreeOptionPrice = 0.06;
            var simulations = 10000;
            var volatility = 0.2;
            //Func<int, LeastSquareMC> creator = x => new LeastSquareMC(initialStock, strike, T, tMaturity, volatility, riskFreeOptionPrice,
            //    simulations, "put");
            //IEnumerable<int> indices = Enumerable.Range(0, simulations - 1);
            //LeastSquareMC[] results = new LeastSquareMC[simulations];
            //results = indices.AsParallel().Select(creator).ToArray();
            var results = new LeastSquareMC(initialStock, strike, T, tMaturity, volatility, riskFreeOptionPrice, simulations, "put");
            Assert.IsTrue(results.ReturnPrice().AlmostEqual(4.0, 1.0));
        }
    }
}