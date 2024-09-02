using DatabaseFoundations;
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
            ViewModel.GetEntries(0);
            DataShow.ItemsSource = ViewModel.DataEntries;
        }
    }
}
