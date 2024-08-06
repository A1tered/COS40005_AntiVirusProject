using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace JoelGuiPOC1.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Simple Antivirus";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },

            new NavigationViewItem()
            {
                Content = "Scan",
                Icon = new SymbolIcon { Symbol = SymbolRegular.SearchShield20},
                TargetPageType = typeof(Views.Pages.ScannerPage)
            },

            new NavigationViewItem()
            {
                Content = "Protection History",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ReceiptSearch20},
                TargetPageType = typeof(Views.Pages.ProtectionHistoryPage)
            },

            new NavigationViewItem()
            {
                Content = "Quarantined Items",
                TargetPageType = typeof(Views.Pages.QuarantinedItemsPage),
                Visibility = Visibility.Collapsed
            },

            new NavigationViewItem()
            {
                Content = "Mark as Malicious",
                TargetPageType = typeof(Views.Pages.BlacklistPage),
                Visibility = Visibility.Collapsed
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
