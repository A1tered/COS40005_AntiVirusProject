using SimpleAntivirus.FileHashScanning;
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
            ScanningViewModel _scanningViewModel = new ScanningViewModel();
            ScanningPage _scanningPage = new ScanningPage(_scanningViewModel);
            ScannerViewModel _scannerViewModel = new ScannerViewModel();
            ScannerPage _scannerPage = new ScannerPage(_scannerViewModel);

            if (QuickScanButton.IsChecked == true)
            {
                this.NavigationService.Navigate(_scanningPage);
                await _fileHashScanner.Scan("quick");
                this.NavigationService.GoBack();
            }
            else if (FullScanButton.IsChecked == true)
            {
                this.NavigationService.Navigate(_scanningPage);
                await _fileHashScanner.Scan("full");
                this.NavigationService.Navigate(_scannerPage);
            }
            else if (CustomScanButton.IsChecked == true)
            {
                this.NavigationService.Navigate(_scanningPage);
                await _fileHashScanner.Scan("custom");
            }
            else
            {
               // No scan option selected.
            }
        }

    }
}
