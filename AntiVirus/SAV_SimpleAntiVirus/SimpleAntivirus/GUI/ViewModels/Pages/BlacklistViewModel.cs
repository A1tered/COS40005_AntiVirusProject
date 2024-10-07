/**************************************************************************
* File:        BlacklistViewModel.cs
* Author:      Joel Parks
* Description: Handles interaction with database to blacklist file/hash.
* Last Modified: 8/10/2024
**************************************************************************/

using SimpleAntivirus.Alerts;
using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.FileQuarantine;
using System.IO;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class BlacklistViewModel : ObservableObject
    {
        private DirectoryManager _directoryManager;
        private DatabaseConnector _databaseConnector;
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;
        private IDatabaseManager _databaseManager;
        private Hasher _hasher;

        public BlacklistViewModel()
        {
            _directoryManager = new DirectoryManager();
            _databaseConnector = new DatabaseConnector(_directoryManager.getDatabaseDirectory("sighash.db"), true);
            _fileMover = new FileMover();
            _databaseManager = new DatabaseManager(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db"));
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManager, "C:\\ProgramData\\SimpleAntiVirus\\Quarantine");
            _hasher = new Hasher();
        }

        public async Task<bool> BlacklistFile(string path, AlertManager alertManager, EventBus eventBus)
        {
            string hash = _hasher.OpenHashFile(path);
            _databaseConnector.Open(_directoryManager.getDatabaseDirectory("sighash.db"), true);
            bool result = _databaseConnector.AddHash(hash);
            await _quarantineManager.QuarantineFileAsync(path, eventBus, "filehash");
            _databaseConnector.CleanUp();
            return result;
        }

        public bool BlacklistHash(string hash)
        {
            _databaseConnector.Open(_directoryManager.getDatabaseDirectory("sighash.db"), true);
            bool result = _databaseConnector.AddHash(hash);
            _databaseConnector.CleanUp();
            return result;
        }
    }
}
