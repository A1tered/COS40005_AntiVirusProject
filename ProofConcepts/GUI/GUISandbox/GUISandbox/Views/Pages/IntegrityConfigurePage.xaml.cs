using DatabaseFoundations;
using GUISandbox.ViewModels.Pages;
using IntegrityModule.ControlClasses;
using Microsoft.Win32;
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
namespace GUISandbox.Views.Pages
{
    /// <summary>
    /// Interaction logic for IntegrityTestPage.xaml
    /// </summary>
    public partial class IntegrityConfigurePage : Page
    {
        public IntegrityConfigViewModel ViewModel { get; set; }

        private bool _adding;
        public IntegrityConfigurePage(IntegrityConfigViewModel integViewModel)
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
        private void UpdateEntries()
        {
            // We do not want to send another command to the database, whilst entries are being added
            if (!_adding)
            {
                ViewModel.GetEntries();
            }
            DataShow.ItemsSource = ViewModel.DataEntries;
            PageCount.Content = ViewModel.PageNumber;
        }

        // Previous Page button triggers update and changes view model property.
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PageNumber--;
            UpdateEntries();
        }

        // Next Page button triggers update and changes view model property.
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PageNumber++;
            UpdateEntries();
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
            if (success) {
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
            if (fileGet != null)
            {
                result = await ViewModel.AddIntegrityPath(fileGet);
            }
            DisplayResultOfAdded(result);
        }



        // Button that opens folder dialog to be sent to ViewModel.
        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            OpenFolderDialog folderDialog = new Microsoft.Win32.OpenFolderDialog();
            folderDialog.ShowDialog();
            string folderGet = folderDialog.FolderName;
            // Start load bar
            DisplayLoading(true);
            // Send to view model the path of folder.
            if (folderGet != null)
            {
                result = await ViewModel.AddIntegrityPath(folderGet);
            }
            DisplayLoading(false);
            DisplayResultOfAdded(result);
        }

        // This is triggered when the table is selected.
        private void DataShow_Selected(object sender, RoutedEventArgs e)
        {
            if (DataShow.SelectedItem != null)
            {
                string directory = ((DataRow)DataShow.SelectedItem).DisplayDirectory;
                string realDirectory = ((DataRow)DataShow.SelectedItem).HiddenDirectory;
                SelectLabel.Content = $"Selected: {directory}";
                ViewModel.PathSelected = realDirectory;
            }
            else
            {
                SelectLabel.Content = "None Selected";
                ViewModel.PathSelected = null;
            }
        }
    }
}
