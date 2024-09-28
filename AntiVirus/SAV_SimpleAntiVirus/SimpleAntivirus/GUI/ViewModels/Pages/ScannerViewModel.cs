using System.ComponentModel;
using System.Diagnostics;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class ScannerViewModel : ObservableObject, INotifyPropertyChanged
    {
        private bool _isScanRunning;
        private bool _isAddFolderButtonVisible;
        
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

        public ScannerViewModel()
        {
            IsAddFolderButtonVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}