using GUIApplication.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace GUIApplication.Views.Pages
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

        private void CheckBox_Checked()
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
