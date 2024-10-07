/**************************************************************************
* File:        WhitelistViewModel.cs
* Author:      Joel Parks
* Description: Handles removal of whitelist items from database.
* Last Modified: 8/10/2024
**************************************************************************/

using SimpleAntivirus.FileQuarantine;
using System.ComponentModel;
using System.IO;
using SimpleAntivirus.GUI.Views.Pages;
using System.Diagnostics;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class WhitelistViewModel : ObservableObject, INotifyPropertyChanged
    {
        private List<WhitelistEntry> _pathSelected;
        private bool _allSelected;
        private bool _isBusy;
        private IDatabaseManager _databaseManager;

        public WhitelistViewModel()
        {
            _allSelected = false;
            _isBusy = false;
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
        }

        public List<WhitelistEntry> PathSelected
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

        public async Task<int> Unwhitelist()
        {
            // Mishap, has one item failed to be unquarantined>
            bool mishap = false;
            bool returnInfo = false;
            if (_pathSelected != null)
            {
                foreach (WhitelistEntry entry in _pathSelected)
                {
                    returnInfo = await _databaseManager.RemoveFromWhitelistAsync(entry.FilePath);
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
