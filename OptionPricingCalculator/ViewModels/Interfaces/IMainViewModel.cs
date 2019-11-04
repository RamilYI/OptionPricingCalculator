using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OptionPricingCalculator.ViewModels.Interfaces
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        ICommand ExitCommand { get; }

        ICommand OpenHelpCommand { get; }

        ICommand OpenSettingsCommand { get; }

        ICommand GenerateReportCommand { get; }

        ICommand SaveProjectCommand { get; }

        ICommand OpenProjectCommand { get; }

        ICommand CreateProjectCommand { get; }

        ICommand Calculate { get; }

        string OptionType { get; set; }

        string OptionView { get; set; }

        double InitialStock { get; set; }

        double Volatility { get; set; }

        double StrikeValue { get; set; }

        double MaturityTime { get; set; }

        double RiskFreeInterestRate { get; set; }

        double DividendYield { get; set; }

        double NumberOfAssets { get; set; }
    }
}
