using JoelGuiPOC1.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.Views.Pages
{
    public partial class ProtectionHistoryPage : INavigableView<ProtectionHistoryViewModel>
    {
        public ProtectionHistoryViewModel ViewModel { get; }

        public ProtectionHistoryPage(ProtectionHistoryViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
