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

        private void DarkModeChange(ToggleSwitch toggle)
        {
            toggle.Content = "Dark theme enabled";
            ViewModel.OnChangeTheme("theme_dark");
        }

        private void LightModeChange(ToggleSwitch toggle)
        {
            toggle.Content = "Light theme enabled";
            ViewModel.OnChangeTheme("theme_light");
        }

        private void DarkModeEnabled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewModel = new SettingsViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                DarkModeChange(toggleSwitch);
            }
        }

        private void LightModeEnabled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel viewModel = new SettingsViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                LightModeChange(toggleSwitch);
            }
        }



        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurrentTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark)
            {
                DarkModeChange(ThemeSwitch);
                ThemeSwitch.IsChecked = true;
            }
            else
            {
                LightModeChange(ThemeSwitch);
            }
        }
    }
}
