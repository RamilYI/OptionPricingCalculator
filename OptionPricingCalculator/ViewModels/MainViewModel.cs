using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using DynamicData;
using Microsoft.Win32;
using OptionPricingCalculator.Common.Models;
using OptionPricingCalculator.Common.Settings;
using OptionPricingCalculator.Computer;
using OptionPricingCalculator.ViewModels.Interfaces;
using OptionPricingCalculator.Views.Windows;
using OxyPlot;
using OxyPlot.Series;
using ReactiveUI;

namespace OptionPricingCalculator.ViewModels
{
    public class MainViewModel : ReactiveObject, IMainViewModel
    {
        private OptionModel optionModel;

        private readonly ReadOnlyObservableCollection<OptionParameters> _optionPricingCalculationResults;

        public ReadOnlyObservableCollection<OptionParameters> OptionPricingCalculationResults =>
            this._optionPricingCalculationResults;

        private PlotModel _priceChartSeries;

        public PlotModel PriceChartSeries
        {
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

        private readonly ReactiveCommand<Unit, Unit> _calculateCommand;

        public ICommand CalculateCommand => this._calculateCommand;

        private readonly ReactiveCommand<Unit, Unit> _cancelCommand;

        public ICommand CancelCommand => this._cancelCommand;

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
            "call",
        };

        private string _calculationDuration = string.Empty;

        public string CalculationDuration
        {
            get => this._calculationDuration;
            set => this.RaiseAndSetIfChanged(ref this._calculationDuration, value);
        }

        private string _status = string.Empty;

        public string Status
        {
            get => this._status;
            set => this.RaiseAndSetIfChanged(ref this._status, value);
        }

        private bool _isCancel = true;

        public bool IsCancel
        {
            get => this._isCancel;
            set => this.RaiseAndSetIfChanged(ref this._isCancel, value);
        }

        public MainViewModel(OptionModel model)
        {
            this.optionModel = model;
            this.WhenAnyValue(x => x.OptionType).Subscribe(x => this.optionModel.OptionType = x);
            this.WhenAnyValue(x => x.InitialStock).Subscribe(x => this.optionModel.InitialStock = x);
            this.WhenAnyValue(x => x.Volatility).Subscribe(x => this.optionModel.Volatility = x);
            this.WhenAnyValue(x => x.StrikeValue).Subscribe(x => this.optionModel.StrikeValue = x);
            this.WhenAnyValue(x => x.MaturityTime).Subscribe(x => this.optionModel.MaturityTime = x);
            this.WhenAnyValue(x => x.RiskFreeInterestRate).Subscribe(x => this.optionModel.RiskFreeInterestRate = x);
            this.WhenAnyValue(x => x.DividendYield).Subscribe(x => this.optionModel.DividendYield = x);
            this.WhenAnyValue(x => x.NumberOfAssets).Subscribe(x => this.optionModel.NumberOfAssets = x);
            this.WhenAnyValue(x => x.CalculationDuration).Subscribe(x => this.optionModel.CalculationDuration = x);
            this.WhenAnyValue(x => x.Status).Subscribe(x => this.optionModel.Status = x);
            this.OptionType = this.OptionValues.FirstOrDefault();
            var sourceResults = new SourceList<OptionParameters>();

            this._priceChartSeries = new PlotModel();
            this.PriceChartSeries.Series.Add(new LineSeries());
            this._calculateCommand = ReactiveCommand.CreateFromTask(() => Task.Run(() => this.Calculate(sourceResults)));
            var cancellationResults = sourceResults.Connect().ObserveOnDispatcher().Bind(out this._optionPricingCalculationResults)
                .DisposeMany().Subscribe();

            this._exitCommand = ReactiveCommand.Create(() => Application.Current.Shutdown());

            this._openHelpCommand = ReactiveCommand.Create(this.OpenHelp);

            this._openSettingsCommand = ReactiveCommand.Create(this.OpenSettings);

            this._generateReportCommand = ReactiveCommand.CreateFromTask(this.GenerateReportAsync);

            this._saveProjectCommand = ReactiveCommand.CreateFromTask(this.SaveProjectAsync);

            this._openProjectCommand = ReactiveCommand.CreateFromTask(this.OpenProjectAsync);

            this._createProjectCommand = ReactiveCommand.Create(() =>
            {
                this.optionModel = new OptionModel();
                this.InitializeProperties();
            });

            this._cancelCommand = ReactiveCommand.Create(() =>
            {
                this._isCancel = false;
                this.RaisePropertyChanged(nameof(this.IsCancel));
            });
        }

