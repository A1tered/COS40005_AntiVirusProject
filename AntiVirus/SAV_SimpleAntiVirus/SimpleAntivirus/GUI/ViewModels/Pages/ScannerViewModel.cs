/**************************************************************************
* File:        ScannerViewModel.cs
* Author:      JoelParks
* Description: Handles timing of scan, and the visibility of custom scan button.
* Last Modified: 8/10/2024
**************************************************************************/

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class ScannerViewModel : ObservableObject, INotifyPropertyChanged
    {
        private bool _isScanRunning;
        private bool _isAddFolderButtonVisible;
        private bool _isCustomListVisible;
        private string _customScanText;

        // Stopwatch
        private Stopwatch _stopwatch;
        private DispatcherTimer _timer;
        private string _stopwatchText;
        private string _stopwatchShortText;

        public ScannerViewModel()
        {
            IsAddFolderButtonVisible = false;
            _stopwatch = new Stopwatch();
            _timer = new DispatcherTimer
            {
                // Set tick interval to seconds
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = _stopwatch.Elapsed;
            StopwatchShortText = $"{elapsed:hh\\:mm\\:ss}";
            StopwatchText = $"Time elapsed: {elapsed:hh\\:mm\\:ss}";
        }

        public bool IsScanRunning
        {
            get => _isScanRunning;
            set
            {
                _isScanRunning = value;
                Debug.WriteLine($"invoke {value}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsScanRunning"));
            }
        }

        public bool IsAddFolderButtonVisible
        {
            get => _isAddFolderButtonVisible;
            set
            {
                _isAddFolderButtonVisible = value;
                Debug.WriteLine($"invoke {value}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsAddFolderButtonVisible"));
            }
        }

        public bool IsCustomListVisible
        {
            get => _isCustomListVisible;
            set
            {
                _isCustomListVisible = value;
                Debug.WriteLine($"invoke {value}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsCustomListVisible"));
            }
        }

        public string CustomScanText
        {
            get => _customScanText;
            set
            {
                _customScanText = value;
                Debug.WriteLine($"Current custom scan list: {_customScanText}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CustomScanText"));
            }
        }

        public string StopwatchShortText
        {
            get => _stopwatchShortText;
            set
            {
                _stopwatchShortText = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StopwatchShortText"));
            }
        }

        public string StopwatchText
        {
            get => _stopwatchText;
            set
            {
                _stopwatchText = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StopwatchText"));
            }
        }

        public void StartTimer()
        {
            _stopwatch.Start();
            _timer.Start();
        }

        public void StopTimer()
        {
            _stopwatch.Stop();
            _timer.Stop();
        }

        public void ResetTimer()
        {
            _stopwatch.Reset();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}