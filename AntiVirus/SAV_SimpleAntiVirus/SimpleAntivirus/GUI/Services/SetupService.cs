using iTextSharp.text.pdf;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using SimpleAntivirus.AntiTampering;
using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.FileQuarantine;
using SimpleAntivirus.GUI.Services.Interface;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.Views.Windows;
using SimpleAntivirus.MaliciousCodeScanning;
using System.Data.Common;
using System.IO;
using System.Text;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace SimpleAntivirus.GUI.Services
{
    /// <summary>
    /// Mainly helpful for Anti-Tampering, set up of keys and what not, but will provide any setup routines that need to be done when
    /// running the program <para/>
    /// Has operations for first run, and run every time.
    /// <para/> Provides error messages if things are missing, ideal for when we decide to publish the software.
    /// </summary>
    public class SetupService : ISetupService
    {

        private bool _firstSetup;
        private INavigationWindow _iNaviWindow;
        private string _dbFolder;
        private string[] _dbNames;
        private string _configPath;
        private bool _testingMode;
        Dictionary<string, int> _configDictionary;
        private IServiceProvider _serviceSet;

        // db folders
        string[] _createFolders;

        // Indicating that decryption stage failed later on, ensure to cancel any saving of config.
        bool _programCooked;

        private static SetupService _setupService;
        private static readonly object _lock = new object(); 

        private SetupService(IServiceProvider serviceSet, bool testingMode = false)
        {
            _testingMode = testingMode;
            _programCooked = false;
            _firstSetup = false;

            if (!_testingMode)
            {
                _iNaviWindow = serviceSet.GetService<INavigationWindow>();
            }
            else // If testing, assume booting first time.
            {
                _firstSetup = true;
            }
            _dbFolder = Path.Combine(AppContext.BaseDirectory, "Databases");
            _configPath = CreateFilePathProgramDataDirectory("config.enc");
            _serviceSet = serviceSet;
            // Config Dictionary (eg, firstRun=1)
            _configDictionary = new();

            _dbNames = new string[] { "sighash.db", "alerts.db", "malicious_commands.db", "quarantine.db", "integrity_database.db" };

            // db folers
            _createFolders = new string[]{@"C:\ProgramData\SimpleAntiVirus\EncryptionKey",
                            @"C:\ProgramData\SimpleAntiVirus\DatabaseKey"};
        }

        // Sets setupservice into testing mode, it sheds functionality and ensures DBKey returns a empty key.
        public void TestingMode()
        {
            _testingMode = true;
        }

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        /// <param name="naviWindow"></param>
        /// <returns></returns>
        public static ISetupService GetInstance(IServiceProvider serviceSet, bool testingMode = false)
        {
            if (_setupService == null)
            {
                lock (_lock)
                {
                    if (_setupService == null)
                    {
                        _setupService = new SetupService(serviceSet, testingMode);
                    }
                }
            }
            return _setupService;
        }

        /// <summary>
        /// Get SetupService without creating a new one.
        /// </summary>
        /// <returns></returns>
        public static ISetupService GetExistingInstance()
        {
            if (_setupService != null)
            {
                return _setupService;
            }
            throw new Exception("GetExistingInstance() in SetupService called, despite not GetInstance() not called beforehand.");
        }

        /// <summary>
        /// Display error message and then close program.
        /// </summary>
        /// <param name="problem"></param>
        private void ErrorMessage(string problem)
        {
            _programCooked = true;
            if (_testingMode == false)
            {
                System.Windows.MessageBox.Show($"Operation Failure: {problem}", "Simple Antivirus", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            // Close program

                MainWindow window = _iNaviWindow as MainWindow;
                window.CloseWindowGracefully();
            }
        }

        // Create a path that represents a new dir or file
        private string CreateFilePathInProjectDirectory(string nameOrDir)
        {
            return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameOrDir));
        }

        // Create a path that represents a new dir or file
        private string CreateFilePathProgramDataDirectory(string nameOrDir)
        {
            return (Path.Combine("C:\\ProgramData\\SimpleAntiVirus", nameOrDir));
        }


        /// <summary>
        /// Given a key, save a integer value into config file.
        /// </summary>
        /// <param name="key">Key eg "darkMode"</param>
        /// <param name="value">value eg 1</param>
        public void AddToConfig(string key, int value)
        {
            _configDictionary[key] = value;
        }


        /// <summary>
        /// Given key, get value from config file, if KEY cannot be found, -1 will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetFromConfig(string key)
        {
            if (_configDictionary.ContainsKey(key))
            {
                return _configDictionary[key];
            }
            else
            {
                return -1;
            }
        }

        private string ReadConfig()
        {
            byte[] configInfo = EncryptionHandler.DecryptIntoMemory(_configPath, EncryptionHandler.DecryptionKeyStorage(), EncryptionHandler.DecryptionIVStorage());

            return new string(UTF8Encoding.UTF8.GetChars(configInfo));
        }

        /// <summary>
        /// On closure, update config file. Find key, and update value.
        /// </summary>
        public async Task UpdateConfig()
        {
            if (_programCooked == false)
            {
                int valueRepresent;
                string contents = ReadConfig();

                valueRepresent = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark ? 1 : 0;
                _configDictionary["darkMode"] = valueRepresent;
                // Setup config.txt file for basic info.
                using (StreamWriter fileS = new(_configPath, false))
                {
                    await fileS.WriteLineAsync("Config_File_SAV");
                    foreach (KeyValuePair<string, int> valuePair in _configDictionary)
                    {
                        await fileS.WriteLineAsync($"{valuePair.Key}={valuePair.Value}");
                    }
                }
                EncryptionHandler.InitialEncryptFiles(_configPath, EncryptionHandler.DecryptionKeyStorage(), EncryptionHandler.DecryptionIVStorage());
            }
        }



        public async Task<bool> Run()
        {
            System.Diagnostics.Debug.WriteLine("Startup Routine\n\n");
            string configPath = _configPath;
            if (Path.Exists(configPath))
            {
                // Decrypt via set keys.

                // Decrypt config file which is expected to have line "firstRun=1"
                try
                {
                    string fileInfo = ReadConfig();

                    System.Diagnostics.Debug.WriteLine($"Read Config Info:\n {fileInfo}");
                    System.Diagnostics.Debug.WriteLine("\n");
                    string[] lines = fileInfo.Split("\n");
                    foreach (string line in lines)
                    {
                        string[] commandValue = line.Split("=");
                        if (commandValue.Count() == 2)
                        {
                            _configDictionary[commandValue[0]] = Convert.ToInt32(commandValue[1]);
                        }
                    }


                    if (_configDictionary.ContainsKey("firstRun"))
                    {
                        if (_configDictionary["firstRun"] == 1)
                        {
                            _firstSetup = false;
                            GeneralRun();
                        }
                        return true;
                    }
                }
                catch (KeyNotFoundException e)
                {
                    // If we decrypt the config file and don't have this string, there is an issue.
                    ErrorMessage("Configuration file has been tampered with and cannot be read, reinstall program.");
                }
            }
            else
            {
                await FirstRun();
            }
            return true;
        }


        /// <summary>
        /// We are assuming, on the first run that the current items will already exist: <para/>
        /// * Database Folder <para/>
        /// * Hash Database <para/>
        /// * Malicious Commands Database <para/>
        /// This function is to create <para/>
        /// Config File, Key for database, key for encryption
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FirstRun()
        {
            _firstSetup = true;

            foreach (string folders in _createFolders)
            {
                if (Directory.Exists(folders))
                {
                    Directory.Delete(folders, true);
                }
            }

            foreach (string folders in _createFolders)
            {
                try
                {
                    Directory.CreateDirectory(folders);
                    Console.WriteLine($"Created Encryption storage: {folders}");
                }
                catch (Exception errormsg)
                {
                    Console.WriteLine($"Unexpected error creating: {folders}. \nDescription: {errormsg.Message}");
                }
            }
            if (!Path.Exists(_configPath))
            {
                _configDictionary["firstRun"] = 1;
                // Setup config.txt file for basic info.
                using (StreamWriter fileS = new(_configPath))
                {
                    await fileS.WriteAsync("Config_File_SAV");
                    await fileS.WriteAsync("\nfirstRun=1");
                }
                EncryptionHandler.GenerateEncryptionKey(out byte[] aesKey, out byte[] aesIV);
                EncryptionHandler.InitialEncryptFiles(_configPath, aesKey, aesIV);
                EncryptionHandler.EncryptionKeyStorage(aesKey);
                EncryptionHandler.EncryptionIVStorage(aesIV);


                // Database Key Management
                EncryptionHandler.GenerateDBKey(out byte[] aesKeyDb, out byte[] aesIVDb);
                EncryptionHandler.EncryptionDBKeyStorage(aesKeyDb);
                EncryptionHandler.EncryptionDBIVStorage(aesIVDb);


                // Do advise, from the code... Integrity, Malicious_Cmd_DB, Quarantine are all capable of generating a db is one is not provided.
                // 

                

                // Setup Database folder
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // If database folder does not exist, create it.
                if (!Directory.Exists(_dbFolder))
                {

                    Directory.CreateDirectory(_dbFolder);
                }
                else
                {
                    foreach (string databaseName in _dbNames)
                    {
                        if (Path.Exists(CreateFilePathInProjectDirectory($"Databases\\{databaseName}")))
                        {
                            ErrorMessage($"Database setup failed, unexpected pre-existing databases found");
                        }
                    }
                }

                // Malicious DB Setup
                DatabaseHandler databaseMalHandler = new(_dbFolder, true);

                DatabaseConnector databaseHashHandler = new(Path.Combine(_dbFolder, "sighash.db"), true, true);

                // DB setup for DBs that dont automatically set up.

                DatabaseManager databaseManager = new(Path.Combine(_dbFolder, "quarantine.db"));

                // Dispose (Messily)
                databaseMalHandler = null;

                databaseHashHandler = null;

                databaseManager = null;



                // The only issue will be is the transfer of data for signature hash, and malicious cmd db.
                // Hopefully on setup, we can provide all database files for simplicity, rather than any form of generation...

            }


            System.Windows.MessageBox.Show("Program has been booted up for the first time\n This message will only show up if no data is found!", "First time booting up", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        /// <summary>
        /// Ran everytime the program is started up
        /// </summary>
        /// <returns>True - Program cannot detect any issues <para/> False - Program files are missing / tampering detected</returns>
        public bool GeneralRun()
        {
            // Read config file to find preferred theme
            if (_configDictionary.ContainsKey("darkMode")) {
                if (_configDictionary["darkMode"] == 0)
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                }
                else
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                }
                if (!_testingMode)
                {
                    _serviceSet.GetService<DashboardViewModel>().CurrentTheme = ApplicationThemeManager.GetAppTheme();
                }
            }
            // Ensure existence of database key folders
            foreach (string folderSet in _createFolders)
            {
                if (!Directory.Exists(folderSet))
                {
                    ErrorMessage("Incomplete ProgramData Setup! Reinstall Program!");
                }
            }

            // On any runs after first run, occur and a database is found to be missing, then this is an issue and alert it.
            if (Path.Exists(CreateFilePathInProjectDirectory("Databases")))
            {
                foreach (string databaseName in _dbNames)
                {
                    if (!Path.Exists(CreateFilePathInProjectDirectory($"Databases\\{databaseName}")))
                    {
                        ErrorMessage($"{databaseName} DB missing, reinstall program");
                    }
                }
            }
            else
            {
                ErrorMessage("Databases folder does not exist!");
                return false;
            }


            // Now lets test the database key to ensure its correct. (We'll test on the malicious database handle here)
            try
            {
                DatabaseHandler databaseMalHandler = new(_dbFolder, true);
                databaseMalHandler = null;
            }
            catch (Microsoft.Data.Sqlite.SqliteException e)
            {
                if (e.Message.Contains("file is not a database"))
                {
                    ErrorMessage("Keys in ProgramData are not the same keys that were utilised to encrypt the database. Reinstall Program.");
                    return false;
                }

                ErrorMessage("Database failed to open.");
            }


            //if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\IntegrityDatabase.db"));
            return true;
        }


        /// <summary>
        /// Load table from another database, into newly encrypted database.
        /// </summary>
        /// <param name="sqliteConnection"> Existing database connection</param>
        /// <param name="initialisationDatabaseFolder">Where databases are stored</param>
        /// <param name="fillerDatabaseName">What is the name of the database that is prefilled and not encrypted</param>
        /// <param name="table">Name of the table to transfer</param>
        public static void TransferContents(SqliteConnection sqliteConnection, string initialisationDatabaseFolder, string fillerDatabaseName, string table)
        {
            string fillerDatabase = Path.Combine(initialisationDatabaseFolder, $"initialisation_databases\\{fillerDatabaseName}");
            
            new SqliteCommand($"ATTACH DATABASE '{fillerDatabase}' as 'fillerDatabase' KEY ''", sqliteConnection).ExecuteNonQuery();
            new SqliteCommand($"INSERT OR IGNORE INTO {table} SELECT * FROM fillerDatabase.{table}", sqliteConnection).ExecuteNonQuery();
            new SqliteCommand($"DETACH fillerDatabase", sqliteConnection).ExecuteNonQuery();
        }

        public string DbKey()
        {
            // If testing just return empty string.
            if (_testingMode)
            {
                return "";
            }
            try
            {
                byte[] byteSet = EncryptionHandler.DecryptionDBKeyStorage();
                return BitConverter.ToString(byteSet);
            }
            catch (System.Security.Cryptography.CryptographicException e)
            {
                ErrorMessage("Key for database has been corrupted or mismatch, reinstall program.");
                return "failure";
            }
        }

        public static void GetInstance()
        {
            throw new NotImplementedException();
        }

        public bool FirstTimeRunning
        {
            get
            {
                return _firstSetup;
            }
        }

        public bool ProgramCooked
        {
            get
            {
                return _programCooked;
            }
        }

        public static ISetupService Instance { get; set; }
    }
}
