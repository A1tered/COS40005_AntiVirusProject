using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using Microsoft.Win32;
using SimpleAntivirus.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace SimpleAntivirus.GUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for IntegrityTestPage.xaml
    /// </summary>
    public partial class IntegrityPage : Page
    {
        public IntegrityViewModel ViewModel { get; set; }

        private bool _adding;
        public IntegrityPage(IntegrityViewModel integViewModel)
        {
            ViewModel = integViewModel;
            DataContext = integViewModel;
            InitializeComponent();
            _adding = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // What to load...
            UpdateEntries();
        }

        // Updates data to be loaded onto the table.
        private void UpdateEntries(string searchTerm = null)
        {
            // We do not want to send another command to the database, whilst entries are being added
            if (!_adding)
            {
                ViewModel.GetEntries(searchTerm);
            }
            DataShow.ItemsSource = ViewModel.DataEntries;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // 0 > no selection, 1> no item found, 2 -> item deleted
            int resultInt = ViewModel.DeleteItem();
            DisplayResultDelete(resultInt);
            ViewModel.PathSelected = null;
        }

        // Handle message box for addition info
        private void DisplayResultOfAdded(bool success)
        {
            UpdateEntries();
            if (success)
            {
                MessageBox.Show("Data added to database", "Integrity Add Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Data failed to be added to database", "Integrity Add Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handle message box for addition info
        private void DisplayResultDelete(int returnId)
        {
            UpdateEntries();
            switch (returnId)
            {
                case 0:
                    MessageBox.Show("No Item Selected", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    MessageBox.Show("Data could not be found in Database", "Incorrect Entry", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 2:
                    MessageBox.Show("Data removed successfully", "Data Item Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    MessageBox.Show("Some data was removed successfully", "Some Data Item Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void DisplayLoading(bool start)
        {
            Rotator.BeginAnimation(RotateTransform.AngleProperty, null);
            if (start)
            {
                _adding = true;
                ViewModel.AddProgress = "";
                ProgressInfo.Visibility = Visibility.Visible;
                ProgressAdd.Visibility = Visibility.Visible;
                DoubleAnimation anim = new();
                anim.From = 0;
                anim.To = 360;
                anim.Duration = new Duration(TimeSpan.FromSeconds(2));
                anim.RepeatBehavior = RepeatBehavior.Forever;
                Rotator.BeginAnimation(RotateTransform.AngleProperty, anim);
            }
            else
            {
                _adding = false;
                ProgressInfo.Visibility = Visibility.Hidden;
                ProgressAdd.Visibility = Visibility.Hidden;
            }
        }

        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.ShowDialog();
            string fileGet = fileDialog.FileName;
            if (fileGet != "")
            {
                result = await ViewModel.AddIntegrityPath(fileGet);
                DisplayResultOfAdded(result);
            }
        }



        // Button that opens folder dialog to be sent to ViewModel.
        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            OpenFolderDialog folderDialog = new Microsoft.Win32.OpenFolderDialog();
            folderDialog.ShowDialog();
            string folderGet = folderDialog.FolderName;
            // Start load bar
            // Send to view model the path of folder.
            if (folderGet != "")
            {
                DisplayLoading(true);
                result = await ViewModel.AddIntegrityPath(folderGet);
                DisplayLoading(false);
                DisplayResultOfAdded(result);
            }
        }

        // This is triggered when the table is selected.
        private void DataShow_Selected(object sender, RoutedEventArgs e)
        {
            if (DataShow.SelectedItem != null)
            {
                List<DataRow> selectedItems = DataShow.SelectedItems.Cast<DataRow>().ToList();
                string infoText = "";
                List<string> selectedDirectories = new();
                if (selectedItems.Count() == 1)
                {
                    infoText = $"Selected: {selectedItems[0].DisplayDirectory}";
                }
                else
                {
                    infoText = $"Selected: {selectedItems.Count()} Items";
                }
                foreach (DataRow datarowItem in selectedItems)
                {
                    selectedDirectories.Add(datarowItem.HiddenDirectory);
                }
                // Remove final comma.
                infoText.Remove(infoText.Length - 1, 1);
                SelectLabel.Text = infoText;
                ViewModel.PathSelected = selectedDirectories;
            }
            else
            {
                SelectLabel.Text = "None Selected";
                ViewModel.PathSelected = null;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text.Length <= 0)
            {
                UpdateEntries();
            }
            else
            {
                UpdateEntries(SearchBox.Text);
            }
        }

        // Integrity Scanning Section
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.ScanInUse)
            {
                int result = await ViewModel.Scan();
                if (result > 0)
                {
                    ViolationNote.Foreground = new SolidColorBrush(Colors.Red);
                    ViolationNote.Text = $"Violations Found: {result}";
                    ResultsButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ViolationNote.Foreground = null;
                    ViolationNote.Text = "No Violations Found";
                }
            }
        }

        private void See_Results_Click(object sender, RoutedEventArgs e)
        {
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(IntegrityResultsPage));
        }
    }
}
