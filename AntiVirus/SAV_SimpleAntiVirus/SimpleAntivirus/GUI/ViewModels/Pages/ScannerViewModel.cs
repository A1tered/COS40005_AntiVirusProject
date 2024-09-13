using System.ComponentModel;
using System.Diagnostics;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class ScannerViewModel : ObservableObject, INotifyPropertyChanged
    {
        private bool _isScanRunning;
        private bool _isAddFolderButtonVisible;
        private bool _isAddFileButtonVisible;
        
        public bool IsScanRunning
        {
            get => _isScanRunning;
            set => SetProperty(ref _isScanRunning, value);
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

        public bool IsAddFileButtonVisible
        {
            get => _isAddFileButtonVisible;
            set
            {
                _isAddFileButtonVisible = value;
                Debug.WriteLine($"invoke {value}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsAddFileButtonVisible"));
            }
        }

        public ScannerViewModel()
        {
            IsAddFolderButtonVisible = false;
            IsAddFileButtonVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}