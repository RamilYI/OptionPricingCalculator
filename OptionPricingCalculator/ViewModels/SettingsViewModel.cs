using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionPricingCalculator.Models;
using OptionPricingCalculator.ViewModels.Interfaces;
using ReactiveUI;

namespace OptionPricingCalculator.ViewModels
{
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private int numOfSimulations = EnvironmentSettings.Instance.SimulationNumbers;

        private int gridOfTime = EnvironmentSettings.Instance.GridForTime;

        private bool isDeltaEnabled = EnvironmentSettings.Instance.IsDeltaEnabled;

        private bool isGammaEnabled = EnvironmentSettings.Instance.IsGammaEnabled;

        private bool isVegaEnabled = EnvironmentSettings.Instance.IsVegaEnabled;

        private bool isRhoEnabled = EnvironmentSettings.Instance.IsRhoEnabled;

        private bool isThetaEnabled = EnvironmentSettings.Instance.IsThetaEnabled;

        private bool isParallel = EnvironmentSettings.Instance.IsParallel;

        public int NumOfSimulations
        {
            get => this.numOfSimulations;
            set
            {
                this.RaiseAndSetIfChanged(ref this.numOfSimulations, value);
                EnvironmentSettings.Instance.SimulationNumbers = this.numOfSimulations;
            }
        }

        public int GridOfTime
        {
            get => this.gridOfTime;
            set
            {
                this.RaiseAndSetIfChanged(ref this.gridOfTime, value);
                EnvironmentSettings.Instance.GridForTime = this.gridOfTime;
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
    }
}
