using DatabaseFoundations;
using GUISandbox.Services;
using GUISandbox.ViewModels.Pages;
using IntegrityModule.ControlClasses;
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
using Wpf.Ui.Controls;
namespace GUISandbox.Views.Pages
{
    /// <summary>
    /// Interaction logic for IntegrityTestPage.xaml
    /// </summary>
    public partial class IntegrityTestPage : Page
    {
        public IntegrityViewModel ViewModel { get; set; }
        public IntegrityTestPage(IntegrityViewModel integViewModel)
        {
            ViewModel = integViewModel;
            DataContext = integViewModel;
            InitializeComponent();
        }

        // Represents scan button in integrity.
        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!ViewModel.ScanInUse)
        //    {
        //        int result = await ViewModel.Scan();
        //        if (result > 0)
        //        {
        //            ViolationNote.Foreground = new SolidColorBrush(Colors.Red);
        //            ViolationNote.Content = $"Violations Found: {result}";
        //            ResultsButton.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            ViolationNote.Foreground = new SolidColorBrush(Colors.White);
        //            ViolationNote.Content = "No Violations Found";
        //        }
        //    }
        //}

        //private void See_Results_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationServiceIntermediary.NavigationService.Navigate(typeof(IntegrityResultsPage));
        //}
    }
}
