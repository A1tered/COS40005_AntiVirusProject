using DatabaseFoundations;
using GUISandbox.ViewModels.Pages;
using IntegrityModule.ControlClasses;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        public IntegrityConfigurePage(IntegrityConfigViewModel integViewModel)
        {
            ViewModel = integViewModel;
            DataContext = integViewModel;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // What to load...
            ViewModel.GetEntries();
            DataShow.ItemsSource = ViewModel.DataEntries;
        }

        // Updates data to be loaded onto the table.
        private void UpdateEntries()
        {
            ViewModel.GetEntries();
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

        }

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

        private async void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.ShowDialog();
            string fileGet = fileDialog.FileName;
            bool result = await ViewModel.AddIntegrityPath(fileGet);
            DisplayResultOfAdded(result);
        }

        // Button that opens folder dialog to be sent to ViewModel.
        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            //OpenFolderDialog folderDialog = new Microsoft.Win32.OpenFolderDialog();
            //folderDialog.ShowDialog();
            //string folderGet = folderDialog.FolderName;
            //bool result = await ViewModel.AddIntegrityPath(folderGet);
            //DisplayResultOfAdded(result);
        }

        // This is triggered when the table is selected.
        private void DataShow_Selected(object sender, RoutedEventArgs e)
        {
            if (DataShow.SelectedItem != null)
            {
                SelectLabel.Content = $"Selected: {((DataRow)DataShow.SelectedItem).Directory}";
            }
            else
            {
                SelectLabel.Content = "None Selected";
            }
        }
    }
}
