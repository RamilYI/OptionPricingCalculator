using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DynamicData;
using OptionPricingCalculator.Computer;
using OptionPricingCalculator.Models;
using OptionPricingCalculator.ViewModels.Interfaces;
using OptionPricingCalculator.Views.Windows;
using OxyPlot;
using OxyPlot.Series;
using PropertyChanged;
using ReactiveUI;

namespace OptionPricingCalculator.ViewModels
{
    [DoNotNotify]
    public class MainViewModel : ReactiveObject, IMainViewModel
    {
        private ReadOnlyObservableCollection<OptionParameters> _optionPricingCalculationResults;

        public ReadOnlyObservableCollection<OptionParameters> OptionPricingCalculationResults =>
            this._optionPricingCalculationResults;

        private PlotModel _priceChartSeries;

        public PlotModel PriceChartSeries {
            get { return this._priceChartSeries; }
            set { this.RaiseAndSetIfChanged(ref this._priceChartSeries, value); }
        }

        private readonly ReactiveCommand<Unit, Unit> _exitCommand;

        public ICommand ExitCommand => this._exitCommand;

        private readonly ReactiveCommand<Unit, Unit> _openHelpCommand;

        public ICommand OpenHelpCommand => this._openHelpCommand;

        private readonly ReactiveCommand<Unit, Unit> _openSettingsCommand;

        public ICommand OpenSettingsCommand => this._openSettingsCommand;

        private readonly ReactiveCommand<Unit, Unit> _generateReportCommand;

        public ICommand GenerateReportCommand => this._generateReportCommand;

        private readonly ReactiveCommand<Unit, Unit> _saveProjectCommand;

        public ICommand SaveProjectCommand => this._saveProjectCommand;

        private readonly ReactiveCommand<Unit, Unit> _openProjectCommand;

        public ICommand OpenProjectCommand => this._openProjectCommand;

        private readonly ReactiveCommand<Unit, Unit> _createProjectCommand;

        public ICommand CreateProjectCommand => this._createProjectCommand;

        private readonly ReactiveCommand<Unit, Unit> _calculate;

        public ICommand Calculate => this._calculate;

        private string _optionType;

        public string OptionType
        {
            get => this._optionType;
            set => this.RaiseAndSetIfChanged(ref this._optionType, value);
        }

        private string _optionView;

        public string OptionView
        {
            get => this._optionView;
            set => this.RaiseAndSetIfChanged(ref this._optionView, value);
        }

        private double _initialStock;

        public double InitialStock
        {
            get => this._initialStock;
            set => this.RaiseAndSetIfChanged(ref this._initialStock, value);
        }

        private double _volatility;

        public double Volatility
        {
            get => this._volatility;
            set => this.RaiseAndSetIfChanged(ref this._volatility, value);
        }

        private double _strikeValue;

        public double StrikeValue
        {
            get => this._strikeValue;
            set => this.RaiseAndSetIfChanged(ref this._strikeValue, value);
        }

        public double _maturityTime;

        public double MaturityTime
        {
            get => this._maturityTime;
            set => this.RaiseAndSetIfChanged(ref this._maturityTime, value);
        }

        private double _riskFreeInterestRate;

        public double RiskFreeInterestRate
        {
            get => this._riskFreeInterestRate;
            set => this.RaiseAndSetIfChanged(ref this._riskFreeInterestRate, value);
        }

        private double _dividendYield;

        public double DividendYield
        {
            get => this._dividendYield;
            set => this.RaiseAndSetIfChanged(ref this._dividendYield, value);
        }

        private double _numberOfAssets;

        public double NumberOfAssets
        {
            get => this._numberOfAssets;
            set => this.RaiseAndSetIfChanged(ref this._numberOfAssets, value);
        }

        public List<string> OptionValues { get; } = new List<string>
        {
            "put",
            "call"
        };

        public MainViewModel()
        {
            this._optionType = OptionValues.FirstOrDefault();
            var sourceResults = new SourceList<OptionParameters>();
            //Random rand = new Random();

            //for (var i = 0; i < 10000; i++)
            //{
            //    var id = i + 1;
            //    var popularity = rand.Next(10000);
            //    sourceResults.Add(new OptionParameters
            //    {
            //        Episodes = rand.Next(10000),
            //        Genre = rand.Next(10000).ToString(),
            //        Id = id,
            //        Popularity = popularity,
            //        Score = rand.NextDouble(),
            //        TitleName = rand.Next(10000).ToString(),
            //    });
            //}
            var testModel = new PlotModel
            {
                Title = "Test Model",
            };
            var test = new LineSeries();
            this._calculate = ReactiveCommand.CreateFromTask(() => Task.Factory.StartNew(() =>
            {
                testModel.ResetAllAxes();
                sourceResults.Clear();
                //var gridForTime = 50;
                LeastSquareMC result;
                for (var i = 1000; i <= EnvironmentSettings.Instance.SimulationNumbers; i += 1000)
                {
                    result = new LeastSquareMC(this._initialStock, this._strikeValue, this._maturityTime, EnvironmentSettings.Instance.GridForTime, this._volatility,
                        this._riskFreeInterestRate, i, this._optionType);
                    
                    var optionParameter = new OptionParameters {Paths = i, Price = result.ReturnPrice()};

                    if (EnvironmentSettings.Instance.IsDeltaEnabled)
                    {
                        optionParameter.Delta = 0.0;
                    }

                    if (EnvironmentSettings.Instance.IsGammaEnabled)
                    {
                        optionParameter.Gamma = 0.0;
                    }

                    if (EnvironmentSettings.Instance.IsRhoEnabled)
                    {
                        optionParameter.Rho = 0.0;
                    }

                    if (EnvironmentSettings.Instance.IsThetaEnabled)
                    {
                        optionParameter.Theta = 0.0;
                    }

                    if (EnvironmentSettings.Instance.IsVegaEnabled)
                    {
                        optionParameter.Vega = 0.0;
                    }

                    sourceResults.Add(optionParameter);
                    test.Points.Add(new DataPoint(i, result.ReturnPrice()));
                    this.PriceChartSeries.InvalidatePlot(true);
                }
            }));
            testModel.Series.Add(test);
            this._priceChartSeries = testModel;
            var cancellationResults = sourceResults.Connect().ObserveOnDispatcher().Bind(out this._optionPricingCalculationResults)
                .DisposeMany().Subscribe();

            //var test = new LineSeries();
            //for (var i = 0; i < 500; i++)
            //{
            //    test.Points.Add(new DataPoint(i + 1, Math.Pow(i + 1, 2)));
            //}

            this._exitCommand = ReactiveCommand.Create(() => Application.Current.Shutdown());

            // TODO потом написать.
            //// написать команду, открывающую справку.
            this._openHelpCommand = ReactiveCommand.Create(() =>
            {
                var helpView = new HelpView();
                helpView.Show();
            });

            //// написать команду, открывающую настройки.
            this._openSettingsCommand = ReactiveCommand.Create(() =>
            {
                var settingsView = new SettingsView();
                settingsView.ShowDialog();
            });

            //// написать команду, генерирующую отчёт.
            //this._generateReportCommand = ReactiveCommand.Create(());

            //// написать команду, сохраняющую проект.
            //this._saveProjectCommand = ReactiveCommand.Create(());

            //// написать команду, открывающую проект.
            //this._openProjectCommand = ReactiveCommand.Create(());

            //// написать команду, создающую новый проект.
            //this._createProjectCommand = ReactiveCommand.Create(());
        }
    }
}
