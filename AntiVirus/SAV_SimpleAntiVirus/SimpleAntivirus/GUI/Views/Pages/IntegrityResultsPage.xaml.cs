/**************************************************************************
 * File:        IntegrityResultsPage.xaml.cs
 * Author:      Christopher Thompson
 * Description: Shows results of a recent integrity scan.
 * Last Modified: 21/10/2024
 **************************************************************************/

using SimpleAntivirus.GUI.Services;
using System.Windows.Controls;
using SimpleAntivirus.ViewModels.Pages;

namespace SimpleAntivirus.GUI.Views.Pages
{
    /// <summary>
    /// Interaction logic for IntegrityTestPage.xaml
    /// </summary>
    public partial class IntegrityResultsPage : Page
    {
        public IntegrityResultsViewModel ViewModel { get; set; }
        public IntegrityResultsPage(IntegrityResultsViewModel integViewModel)
        {
            ViewModel = integViewModel;
            DataContext = integViewModel;
            InitializeComponent();
        }

        // Handle message box for addition info

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataShow.ItemsSource = ViewModel.GetEntries();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(IntegrityPage));
        }
    }
}
