﻿/**************************************************************************
 * File:        QuarantinedItemsPage.xaml.cs
 * Author:      Joel Parks
 * Description: Handles quarantine page, where users can quarantine, whitelist, delete.
 * Last Modified: 21/10/2024
 **************************************************************************/

using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public class Entry
    {
        public required int Id { get; set; }
        public required string QuarantinedFilePath { get; set; }
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
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, "C:\\ProgramData\\SimpleAntiVirus\\Quarantine");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateEntries();
        }

        private async void UpdateEntries()
        {
            var entries = await _quarantineManager.GetQuarantinedFilesAsync();
            _entries = new ObservableCollection<Entry>(
                entries.Select(e => new Entry
                {
                    Id = e.Id,
                    QuarantinedFilePath = e.QuarantinedFilePath,
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

        private async void Unquarantine_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult choice = System.Windows.MessageBox.Show("Are you sure you want to unquarantine the selected items?", "Simple Antivirus", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (choice == System.Windows.MessageBoxResult.OK)
            {
                ViewModel.IsBusy = true;
                int result = await ViewModel.Unquarantine();
                UpdateEntries();
                ViewModel.IsBusy = false;
                DisplayResultUnquarantine(result);
                return;
            }
            DisplayResultUnquarantine(4);
            ViewModel.IsBusy = false;
        }

        private void DisplayResultUnquarantine(int result)
        {
            switch (result)
            {
                case 0:
                    System.Windows.MessageBox.Show("Unquarantine Failed: No item selected.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    System.Windows.MessageBox.Show("Unquarantine Failed: Quarantined file not found.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 2:
                    System.Windows.MessageBox.Show("Unquarantine successful!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    System.Windows.MessageBox.Show("Unquarantine Partially Successful: Not all items were able to be unquarantined. Please try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 4:
                    System.Windows.MessageBox.Show("Unquarantine cancelled. The quarantined files are still present on your computer however do not pose a threat while in quarantine. You may choose to unquarantine, whitelist or delete them at any time.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
                    break;
            }
        }

        private async void Whitelist_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult choice = System.Windows.MessageBox.Show("Are you sure you want to whitelist the selected items?", "Simple Antivirus", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (choice == System.Windows.MessageBoxResult.OK)
            {
                ViewModel.IsBusy = true;
                await ViewModel.Unquarantine();
                int result = await ViewModel.Whitelist();
                UpdateEntries();
                ViewModel.IsBusy = false;
                DisplayResultWhitelist(result);
                return;
            }
            DisplayResultWhitelist(4);
            ViewModel.IsBusy = false;
        }

        private void DisplayResultWhitelist(int result)
        {
            switch (result)
            {
                case 0:
                    System.Windows.MessageBox.Show("Whitelisting Failed: No item selected.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    System.Windows.MessageBox.Show("Whitelisting Failed: Quarantined file not found.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 2:
                    System.Windows.MessageBox.Show("Whitelisting successful!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    System.Windows.MessageBox.Show("Whitelisting Partially Successful: Not all items were able to be whitelisted. Please try again.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case 4:
                    System.Windows.MessageBox.Show("Whitelisting cancelled. The quarantined files are still present on your computer however do not pose a threat while in quarantine. You may choose to unquarantine, whitelist or delete them at any time.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
                    break;
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBoxResult choice = System.Windows.MessageBox.Show("Are you sure you want to delete the selected items? These files will be deleted permanently and will NOT be sent to the Recycle Bin!", "Simple Antivirus", System.Windows.MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (choice  == System.Windows.MessageBoxResult.OK)
            {
                ViewModel.IsBusy = true;
                int result = await ViewModel.Delete();
                UpdateEntries();
                ViewModel.IsBusy = false;
                DisplayResultDelete(result);
                return;
            }
            DisplayResultDelete(4);
            ViewModel.IsBusy = false;
        }

        private void DisplayResultDelete(int result)
        {
            switch (result)
            {
                case 0:
                    System.Windows.MessageBox.Show("Delete Failed: No Item Selected", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    System.Windows.MessageBox.Show("Delete Failed: Quarantined file not found.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 2:
                    System.Windows.MessageBox.Show("Quarantined file successfully deleted!", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    System.Windows.MessageBox.Show("Delete Partially Successful: Not all items were able to be deleted. Please try again.", "SimpleAntivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 4:
                    System.Windows.MessageBox.Show("Delete operation cancelled. The quarantined files are still present on your computer however do not pose a threat while in quarantine. You may choose to delete them at any time.", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Stop);
                    break;
            }
        }
    }
}
