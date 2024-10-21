/**************************************************************************
 * File:        AlertReportPage.xaml.cs
 * Author:      Joel Parks & Christopher Thompson
 * Description: Gives specific detail in relation to a single alert.
 * Last Modified: 21/10/2024
 **************************************************************************/
using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Services;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class AlertReportPage : INavigableView<ProtectionHistoryViewModel>
    {
        public ProtectionHistoryViewModel ViewModel { get; }

        public AlertReportPage(ProtectionHistoryViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AlertRow alertRowItem = ViewModel.SelectedRow;
            TimeStampBlock.Text = $"{alertRowItem.TimeStamp}";
            ThreatType.Text = $"Category: {alertRowItem.Component}";
            Severity.Text = $"Severity: {alertRowItem.Severity}";
            Message.Text = $"Message: {alertRowItem.EntireMessage}";
            SuggestedAction.Text = $"Suggested Action: {alertRowItem.SuggestedAction}";
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(ProtectionHistoryPage));
        }
    }
}
