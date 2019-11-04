using System.Windows;
using MahApps.Metro.Controls;
using OptionPricingCalculator.ViewModels.Interfaces;
using ReactiveUI;

namespace OptionPricingCalculator.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow, IViewFor<IMainViewModel>
    {
        public MainView()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) => this.ViewModel = this.DataContext as IMainViewModel;
            this.WhenActivated(disposable => { });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(IMainViewModel), typeof(MainView), null);

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (IMainViewModel)value;
        }

        public IMainViewModel ViewModel
        {
            get => (IMainViewModel)this.GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
    }
}
