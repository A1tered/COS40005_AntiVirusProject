using SimpleAntivirus.GUI.Views.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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


            Wpf.Ui.Controls.MenuItem scanMenuItem = new();
            scanMenuItem.Header = "Scan Page";
            scanMenuItem.Click += ClickScan;
            contextMenuCreate.Items.Add(scanMenuItem);

            Wpf.Ui.Controls.MenuItem integrityMenuItem = new();
            integrityMenuItem.Header = "Integrity Page";
            integrityMenuItem.Click += ClickIntegrity;
            contextMenuCreate.Items.Add(integrityMenuItem);

            Wpf.Ui.Controls.MenuItem quarantineMenuItem = new();
            quarantineMenuItem.Header = "Quarantine";
            quarantineMenuItem.Click += ClickQuarantine;
            contextMenuCreate.Items.Add(quarantineMenuItem);

            Wpf.Ui.Controls.MenuItem alertMenuItem = new();
            alertMenuItem.Header = "Alerts";
            alertMenuItem.Click += ClickAlerts;
            contextMenuCreate.Items.Add(alertMenuItem);

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
            NavigationServiceIntermediary.NavigationService.Navigate(typeof(AlertReportPage));
        }


        // Properly close program.
        public void ClickExit(object sender, RoutedEventArgs e)
        {
            MainWindow window = _naviWindow as MainWindow;
            window.CloseWindowForcefully();
        }

        // TODO: Implement safer way to dispose this (if required)!

        //public void Dispose()
        //{
        //    _notifyIcon.Unregister();
        //    Dispose(true);
        //}
    }
}
