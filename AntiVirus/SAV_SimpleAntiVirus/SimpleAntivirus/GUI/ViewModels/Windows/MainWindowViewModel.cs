using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.ViewModels.Windows
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
                Icon = new SymbolIcon { Symbol = SymbolRegular.AlertBadge20},
                TargetPageType = typeof(Views.Pages.ProtectionHistoryPage)
            },

            new NavigationViewItem()
            {
                Content="Integrity Checking",
                Icon = new SymbolIcon {Symbol = SymbolRegular.DesktopCheckmark20},
                TargetPageType = typeof(Views.Pages.IntegrityPage)
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
            },
            
            new NavigationViewItem()
            {
                Content = "Whitelist",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ShieldCheckmark20},
                TargetPageType = typeof(Views.Pages.WhitelistPage),
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
