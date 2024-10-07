/**************************************************************************
* File:        BlacklistPage.xaml.cs
* Author:      Joel Parks
* Description: Allows files to be marked as malicious.
* Last Modified: 8/10/2024
**************************************************************************/

using Microsoft.Win32;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.AntiTampering;
using SimpleAntivirus.GUI.ViewModels.Pages;
using System.Text.RegularExpressions;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class BlacklistPage : INavigableView<BlacklistViewModel>
    {
        public BlacklistViewModel ViewModel { get; }
        private AlertManager _alertManager;
        private EventBus _eventBus;

        public BlacklistPage(BlacklistViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            _alertManager = new AlertManager();
            _eventBus = new EventBus(_alertManager);
        }

        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.ShowDialog();
            string fileGet = fileDialog.FileName;
            // Start AntiTampering Implementation
            if (InputValSan.FilePathCharLimit(fileGet) && InputValSan.FilePathValidation(fileGet))
            {
                if (fileGet != "")
                {
                    bool result = await ViewModel.BlacklistFile(fileGet, _alertManager, _eventBus);
                    DisplayResultFile(result, fileGet);
                }
            }
        }

        private void AddHash_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(AddHashTextBox.Text, "^[0-9a-fA-F]{32}$"))
            {
                AddHashTextBox.Clear();
                System.Windows.MessageBox.Show("The hash entered is an MD5 hash. Please enter a SHA1 hash and try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Regex.IsMatch(AddHashTextBox.Text, "^[0-9a-fA-F]{64}$"))
            {
                AddHashTextBox.Clear();
                System.Windows.MessageBox.Show("The hash entered is a SHA256 hash. Please enter a SHA1 hash and try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Regex.IsMatch(AddHashTextBox.Text, "^[0-9a-fA-F]{40}$"))
            {
                bool result = ViewModel.BlacklistHash(AddHashTextBox.Text);
                AddHashTextBox.Clear();
                DisplayResultManualBlacklist(result);
            }
            else if (AddHashTextBox.Text.Length == 0)
            {
                AddHashTextBox.Clear();
                System.Windows.MessageBox.Show("Hash cannot be blank. Please enter a SHA1 hash and try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                AddHashTextBox.Clear();
                System.Windows.MessageBox.Show("Invalid hash entered. Please enter a SHA1 hash and try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void DisplayResultManualBlacklist(bool result)
        {
            if (result)
            {
                System.Windows.MessageBox.Show($"Hash {AddHashTextBox.Text} has been successfully blacklisted!" , "Simple Antivirus" , System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                await _eventBus.PublishAsync("Mark as Malicious", "Informational", $"Hash {AddHashTextBox.Text} has been successfully blacklisted!", "Run a scan to ensure that files matching that hash are quarantined!");
            }
        }

        private async void DisplayResultFile(bool result, string fileGet)
        {
            if (result)
            {
                System.Windows.MessageBox.Show($"File {fileGet} has been successfully blacklisted!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                await _eventBus.PublishAsync("Mark as Malicious", "Informational", $"File {fileGet} has been successfully blacklisted!", "Run a scan to ensure that files matching that hash are quarantined!");
            }
        }
    }
}
