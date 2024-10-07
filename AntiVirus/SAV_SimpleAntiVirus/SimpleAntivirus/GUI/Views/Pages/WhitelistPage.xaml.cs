/**************************************************************************
* File:        WhitelistPage.xaml.cs
* Author:      Joel Parks
* Description: Allows the reversal of whitelisted items
* Last Modified: 8/10/2024
**************************************************************************/

using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public class WhitelistEntry
    {
        public required string FilePath { get; set; }
    }

    public partial class WhitelistPage : INavigableView<WhitelistViewModel>
    {
        public WhitelistViewModel ViewModel { get; }
        private ObservableCollection<WhitelistEntry> _entries;
        private IDatabaseManager _databaseManager;

        public WhitelistPage(WhitelistViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEntries();
        }

        private async void UpdateEntries()
        {
            var entries = await _databaseManager.GetWhitelistAsync();
            _entries = new ObservableCollection<WhitelistEntry>(
                entries.Select(filePath => new WhitelistEntry
                {
                    FilePath = filePath
                }).ToList());

            WhitelistDataGrid.ItemsSource = _entries;
        }

        private void WhitelistSelected(object sender, RoutedEventArgs e)
        {
            if (WhitelistDataGrid.SelectedItem != null)
            {
                List<WhitelistEntry> selectedItems = WhitelistDataGrid.SelectedItems.Cast<WhitelistEntry>().ToList();
                int allItemCount = WhitelistDataGrid.Items.Count;
                string infoText = "";
                if (!(allItemCount == selectedItems.Count) || selectedItems.Count == 1)
                {
                    ViewModel.AllSelected = false;
                    if (selectedItems.Count() == 1)
                    {
                        infoText = $"Selected: {selectedItems[0].FilePath}";
                    }
                    else
                    {
                        infoText = $"Selected: {selectedItems.Count()} Items";
                    }
                    // Remove final comma.
                    infoText.Remove(infoText.Length - 1, 1);
                    ViewModel.PathSelected = selectedItems;
                }
                else
                {
                    infoText = $"All Items Selected ({allItemCount} Items)";
                    ViewModel.AllSelected = true;
                    ViewModel.PathSelected = selectedItems;
                }
                SelectLabel.Text = infoText;
            }
            else
            {
                SelectLabel.Text = "No Item Selected";
                ViewModel.PathSelected = null;
            }
        }

        private async void Unwhitelist_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult choice = System.Windows.MessageBox.Show("Are you sure you want to remove the selected items from the whitelist?", "Simple Antivirus", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (choice == System.Windows.MessageBoxResult.OK)
            {
                ViewModel.IsBusy = true;
                int result = await ViewModel.Unwhitelist();
                UpdateEntries();
                ViewModel.IsBusy = false;
                DisplayResultUnwhitelist(result);
                return;
            }
            DisplayResultUnwhitelist(4);
            ViewModel.IsBusy = false;
        }

        private void DisplayResultUnwhitelist(int result)
        {
            switch (result)
            {
                case 0:
                    System.Windows.MessageBox.Show("Whitelist Removal Failed: No item selected.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    System.Windows.MessageBox.Show("Whitelist Removal Failed: Quarantined file not found.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 2:
                    System.Windows.MessageBox.Show("Removal from Whitelist successful!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    System.Windows.MessageBox.Show("Whitelist Removal Partially Successful: Not all items were able to be removed from the whitelist. Please try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 4:
                    System.Windows.MessageBox.Show("Removal from Whitelist cancelled. The files will remain in the whitelist and if malicious, can pose a threat to your computer. You may choose to remove them from the whitelist at any time.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
                    break;
            }
        }
    }
}
