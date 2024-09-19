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

            Wpf.Ui.Controls.MenuItem exitMenuItem = new();
            exitMenuItem.Header = "Exit";
            exitMenuItem.Click += ClickExit;
            contextMenuCreate.Items.Add(exitMenuItem);

            _naviWindow = naviWindow;

            _notifyIcon = notifyService;
            _notifyIcon.Register();
            _notifyIcon.ContextMenu = contextMenuCreate;
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
