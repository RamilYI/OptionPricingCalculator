using System.Windows;
using OptionPricingCalculator.ViewModels;
using OptionPricingCalculator.Views;
using MainView = OptionPricingCalculator.Views.Windows.MainView;

namespace OptionPricingCalculator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public App() => this.InitializeComponent();

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            var mainViewModel = new MainViewModel();
            var window = new MainView { DataContext = mainViewModel };
            window.Closed += delegate { this.Shutdown(); };
            window.Show();
        }
    }
}
