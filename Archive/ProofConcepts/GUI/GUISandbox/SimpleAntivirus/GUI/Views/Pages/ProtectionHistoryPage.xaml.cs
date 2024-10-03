using SimpleAntivirus.GUI.ViewModels.Pages;
using System.Windows.Controls;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Services;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ProtectionHistoryPage : Page
    {
        private readonly EventBus _eventBus;

        public ProtectionHistoryViewModel ViewModel { get; }

        public ProtectionHistoryPage(ProtectionHistoryViewModel viewModel, EventBus eventBus)
        {
            ViewModel = viewModel;
            _eventBus = eventBus;
            DataContext = this;
            InitializeComponent();
            ViewModel.AlertPropagate += HandlePropagation;
        }

        // Taking the event from Alerts, now update the datagrid.
        private async void HandlePropagation(object? sender, EventArgs e)
        {
            await this.Dispatcher.Invoke(() => RefreshDatagrid());
        }

        // Update datagrid with contents of database.
        private async Task RefreshDatagrid()
        {
            
            // TODO: Fix this! SortDescription effect disappears when page is reloaded!
            ProtectionHistoryDataGrid.Items.SortDescriptions.Clear();
            ProtectionHistoryDataGrid.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("TimeStamp", System.ComponentModel.ListSortDirection.Descending));
            ProtectionHistoryDataGrid.ItemsSource = await ViewModel.GetEntries();
            ViewModel.SelectedRow = null;
            DetailsButton.Visibility = Visibility.Hidden;
            ItemAmount.Text = $"{ProtectionHistoryDataGrid.Items.Count} Items";
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

        

        // When the page loads, refresh the datagrid.
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshDatagrid();
        }

        // Go to details page, which will access data stored in the ViewModel.
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(AlertReportPage));
        }

        // Clear the alert database, and then refresh the datagrid. 
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ClearAlertDatabase();
            await RefreshDatagrid();
            MessageBox.Show("Alerts Cleared", "Protection History", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
