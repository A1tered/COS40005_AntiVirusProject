using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.MaliciousCodeScanning;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.FileQuarantine;
using Wpf.Ui.Controls;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ScannerPage : INavigableView<ScannerViewModel>
    {
        // Alerts
        private readonly AlertManager _alertManager;
        private readonly EventBus _eventBus;

        // File hash scanner
        private CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _token;
        private FileHashScanner _fileHashScanner;

        // Malicious code scanner
        private MaliciousCodeScanner _maliciousCodeScanner;
        private DatabaseHandler _databaseHandler;
        private Detector _detector;

        // File quarantine
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;
        private IDatabaseManager _databaseManager;

        // ScannerPage GUI
        private List<string> _customList;
        public ScannerViewModel ViewModel { get; }

        public ScannerPage(ScannerViewModel viewModel, AlertManager alertManager, EventBus eventBus)
        {
            // initialise ScannerPage
            DataContext = viewModel;
            InitializeComponent();
            ViewModel = viewModel;

            _customList = new List<string>();

            // Initialise Alerts for page
            _alertManager = alertManager;
            _eventBus = eventBus;

            // Initialise File Quarantine
            _fileMover = new FileMover();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, "C:\\ProgramData\\SimpleAntiVirus\\Quarantine");

            // Initialise Malicious Code
            _databaseHandler = new DatabaseHandler(Path.Combine(AppContext.BaseDirectory, "Databases"));
            _detector = new Detector(_databaseHandler);
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsScanRunning = true;
            try
            {
                Debug.WriteLine($"Scan running: {ViewModel.IsScanRunning}");
                _cancellationTokenSource = new CancellationTokenSource();
                _fileHashScanner = new FileHashScanner(_alertManager, _eventBus, _cancellationTokenSource.Token, _quarantineManager);
                _maliciousCodeScanner = new MaliciousCodeScanner(_alertManager, _eventBus, _databaseHandler, _detector, _cancellationTokenSource.Token, _quarantineManager);

                if (QuickScanButton.IsChecked == true)
                {
                    Task quickScanFileHash = _fileHashScanner.Scan("quick", null);
                    Task quickScanMalCode = _maliciousCodeScanner.Scan("quick", null);
                    await Task.WhenAll(quickScanFileHash, quickScanMalCode);
                    if (ViewModel.IsScanRunning)
                    {
                        System.Windows.MessageBox.Show($"Quick scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (FullScanButton.IsChecked == true)
                {
                    Task fullScanFileHash = _fileHashScanner.Scan("full", null);
                    Task fullScanMalCode = _maliciousCodeScanner.Scan("full", null);
                    await Task.WhenAll(fullScanFileHash, fullScanMalCode);
                    if (ViewModel.IsScanRunning)
                    {
                        System.Windows.MessageBox.Show($"Full scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (CustomScanButton.IsChecked == true)
                {
                    ViewModel.IsScanRunning = false;
                    Task customscanFileHash = _fileHashScanner.Scan("custom", _customList);
                    Task customscanMalCode = _maliciousCodeScanner.Scan("custom", _customList);
                    if (_customList.Count == 0 || _customList == null)
                    {
                        System.Windows.MessageBox.Show("List of files and folders to scan cannot be empty.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ViewModel.IsScanRunning = true;
                        ViewModel.IsAddFolderButtonVisible = false;
                        await Task.WhenAll(customscanFileHash, customscanMalCode);
                        if (_customList != null && _customList.Count > 0 && ViewModel.IsScanRunning)
                        {
                            System.Windows.MessageBox.Show($"Custom scan completed!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        ViewModel.IsAddFolderButtonVisible = true;
                        _customList.Clear();
                    }
                }
                else
                {
                    // No scan option selected.
                }
            }
            catch (OperationCanceledException)
            {

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

        private void CancelScanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsScanRunning = false;
            _customList.Clear();
            ViewModel.IsAddFolderButtonVisible= true;
            Debug.WriteLine("Cancelling scan");
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                System.Windows.MessageBox.Show("Scan cancelled.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (_customList.Contains(folderGet))
                {
                    System.Windows.MessageBox.Show($"Error adding folder: Folder {folderGet} already in custom scan list.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else
                {
                    _customList.Add(folderGet);
                    System.Windows.MessageBox.Show($"Custom Scan: Folder {folderGet} selected.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }
    }
}
