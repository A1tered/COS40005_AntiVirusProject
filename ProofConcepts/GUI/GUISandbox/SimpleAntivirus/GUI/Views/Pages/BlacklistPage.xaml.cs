using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
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
