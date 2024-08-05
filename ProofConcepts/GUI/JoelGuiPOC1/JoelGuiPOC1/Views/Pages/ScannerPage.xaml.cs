using JoelGuiPOC1.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.Views.Pages
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
    }
}
