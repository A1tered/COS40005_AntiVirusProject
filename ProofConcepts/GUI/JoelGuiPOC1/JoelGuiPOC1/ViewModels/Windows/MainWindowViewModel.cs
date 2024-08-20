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
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home20},
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
                Icon = new SymbolIcon { Symbol = SymbolRegular.History20},
                TargetPageType = typeof(Views.Pages.ProtectionHistoryPage)
            },

            new NavigationViewItem()
            {
                Content = "Quarantined Items",
                Icon = new SymbolIcon { Symbol = SymbolRegular.TextBulletListSquareShield20},
                TargetPageType = typeof(Views.Pages.QuarantinedItemsPage),
            },

            new NavigationViewItem()
            {
                Content = "Mark as Malicious",
                Icon = new SymbolIcon { Symbol = SymbolRegular.WarningShield20},
                TargetPageType = typeof(Views.Pages.BlacklistPage),
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
