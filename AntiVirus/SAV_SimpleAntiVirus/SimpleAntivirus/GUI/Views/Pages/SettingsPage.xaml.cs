using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void DarkModeEnabled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewModel = new SettingsViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                toggleSwitch.Content = "Dark theme enabled";
                viewModel.OnChangeTheme("theme_dark");
            }
        }

        private void LightModeEnabled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewModel = new SettingsViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                toggleSwitch.Content = "Light theme enabled";
                viewModel.OnChangeTheme("theme_light");
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
