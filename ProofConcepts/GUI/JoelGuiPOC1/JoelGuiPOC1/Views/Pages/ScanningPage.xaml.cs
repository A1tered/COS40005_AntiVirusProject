using JoelGuiPOC1.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.Views.Pages
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
    }
}
