using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptionPricingCalculator.ViewModels.Interfaces
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        int NumOfSimulations { get; set; }

        int GridOfTime { get; set; }

        bool IsDeltaEnabled { get; set; }

        bool IsGammaEnabled { get; set; }

        bool IsVegaEnabled { get; set; }

        bool IsRhoEnabled { get; set; }

        bool IsThetaEnabled { get; set; }

        bool IsParallel { get; set; }
    }
}
