using SimpleAntivirus.FileHashScanning;

namespace SimpleAntivirus.GUI.ViewModels.Pages
{
    public partial class BlacklistViewModel : ObservableObject
    {
        private DirectoryManager _directoryManager;
        private DatabaseConnector _databaseConnector;
        private Hasher _hasher;

        public BlacklistViewModel()
        {
            _directoryManager = new DirectoryManager();
            _databaseConnector = new DatabaseConnector(_directoryManager.getDatabaseDirectory("sighash.db"), true);
            _hasher = new Hasher();
        }

        public bool BlacklistFile(string path)
        {
            string hash = _hasher.OpenHashFile(path);
            _databaseConnector.Open(_directoryManager.getDatabaseDirectory("sighash.db"), true);
            bool result = _databaseConnector.AddHash(hash);
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
