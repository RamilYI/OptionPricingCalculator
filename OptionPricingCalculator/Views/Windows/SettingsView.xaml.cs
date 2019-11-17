using System.Windows;
using MahApps.Metro.Controls;
using OptionPricingCalculator.Common.Settings.Interfaces;
using ReactiveUI;

namespace OptionPricingCalculator.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml.
    /// </summary>
    public partial class SettingsView : MetroWindow, IViewFor<ISettingsViewModel>
    {
        public SettingsView()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) => this.ViewModel = this.DataContext as ISettingsViewModel;
            this.WhenActivated(disposable => { });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(ISettingsViewModel), typeof(SettingsView), null);

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (ISettingsViewModel)value;
        }

        public ISettingsViewModel ViewModel
        {
            get => (ISettingsViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
    }
}
