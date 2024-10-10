<<<<<<< Updated upstream
﻿using Wpf.Ui.Appearance;
=======
﻿/**************************************************************************
* File:        DashboardViewModel.cs
* Author:      WPF template
* Description: Handles the specific details of an alert.
* Last Modified: 8/10/2024
**************************************************************************/

using System.ComponentModel;
using Wpf.Ui.Appearance;
>>>>>>> Stashed changes
using Wpf.Ui.Controls;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        
        // Protection status indicators
        private bool _atRiskLight;
        private bool _atRiskDark;
        private bool _potentialRiskLight;
        private bool _potentialRiskDark;
        private bool _protectedLight;
        private bool _protectedDark;

        // Stat strings
        private string _quarantinedItemsCount;
        private string _integrityViolationsCount;
        private string _whitelistedItemsCount;
        private string _lastScanDateTime;
        private string _lastScanDuration;
        private string _threatsLast24h;
        private string _integrityViolationsLast24h;
        private string _terminalAlertsLast24h;


        // Properties for protection status indicators
        public bool AtRiskLight
        {
            get => _atRiskLight;
            set
            {
                _atRiskLight = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AtRiskLight"));
            }
        }

        public bool AtRiskDark
        {
            get => _atRiskDark;
            set
            {
                _atRiskDark = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AtRiskDark"));
            }
        }

        public bool PotentialRiskLight
        {
            get => _potentialRiskLight;
            set
            {
                _potentialRiskLight = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PotentialRiskLight"));
            }
        }

        public bool PotentialRiskDark
        {
            get => _potentialRiskDark;
            set
            {
                _potentialRiskDark = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PotentialRiskDark"));
            }
        }

        public bool ProtectedLight
        {
            get => _protectedLight;
            set
            {
                _protectedLight = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProtectedLight"));
            }
        }

        public bool ProtectedDark
        {
            get => _protectedDark;
            set
            {
                _protectedDark = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ProtectedDark"));
            }
        }

        // Properties for stat strings
        public string QuarantinedItemsCount
        {
            get => _quarantinedItemsCount;
            set
            {
                _quarantinedItemsCount = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("QuarantinedItemsCount"));
            }
        }

        public string IntegrityViolationsCount
        {
            get => _integrityViolationsCount;
            set
            {
                _integrityViolationsCount = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IntegrityViolationsCount"));
            }
        }

        public string WhitelistedItemsCount
        {
            get => _whitelistedItemsCount;
            set
            {
                _whitelistedItemsCount = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("WhitelistedItemsCount"));
            }
        }

        public string LastScanDateTime
        {
            get => _lastScanDateTime;
            set
            {
                _lastScanDateTime = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LastScanDateTime"));
            }
        }

        public string LastScanDuration
        {
            get => _lastScanDuration;
            set
            {
                _lastScanDuration = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LastScanDuration"));
            }
        }

        public string ThreatsLast24h
        {
            get => _threatsLast24h;
            set
            {
                _threatsLast24h = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ThreatsLast24h"));
            }
        }

        public string IntegrityViolationsLast24h
        {
            get => _integrityViolationsLast24h;
            set
            {
                _integrityViolationsLast24h = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IntegrityViolationsLast24h"));
            }
        }

        public string TerminalAlertsLast24h
        {
            get => _terminalAlertsLast24h;
            set
            {
                _terminalAlertsLast24h = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TerminalAlertsLast24h"));
            }
        }

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;


        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            _isInitialized = true;
        }

        [RelayCommand]
        public void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
