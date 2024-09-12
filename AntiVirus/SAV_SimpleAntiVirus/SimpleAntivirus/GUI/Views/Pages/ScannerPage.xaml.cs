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

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ScannerPage : INavigableView<ScannerViewModel>
    {
        private readonly AlertManager _alertManager;
        private readonly EventBus _eventBus;

        public ScannerViewModel ViewModel { get; }

        public ScannerPage(ScannerViewModel viewModel, AlertManager alertManager, EventBus eventBus)
        {
            InitializeComponent();

            DataContext = ViewModel;
            ViewModel = viewModel;

            _alertManager = alertManager;
            _eventBus = eventBus;
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsScanRunning = true;
            try
            {
                Debug.WriteLine($"Scan running: {ViewModel.IsScanRunning}");
                FileHashScanner _fileHashScanner = new FileHashScanner(_alertManager, _eventBus);

                if (QuickScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("quick");
                }
                else if (FullScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("full");
                }
                else if (CustomScanButton.IsChecked == true)
                {
                    await _fileHashScanner.Scan("custom");
                }
                else
                {
                    // No scan option selected.
                }
            }
            finally
            {
                ViewModel.IsScanRunning = false;
                Debug.WriteLine($"Scan running: {ViewModel.IsScanRunning}");
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

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.ShowDialog();
            string fileGet = fileDialog.FileName;
            if (fileGet != "")
            {
                System.Windows.MessageBox.Show($"File {fileGet} selected.", "Custom scan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
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
                System.Windows.MessageBox.Show($"Folder {folderGet} selected.", "Custom scan", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
