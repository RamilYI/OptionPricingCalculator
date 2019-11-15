using MahApps.Metro.Controls;

namespace OptionPricingCalculator.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для HelpView.xaml.
    /// </summary>
    public partial class HelpView : MetroWindow
    {
        public HelpView(string aboutApplication, string applicationFeatures)
        {
            this.InitializeComponent();
            this.aboutApplication.Text = aboutApplication;
            this.applicationFeatures.Text = applicationFeatures;
        }
    }
}
