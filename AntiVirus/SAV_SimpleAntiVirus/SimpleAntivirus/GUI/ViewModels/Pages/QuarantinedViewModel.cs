using SimpleAntivirus.FileQuarantine;
using System.ComponentModel;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using SimpleAntivirus.GUI.Views.Pages;
using SimpleAntivirus.Alerts;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class QuarantinedViewModel : ObservableObject, INotifyPropertyChanged
    {
        private List<Entry> _pathSelected;
        private bool _allSelected;
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;
        private IDatabaseManager _databaseManager;
        public EventBus EventBus;
        public AlertManager AlertManager;

        public QuarantinedViewModel()
        {
            _allSelected = false;
            _fileMover = new FileMover();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quarantine"));
            AlertManager = new AlertManager;
            EventBus = new EventBus(AlertManager);
        }

        public List<Entry> PathSelected
        {
            get
            {
                return _pathSelected;
            }
            set
            {
                _pathSelected = value;
                OnPropertyChanged(nameof(PathSelected));
            }
        }

        public bool AllSelected
        {
            get
            {
                return _allSelected;
            }
            set
            {
                _allSelected = value;
            }
        }

        public async Task<int> Unquarantine()
        {
            // Mishap, has one item failed to be deleted?
            bool mishap = false;
            bool returnInfo = false;
            if (_pathSelected != null)
            {
                foreach (Entry entry in _pathSelected)
                {
                    returnInfo = await _quarantineManager.UnquarantineFileAsync(entry.Id);
                    if (!returnInfo)
                    {
                        mishap = true;
                    }
                }
                if (returnInfo)
                {
                    return mishap ? 3 : 2;
                }
                return 1;
            }
            else
            {
                // No items selected
                return 0;
            }
        }
    }
}
