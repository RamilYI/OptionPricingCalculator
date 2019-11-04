using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionPricingCalculator.Models;
using OptionPricingCalculator.ViewModels.Interfaces;
using PropertyChanged;
using ReactiveUI;

namespace OptionPricingCalculator.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {

        public SettingsViewModel()
        {
        }
    }
}
