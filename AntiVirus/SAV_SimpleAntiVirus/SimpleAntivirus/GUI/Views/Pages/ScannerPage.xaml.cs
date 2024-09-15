using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using SimpleAntivirus.Alerts;
using Wpf.Ui.Controls;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Win32;
using System.ComponentModel;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ScannerPage : INavigableView<ScannerViewModel>
    {
        private readonly AlertManager _alertManager;
        private readonly EventBus _eventBus;
        private readonly CancellationToken _token;
        private FileHashScanner _fileHashScanner;
        private CancellationTokenSource _cancellationTokenSource;
        private List<string> _customList;
        public ScannerViewModel ViewModel { get; }

        public ScannerPage(ScannerViewModel viewModel, AlertManager alertManager, EventBus eventBus)
        {

            DataContext = viewModel;
            InitializeComponent();
            ViewModel = viewModel;
            _alertManager = alertManager;
            _eventBus = eventBus;
            _customList = new List<string>();
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsScanRunning = true;
            try
            {
                Debug.WriteLine($"Scan running: {ViewModel.IsScanRunning}");
                _cancellationTokenSource = new CancellationTokenSource();
                _fileHashScanner = new FileHashScanner(_alertManager, _eventBus, _cancellationTokenSource.Token);

                if (QuickScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("quick", null);
                    System.Windows.MessageBox.Show($"Quick scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FullScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("full", null);
                    System.Windows.MessageBox.Show($"Full scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (CustomScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("custom", _customList);
                    if (_customList != null && _customList.Count > 0)
                    {
                        System.Windows.MessageBox.Show($"Custom scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    _customList.Clear();
                }
                else
                {
                    // No scan option selected.
                }
            }
            catch (OperationCanceledException)
            {
                System.Windows.MessageBox.Show("Scan cancelled.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ViewModel.IsScanRunning = false;
            }
        }

        private async void CancelScanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsScanRunning = false;
            Debug.WriteLine("Cancelling scan");
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void CustomScanButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.IsAddFileButtonVisible = true;
            ViewModel.IsAddFolderButtonVisible = true;
        }

        private void CustomScanButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.IsAddFolderButtonVisible= false;
            ViewModel.IsAddFileButtonVisible= false;
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            OpenFolderDialog folderDialog = new Microsoft.Win32.OpenFolderDialog();
            folderDialog.ShowDialog();
            string folderGet = folderDialog.FolderName;
            // Start load bar
            // Send to view model the path of folder.
            if (folderGet != "")
            {
                _customList.Add(folderGet);
                System.Windows.MessageBox.Show($"Folder {folderGet} selected.", "Custom scan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
