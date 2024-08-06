using JoelGuiPOC1.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.Views.Pages
{
    public partial class BlacklistPage : INavigableView<BlacklistViewModel>
    {
        public BlacklistViewModel ViewModel { get; }

        public BlacklistPage(BlacklistViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
