using SimpleAntivirus.GUI.Views.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.ApplicationModel.VoiceCommands;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray;

namespace SimpleAntivirus.GUI.Services
{
    public class SystemTrayService
    {
        public INotifyIconService _notifyIcon;
        public INavigationWindow _naviWindow;
        public SystemTrayService(INotifyIconService notifyService, INavigationWindow naviWindow)
        {
            System.Diagnostics.Debug.WriteLine("System Tray Service Boot");
            // Create ContextMenu

            ContextMenu contextMenuCreate = new();

            Wpf.Ui.Controls.MenuItem homeMenuItem = new();
            homeMenuItem.Header = "Home";
            homeMenuItem.Click += ClickHome;
            contextMenuCreate.Items.Add(homeMenuItem);

            Wpf.Ui.Controls.MenuItem scanMenuItem = new();
            scanMenuItem.Header = "Run a scan";
            scanMenuItem.Click += ClickScan;
            contextMenuCreate.Items.Add(scanMenuItem);

            Wpf.Ui.Controls.MenuItem integrityMenuItem = new();
            integrityMenuItem.Header = "Integrity Checking";
            integrityMenuItem.Click += ClickIntegrity;
            contextMenuCreate.Items.Add(integrityMenuItem);

            Wpf.Ui.Controls.MenuItem alertMenuItem = new();
            alertMenuItem.Header = "View protection history";
            alertMenuItem.Click += ClickAlerts;
            contextMenuCreate.Items.Add(alertMenuItem);

            contextMenuCreate.Items.Add(new Separator());

            Wpf.Ui.Controls.MenuItem quarantineMenuItem = new();
            quarantineMenuItem.Header = "Manage quarantined items";
            quarantineMenuItem.Click += ClickQuarantine;
            contextMenuCreate.Items.Add(quarantineMenuItem);

            Wpf.Ui.Controls.MenuItem blacklistMenuItem = new();
            blacklistMenuItem.Header = "Mark items as Malicious";
            blacklistMenuItem.Click += ClickBlacklist;
            contextMenuCreate.Items.Add(blacklistMenuItem);


            Wpf.Ui.Controls.MenuItem whitelistMenuItem = new();
            whitelistMenuItem.Header = "Manage whitelisted files";
            whitelistMenuItem.Click += ClickWhitelist;
            contextMenuCreate.Items.Add(whitelistMenuItem);


            contextMenuCreate.Items.Add(new Separator());

            Wpf.Ui.Controls.MenuItem exitMenuItem = new();
            exitMenuItem.Header = "Exit";
            exitMenuItem.Click += ClickExit;
            contextMenuCreate.Items.Add(exitMenuItem);

            _naviWindow = naviWindow;

            _notifyIcon = notifyService;
            _notifyIcon.Register();
            _notifyIcon.ContextMenu = contextMenuCreate;
        }

        private void ShowWindow()
        {
            MainWindow window = _naviWindow as MainWindow;
            window.Show();
        }

        // Page load
        public void ClickHome(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(DashboardPage));
        }

        // Page Load
        public void ClickScan(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(ScannerPage));
        }

        // Page Load
        public void ClickIntegrity(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(IntegrityPage));
        }

        // Page Load
        public void ClickQuarantine(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(QuarantinedItemsPage));
        }

        // Page Load
        public void ClickAlerts(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(ProtectionHistoryPage));
        }

        // Page Load
        public void ClickBlacklist(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(BlacklistPage));
        }

        public void ClickWhitelist(object sender, RoutedEventArgs e)
        {
            ShowWindow();
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(WhitelistPage));
        }


        // Properly close program.
        public void ClickExit(object sender, RoutedEventArgs e)
        {
            MainWindow window = _naviWindow as MainWindow;
            window.CloseWindowGracefully();
        }

        // TODO: Implement safer way to dispose this (if required)!

        //public void Dispose()
        //{
        //    _notifyIcon.Unregister();
        //    Dispose(true);
        //}
    }
}
