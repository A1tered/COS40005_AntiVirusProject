using SimpleAntivirus.GUI.ViewModels.Pages;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class ScanningPage : INavigableView<ScanningViewModel>
    {
        public ScanningViewModel ViewModel { get; }

        public ScanningPage(ScanningViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void Cancel_Scan(object sender, RoutedEventArgs e)
        {
            ScannerViewModel _viewModel = new ScannerViewModel();
            ScannerPage _scannerPage = new ScannerPage(_viewModel);
            this.NavigationService.Navigate(_scannerPage);
        }
    }
}
