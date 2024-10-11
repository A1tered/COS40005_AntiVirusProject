using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.ViewModels.Windows;
using Wpf.Ui.Appearance;
﻿/**************************************************************************
* File:        DashboardPage.xaml.cs
* Author:      Joel Parks
* Description: Default page of GUI.
* Last Modified: 8/10/2024
**************************************************************************/

using System.IO;
using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.GUI.Services.Interface;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.Models;
using Wpf.Ui.Controls;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.GUI.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        private IDatabaseManager _databaseManager;
        private IntegrityHandlerModel _integrityHandlerModel;
        private AlertManager _alertManager;
        private EventBus _eventBus;

        // ViewModel
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void DarkModeChange(ToggleSwitch toggle)
        {
            toggle.Content = "Dark theme enabled";
            ViewModel.OnChangeTheme("theme_dark");
        }

        private void LightModeChange(ToggleSwitch toggle)
        {
            toggle.Content = "Light theme enabled";
            ViewModel.OnChangeTheme("theme_light");
        }

        private void DarkModeEnabled(object sender, RoutedEventArgs e)
        {
            DashboardViewModel viewModel = new DashboardViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                DarkModeChange(toggleSwitch);
            }
        }

        private void LightModeEnabled(object sender, RoutedEventArgs e)
        {
            DashboardViewModel viewModel = new DashboardViewModel();

            if (sender is ToggleSwitch toggleSwitch)
            {
                LightModeChange(toggleSwitch);
            }
        }

        private void UpdatePage()
        {
            if (ViewModel.CurrentTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark)
            {
                DarkModeChange(ThemeSwitch);
                ThemeSwitch.IsChecked = true;
            }
            else
            {
                LightModeChange(ThemeSwitch);
            }

            // Update Home Page Statistics
            UpdateStats();
        }

        // On page loaded, determine the current theme and check if Dark mode is already enabled.
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _alertManager = new AlertManager();
            _eventBus = new EventBus(_alertManager);
            _integrityHandlerModel = new IntegrityHandlerModel(_eventBus);
            UpdatePage();
        }

        private void UpdateStats()
        {
            // Get instance of SetupService
            ISetupService _setupService = SetupService.GetExistingInstance();

            // Quarantined Items Count
            ViewModel.QuarantinedItemsCount = $"{_databaseManager.GetAllQuarantinedFilesAsync().Result.Count()} files in quarantine";

            // Integrity Violations Count
            ViewModel.IntegrityViolationsCount = $"{_integrityHandlerModel.RecentViolationList.Count} integrity violations";

            // Whitelisted Items Count
            ViewModel.WhitelistedItemsCount = $"{_databaseManager.GetWhitelistAsync().Result.Count()} files whitelisted.";

            // Last Scan DateTime
            int lastScanDateTimeInt = _setupService.GetFromConfig("lastScanDateTime");
            long lastScanDateTimeUnix = (long)lastScanDateTimeInt;
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(lastScanDateTimeUnix).DateTime;
            // Check scan type
            if (_setupService.GetFromConfig("lastScanType") == 1)
            {
                ViewModel.LastScanDateTime = $"Last scan: {dateTime.ToString("dd/mm/yyyy hh:mm tt")} (quick scan)";
            }
            else if (_setupService.GetFromConfig("lastScanType") == 2)
            {
                ViewModel.LastScanDateTime = $"Last scan: {dateTime.ToString("dd/mm/yyyy hh:mm tt")} (full scan)";
            }
            else if (_setupService.GetFromConfig("lastScanType") == 3)
            {
                ViewModel.LastScanDateTime = $"Last scan: {dateTime.ToString("dd/mm/yyyy hh:mm tt")} (custom scan)";
            }
            else
            {
                ViewModel.LastScanDateTime = "Unable to retrieve details of last scan.";
            }

            // Last Scan Duration
            TimeSpan durationSeconds = TimeSpan.FromSeconds(_setupService.GetFromConfig("lastScanDuration"));
            ViewModel.LastScanDuration = $"Duration: {durationSeconds.ToString(@"hh\:mm\:ss")}";

            // Threats Last 24 hours
            int fileHashAlertsLast24h = _alertManager.GetAlertsByComponentWithinPastTimeFrame("File Hash Scanning", 86400).Result;
            int maliciousCodeAlertsLast24h = _alertManager.GetAlertsByComponentWithinPastTimeFrame("Malicious Code Scanning", 86400).Result;
            ViewModel.ThreatsLast24h = $"Threats in last 24 hours: {fileHashAlertsLast24h + maliciousCodeAlertsLast24h}";

            // Integrity Violations Last 24 hours
            ViewModel.IntegrityViolationsLast24h = $"Integrity Violations in last 24 hours: {_alertManager.GetAlertsByComponentWithinPastTimeFrame("Integrity Checking", 86400).Result}";

            // Terminal Alerts Last 24 hours
            ViewModel.TerminalAlertsLast24h = $"Times the registry has been accessed by Command Prompt or PowerShell in the last 24 hours: {_alertManager.GetAlertsByComponentWithinPastTimeFrame("Terminal Scanning", 86400).Result}";
        }
    }
}
