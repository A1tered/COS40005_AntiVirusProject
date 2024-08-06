using JoelGuiPOC1.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.Views.Pages
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

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void QuarantinedItemsLinkClicked(object sender, RoutedEventArgs e)
        {
            QuarantinedViewModel _viewModel = new QuarantinedViewModel();
            QuarantinedItemsPage _quarantinedItemsPage = new QuarantinedItemsPage(_viewModel);
            this.NavigationService.Navigate(_quarantinedItemsPage);
        }

        private void BlacklistLinkClicked(object sender, RoutedEventArgs e)
        {
            BlacklistViewModel _viewModel = new BlacklistViewModel();
            BlacklistPage _blacklistPage = new BlacklistPage(_viewModel);
            this.NavigationService.Navigate(_blacklistPage);
        }

    }
}
