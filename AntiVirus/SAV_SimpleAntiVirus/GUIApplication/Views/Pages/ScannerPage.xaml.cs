using GUIApplication.ViewModels.Pages;
using GUIApplication.Views.Windows;
using Wpf.Ui.Controls;

namespace GUIApplication.Views.Pages
{
    public partial class ScannerPage : INavigableView<ScannerViewModel>
    {
        public ScannerViewModel ViewModel { get; }

        public ScannerPage(ScannerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScanningViewModel _viewModel = new ScanningViewModel();
            ScanningPage _scanningPage = new ScanningPage(_viewModel);
            this.NavigationService.Navigate(_scanningPage);
        }
    }
}
