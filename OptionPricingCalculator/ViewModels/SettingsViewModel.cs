using System.Windows;
using OptionPricingCalculator.Common.Models;
using OptionPricingCalculator.Common.Settings;
using OptionPricingCalculator.Common.Settings.Interfaces;
using OptionPricingCalculator.ViewModels.Interfaces;
using ReactiveUI;

namespace OptionPricingCalculator.ViewModels
{
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private int simulationNumbers = EnvironmentSettings.Instance.SimulationNumbers;

        private int gridForTime = EnvironmentSettings.Instance.GridForTime;

        private bool isDeltaEnabled = EnvironmentSettings.Instance.IsDeltaEnabled;

        private bool isGammaEnabled = EnvironmentSettings.Instance.IsGammaEnabled;

        private bool isVegaEnabled = EnvironmentSettings.Instance.IsVegaEnabled;

        private bool isRhoEnabled = EnvironmentSettings.Instance.IsRhoEnabled;

        private bool isThetaEnabled = EnvironmentSettings.Instance.IsThetaEnabled;

        private bool isParallel = EnvironmentSettings.Instance.IsParallel;

        private int numberOfPath = EnvironmentSettings.Instance.NumberOfPath;

        public int[] PolynomialValues => new[] { 3, 4, 5, 6, 10, 11 };

        private int polynomialDegree = EnvironmentSettings.Instance.PolynomialDegree;

        public string[] StochasticProcessNames => new[] { "Геометрическое броуновское движение", "Скачкообразная диффузия" };

        private string stochasticProcessName = EnvironmentSettings.Instance.StochasticProcessName;

        private double jumpLambda = EnvironmentSettings.Instance.JumpLambda;

        private double jumpLambdaSize = EnvironmentSettings.Instance.JumpLambdaSize;

        private double jumpLambdaStd = EnvironmentSettings.Instance.JumpLambdaStd;

        private int timeIntervals = EnvironmentSettings.Instance.TimeIntervals;

        public int SimulationNumbers
        {
            get => this.simulationNumbers;
            set
            {
                this.RaiseAndSetIfChanged(ref this.simulationNumbers, value);
                EnvironmentSettings.Instance.SimulationNumbers = this.simulationNumbers;
            }
        }

        public int GridForTime
        {
            get => this.gridForTime;
            set
            {
                this.RaiseAndSetIfChanged(ref this.gridForTime, value);
                EnvironmentSettings.Instance.GridForTime = this.gridForTime;
            }
        }

        public bool IsDeltaEnabled
        {
            get => this.isDeltaEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isDeltaEnabled, value);
                EnvironmentSettings.Instance.IsDeltaEnabled = this.isDeltaEnabled;
            }
        }

        public bool IsGammaEnabled
        {
            get => this.isGammaEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isGammaEnabled, value);
                EnvironmentSettings.Instance.IsGammaEnabled = this.isGammaEnabled;
            }
        }

        public bool IsVegaEnabled
        {
            get => this.isVegaEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isVegaEnabled, value);
                EnvironmentSettings.Instance.IsVegaEnabled = this.isVegaEnabled;
            }
        }

        public bool IsRhoEnabled
        {
            get => this.isRhoEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isRhoEnabled, value);
                EnvironmentSettings.Instance.IsRhoEnabled = this.isRhoEnabled;
            }
        }

        public bool IsThetaEnabled
        {
            get => this.isThetaEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isThetaEnabled, value);
                EnvironmentSettings.Instance.IsThetaEnabled = this.isThetaEnabled;
            }
        }

        public bool IsParallel
        {
            get => this.isParallel;
            set
            {
                this.RaiseAndSetIfChanged(ref this.isParallel, value);
                EnvironmentSettings.Instance.IsParallel = this.isParallel;
            }
        }

        public int NumberOfPath
        {
            get => this.numberOfPath;
            set
            {
                this.RaiseAndSetIfChanged(ref this.numberOfPath, value);
                EnvironmentSettings.Instance.NumberOfPath = this.numberOfPath;
            }
        }

        public int PolynomialDegree
        {
            get => this.polynomialDegree;
            set
            {
                this.RaiseAndSetIfChanged(ref this.polynomialDegree, value);
                EnvironmentSettings.Instance.PolynomialDegree = this.polynomialDegree;
            }
        }

        public string StochasticProcessName
        {
            get => this.stochasticProcessName;
            set
            {
                this.RaiseAndSetIfChanged(ref this.stochasticProcessName, value);
                EnvironmentSettings.Instance.StochasticProcessName = this.stochasticProcessName;
                this.RaisePropertyChanged(nameof(this.JumpDiffusionParamsVisibility));
            }
        }

        public double JumpLambda
        {
            get => this.jumpLambda;
            set
            {
                this.RaiseAndSetIfChanged(ref this.jumpLambda, value);
                EnvironmentSettings.Instance.JumpLambda = this.jumpLambda;
            }
        }

        public double JumpLambdaSize
        {
            get => this.jumpLambdaSize;
            set
            {
                this.RaiseAndSetIfChanged(ref this.jumpLambdaSize, value);
                EnvironmentSettings.Instance.JumpLambdaSize = this.jumpLambdaSize;
            }
        }

        public double JumpLambdaStd
        {
            get => this.jumpLambdaStd;
            set
            {
                this.RaiseAndSetIfChanged(ref this.jumpLambdaStd, value);
                EnvironmentSettings.Instance.JumpLambdaStd = this.jumpLambdaStd;
            }
        }

        public int TimeIntervals
        {
            get => this.timeIntervals;
            set
            {
                this.RaiseAndSetIfChanged(ref this.timeIntervals, value);
                EnvironmentSettings.Instance.TimeIntervals = this.timeIntervals;
            }
        }

        public Visibility JumpDiffusionParamsVisibility
        {
            get
            {
                if (EnvironmentSettings.Instance.StochasticProcessName == this.StochasticProcessNames[0])
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }
    }
}
