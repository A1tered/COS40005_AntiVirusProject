using GUIApplication.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace GUIApplication.Views.Pages
{
    public partial class QuarantinedItemsPage : INavigableView<QuarantinedViewModel>
    {
        public QuarantinedViewModel ViewModel { get; }

        public QuarantinedItemsPage(QuarantinedViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
