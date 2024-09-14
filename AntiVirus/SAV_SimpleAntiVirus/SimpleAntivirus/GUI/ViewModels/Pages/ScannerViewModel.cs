namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class ScannerViewModel : ObservableObject
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
            set => SetProperty(ref _isAddFolderButtonVisible, value);
        }

        public bool IsAddFileButtonVisible
        {
            get => _isAddFileButtonVisible;
            set => SetProperty(ref _isAddFolderButtonVisible, value);
        }

        public ScannerViewModel()
        {
            IsAddFolderButtonVisible = false;
            IsAddFileButtonVisible = false;
        }
    }
}