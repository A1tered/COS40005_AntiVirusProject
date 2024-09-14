using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using SimpleAntivirus.Alerts;

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

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private async void Send_Alert(object sender, RoutedEventArgs e)
        {
            await _eventBus.PublishAsync("Test", "Informational", "This is a test alert", "Disregard this alert");
        }
    }
}
