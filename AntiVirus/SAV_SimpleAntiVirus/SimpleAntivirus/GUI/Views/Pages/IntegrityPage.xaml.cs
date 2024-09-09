using SimpleAntivirus.GUI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class IntegrityPage : INavigableView<IntegrityViewModel>
    {
        public IntegrityViewModel ViewModel { get; }

        public IntegrityPage(IntegrityViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
