using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.IntegrityModule.ControlClasses;
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
using Wpf.Ui.Controls;
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