        private Task GenerateReportAsync()
        {
            return Task.Run(() =>
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Csv file (*.csv)|*.csv"
                };
                saveFileDialog.ShowDialog();

                using (var sw = new StreamWriter(saveFileDialog.FileName))
                {
                    sw.WriteLine($"{nameof(OptionParameters.Paths)}; {nameof(OptionParameters.Price)};" +
                                 $"{nameof(OptionParameters.Delta)}; {nameof(OptionParameters.Gamma)};" +
                                 $"{nameof(OptionParameters.Theta)}; {nameof(OptionParameters.Vega)};" +
                                 $"{nameof(OptionParameters.Rho)}");
                    foreach (var optionPricingCalculationResult in OptionPricingCalculationResults)
                    {
                        sw.WriteLine($"{optionPricingCalculationResult.Paths}; {optionPricingCalculationResult.Price}; " +
                                     $"{optionPricingCalculationResult.Delta}; {optionPricingCalculationResult.Gamma};" +
                                     $"{optionPricingCalculationResult.Theta}; ${optionPricingCalculationResult.Vega};" +
                                     $"{optionPricingCalculationResult.Rho}");
                    }
                }
            });
        }

        private void OpenSettings()
        {
            var settingsViewModel = new SettingsViewModel();
            var settingsView = new SettingsView {DataContext = settingsViewModel};
            settingsView.ShowDialog();
        }

        private void OpenHelp()
        {
            var aboutApplication = File.ReadAllText(@"..\..\ThereWillBeInformationAboutApp.txt");
            var aboutFeatures = File.ReadAllText(@"..\..\ThereWillBeInformationAboutFeatures.txt");
            var helpView = new HelpView(aboutApplication, aboutFeatures);
            helpView.Show();
        }

        private void Calculate(SourceList<OptionParameters> sourceResults)
        {
            this._isCancel = true;
            this.RaisePropertyChanged(nameof(this.IsCancel));
            this.Status = "Расчёт идёт";
            this.CalculationDuration = string.Empty;
            var test = new LineSeries();
            this._priceChartSeries.Series.Clear();
            this._priceChartSeries.Series.Add(test);
            sourceResults.Clear();
            LeastSquareMethod result;
            var i1Value = EnvironmentSettings.Instance.SimulationNumbers / EnvironmentSettings.Instance.NumberOfPath;
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            for (var i = i1Value; i <= EnvironmentSettings.Instance.SimulationNumbers; i += i1Value)
            {
                if (!this.IsCancel)
                {
                    break;
                }

                result = new LeastSquareMethod(this.optionModel.InitialStock, this.optionModel.StrikeValue, this.optionModel.MaturityTime, this.optionModel.Volatility,
                    this.optionModel.RiskFreeInterestRate, i, this.optionModel.OptionType);

                var optionParameter = new OptionParameters
                {
                    Paths = i,
                    Price = result.ReturnPrice(),
                    Delta = EnvironmentSettings.Instance.IsDeltaEnabled ? result.Delta() : 0,
                    Gamma = EnvironmentSettings.Instance.IsGammaEnabled ? result.Gamma() : 0,
                    Theta = EnvironmentSettings.Instance.IsThetaEnabled ? result.Theta() : 0,
                    Rho = EnvironmentSettings.Instance.IsRhoEnabled ? result.Rho() : 0,
                    Vega = EnvironmentSettings.Instance.IsRhoEnabled ? result.Vega() : 0,
                };
                sourceResults.Add(optionParameter);
                test.Points.Add(new DataPoint(i, result.ReturnPrice()));
                this._priceChartSeries.InvalidatePlot(true);
            }

            sw1.Stop();
            this.Status = "Расчёт окончен";
            this.CalculationDuration = (sw1.ElapsedMilliseconds / 1000).ToString() + " секунд";
        }

        private Task OpenProjectAsync()
        {
            return Task.Run(() =>
            {
                var openFileDialog = new OpenFileDialog();
                string path = string.Empty;
                openFileDialog.Filter = "Xml file (*.xml)|*.xml";
                openFileDialog.ShowDialog();
                var formatter = new XmlSerializer(typeof(OptionModel));
                using (var fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    this.optionModel = (OptionModel)formatter.Deserialize(fs);
                    this.InitializeProperties();
                }
            });
        }

        private void InitializeProperties()
        {
            this.OptionType = this.optionModel.OptionType;
            this.InitialStock = this.optionModel.InitialStock;
            this.Volatility = this.optionModel.Volatility;
            this.StrikeValue = this.optionModel.StrikeValue;
            this.MaturityTime = this.optionModel.MaturityTime;
            this.RiskFreeInterestRate = this.optionModel.RiskFreeInterestRate;
            this.DividendYield = this.optionModel.DividendYield;
            this.NumberOfAssets = this.optionModel.NumberOfAssets;
        }

        private Task SaveProjectAsync()
        {
            return Task.Run(() =>
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Xml file (*.xml)|*.xml";
                saveFileDialog.ShowDialog();
                var formatter = new XmlSerializer(typeof(OptionModel));

                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, this.optionModel);
                }
            });
        }
    }
}
