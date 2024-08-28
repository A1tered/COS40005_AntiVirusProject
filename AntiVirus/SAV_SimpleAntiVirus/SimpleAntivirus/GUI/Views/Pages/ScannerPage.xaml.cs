using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ScannerPage : INavigableView<ScannerViewModel>
    {
        public ScannerViewModel ViewModel { get; }

        public ScannerPage(ScannerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            FileHashScanner _fileHashScanner = new FileHashScanner();

            if (QuickScanButton.IsChecked == true)
            {
                NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScanningPage));
                await _fileHashScanner.Scan("quick");
                NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScannerPage));
            }
            else if (FullScanButton.IsChecked == true)
            {
                NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScanningPage));
                await _fileHashScanner.Scan("full");
                NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScannerPage));
            }
            else if (CustomScanButton.IsChecked == true)
            {
                NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScanningPage));
                await _fileHashScanner.Scan("custom");
            }
            else
            {
               // No scan option selected.
            }
        }

    }
}
