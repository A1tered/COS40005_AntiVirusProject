using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using System.IO;
using System.Diagnostics;
using System.DirectoryServices;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public class Entry
    {
        public required int Id { get; set; }
        public required string OriginalFilePath { get; set; }
        public required string QuarantineDate { get; set; }
    }

    public partial class QuarantinedItemsPage : INavigableView<QuarantinedViewModel>
    {
        public QuarantinedViewModel ViewModel { get; }
        private ObservableCollection<Entry> _entries;
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;
        private IDatabaseManager _databaseManager;

        public QuarantinedItemsPage(QuarantinedViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
            _fileMover = new FileMover();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quarantine"));
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEntries();
        }

        private async void UpdateEntries()
        {
            var entries = await _quarantineManager.GetQuarantinedFileDataAsync();
            _entries = new ObservableCollection<Entry>(
                entries.Select(e => new Entry
                {
                    Id = e.Id,
                    OriginalFilePath = e.OriginalFilePath,
                    QuarantineDate = e.QuarantineDate,
                }).ToList());

            QuarantinedItemsDataGrid.ItemsSource = _entries;
        }

        private void QuarantinedSelected(object sender, RoutedEventArgs e)
        {
            if (QuarantinedItemsDataGrid.SelectedItem != null)
            {
                List<Entry> selectedItems = QuarantinedItemsDataGrid.SelectedItems.Cast<Entry>().ToList();
                int allItemCount = QuarantinedItemsDataGrid.Items.Count;
                string infoText = "";
                if (!(allItemCount == selectedItems.Count) || selectedItems.Count == 1)
                {
                    ViewModel.AllSelected = false;
                    if (selectedItems.Count() == 1)
                    {
                        infoText = $"Selected: {selectedItems[0].OriginalFilePath}";
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
                }
                SelectLabel.Text = infoText;
            }
            else
            {
                SelectLabel.Text = "No Item Selected";
                ViewModel.PathSelected = null;
            }
        }

        private async void Unquarantine_Click(object sender, RoutedEventArgs e)
        {
            int result = await ViewModel.Unquarantine();
            UpdateEntries();
            DisplayResultUnquarantine(result);
        }

        private async void DisplayResultUnquarantine(int result)
        {
            switch (result)
            {
                case 0:
                    System.Windows.MessageBox.Show("Unquarantine Failed: No item selected.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    await ViewModel.EventBus.PublishAsync("File Quarantine", "Informational", "Unquarantine Failed: No item selected", "Select file(s) to unquarantine and try again.");
                    break;
                case 1:
                    System.Windows.MessageBox.Show("Unquarantine Failed: Quarantined file not found.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    await ViewModel.EventBus.PublishAsync("File Quarantine", "Informational", "Unquarantine Failed: Quarantined file not found", "Please try unquarantining the file again.");
                    break;
                case 2:
                    System.Windows.MessageBox.Show("Unquarantine successful!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    await ViewModel.EventBus.PublishAsync("File Quarantine", "Informational", "Unquarantine Successful!", "Consider whitelisting the file if you do not wish for it to be quarantined again.");
                    break;
                case 3:
                    System.Windows.MessageBox.Show("Unquarantine Partially Successful: Not all items were able to be unquarantined. Please try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                    await ViewModel.EventBus.PublishAsync("File Quarantine", "Warning", "Unquarantine Partially Successful: Not all items were able to be unquarantined. Please try again.", "Please try unquarantining files again.");
                    break;
            }
        }

        private void Whitelist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
