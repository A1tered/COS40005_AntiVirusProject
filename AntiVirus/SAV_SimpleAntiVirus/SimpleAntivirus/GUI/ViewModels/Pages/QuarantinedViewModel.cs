/**************************************************************************
 * File:        QuarantinedViewModel.cs
 * Author:      Joel Parks
 * Description: Handles backend functionality of Quarantine page.
 * Last Modified: 21/10/2024
 **************************************************************************/

using SimpleAntivirus.FileQuarantine;
using System.ComponentModel;
using System.IO;
using SimpleAntivirus.GUI.Views.Pages;
using SimpleAntivirus.Alerts;
using System.Diagnostics;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class QuarantinedViewModel : ObservableObject, INotifyPropertyChanged
    {
        private List<Entry> _pathSelected;
        private bool _allSelected;
        private bool _isBusy;
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;
        private IDatabaseManager _databaseManager;
        public EventBus EventBus;
        public AlertManager AlertManager;

        public QuarantinedViewModel()
        {
            _allSelected = false;
            _isBusy = false;
            _fileMover = new FileMover();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, "C:\\ProgramData\\SimpleAntiVirus\\Quarantine");
            AlertManager = new AlertManager();
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
            // Mishap, has one item failed to be unquarantined>
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

        public async Task<int> Whitelist()
        {
            // Mishap, has one item failed to be whitelisted?
            bool mishap = false;
            bool returnInfo = false;
            if (_pathSelected != null)
            {
                foreach (Entry entry in _pathSelected)
                {
                    returnInfo = await _databaseManager.AddToWhitelistAsync(entry.OriginalFilePath);
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

        public async Task<int> Delete()
        {
            // Mishap, has one item failed to be whitelisted?
            bool mishap = false;
            bool returnInfo = false;
            if (_pathSelected != null)
            {
                foreach (Entry entry in _pathSelected)
                {
                    await _databaseManager.RemoveQuarantineEntryAsync(entry.Id);
                    returnInfo = await _quarantineManager.DeleteFileAsync(entry.QuarantinedFilePath);
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

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                Debug.WriteLine($"invoke {value}");
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
