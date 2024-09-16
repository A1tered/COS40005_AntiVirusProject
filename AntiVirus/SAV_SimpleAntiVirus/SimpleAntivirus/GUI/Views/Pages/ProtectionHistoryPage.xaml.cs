using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Services;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ProtectionHistoryPage : INavigableView<ProtectionHistoryViewModel>
    {
        private readonly EventBus _eventBus;
        private readonly AlertManager _alertManager;

        public ProtectionHistoryViewModel ViewModel { get; }

        public ProtectionHistoryPage(ProtectionHistoryViewModel viewModel, AlertManager alertManager, EventBus eventBus)
        {
            ViewModel = viewModel;
            _alertManager = alertManager;
            _eventBus = eventBus;
            DataContext = this;

            InitializeComponent();
        }

        private void CheckBox_Checked()
        {

        }

        private void DataShow_Selected(object sender, RoutedEventArgs e)
        {
            if (ProtectionHistoryDataGrid.SelectedItem != null)
            {
                DetailsButton.Visibility = Visibility.Visible;
                // Cast to Object
                ViewModel.SelectedRow = (AlertRow)ProtectionHistoryDataGrid.SelectedItem;
            }
            else
            {
                DetailsButton.Visibility = Visibility.Hidden;
            }
        }

        private async void Send_Alert(object sender, RoutedEventArgs e)
        {
            await _eventBus.PublishAsync("Test", "Informational", "This is a test alert", "Disregard this alert");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProtectionHistoryDataGrid.ItemsSource = await ViewModel.GetEntries();
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(AlertReportPage));
        }
    }
}
