using Microsoft.Data.Sqlite;
using SimpleAntivirus.AntiTampering;
using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.GUI.Views.Windows;
using SimpleAntivirus.MaliciousCodeScanning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;

namespace SimpleAntivirus.GUI.Services
{
    /// <summary>
    /// Mainly helpful for Anti-Tampering, set up of keys and what not, but will provide any setup routines that need to be done when
    /// running the program <para/>
    /// Has operations for first run, and run every time.
    /// <para/> Provides error messages if things are missing, ideal for when we decide to publish the software.
    /// </summary>
    public class SetupService
    {

        private bool _firstSetup;
        private INavigationWindow _iNaviWindow;
        private string _dbFolder;

        private static SetupService _setupService;
        private static readonly object _lock = new object(); 
        private SetupService(INavigationWindow naviWindow)
        {
            _firstSetup = false;
            _iNaviWindow = naviWindow;
            _dbFolder = Path.Combine(AppContext.BaseDirectory, "Databases");
        }

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        /// <param name="naviWindow"></param>
        /// <returns></returns>
        public static SetupService GetInstance(INavigationWindow naviWindow)
        {
            if (_setupService == null)
            {
                lock (_lock)
                {
                    if (_setupService == null)
                    {
                        _setupService = new SetupService(naviWindow);
                    }
                }
            }
            return _setupService;
        }

        /// <summary>
        /// Get SetupService without creating a new one.
        /// </summary>
        /// <returns></returns>
        public static SetupService GetExistingInstance()
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
            System.Windows.MessageBox.Show(problem, "Operation Failure", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            // Close program
            MainWindow window = _iNaviWindow as MainWindow;
            window.CloseWindowForcefully();
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



        public async Task<bool> Run()
        {
            System.Diagnostics.Debug.WriteLine("Startup Routine\n\n");
            string configPath = CreateFilePathProgramDataDirectory("config.enc");
            if (Path.Exists(configPath))
            {
                // Decrypt via set keys.

                // Decrypt config file which is expected to have line "firstRun=1"
                byte[] configInfo = EncryptionHandler.DecryptIntoMemory(configPath, EncryptionHandler.DecryptionKeyStorage(), EncryptionHandler.DecryptionIVStorage());

                string fileInfo = new(UTF8Encoding.UTF8.GetChars(configInfo));

                string[] lines = fileInfo.Split("\n");

                if (lines[1].Contains("firstRun=1"))
                {
                    _firstSetup = false;
                    GeneralRun();
                }
                else
                { // If we decrypt the config file and don't have this string, there is an issue.
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
            string[] CreateFolders =
            {
                            @"C:\ProgramData\SimpleAntiVirus\EncryptionKey",
                            @"C:\ProgramData\SimpleAntiVirus\DatabaseKey"
                        };

            foreach (string folders in CreateFolders)
            {
                if (Directory.Exists(folders))
                {
                    Directory.Delete(folders, true);
                }
            }

            foreach (string folders in CreateFolders)
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

            string configPath = CreateFilePathProgramDataDirectory("config");
            if (!Path.Exists(configPath))
            {
                // Setup config.txt file for basic info.
                using (StreamWriter fileS = new(configPath))
                {
                    await fileS.WriteAsync("Config_File_SAV");
                    await fileS.WriteAsync("\nfirstRun=1");
                }
                EncryptionHandler.GenerateEncryptionKey(out byte[] aesKey, out byte[] aesIV);
                EncryptionHandler.InitialEncryptFiles(configPath, aesKey, aesIV);
                EncryptionHandler.EncryptionKeyStorage(aesKey);
                EncryptionHandler.EncryptionIVStorage(aesIV);


                // Database Key Management
                EncryptionHandler.GenerateDBKey(out byte[] aesKeyDb, out byte[] aesIVDb);
                EncryptionHandler.EncryptionDBKeyStorage(aesKeyDb);
                EncryptionHandler.EncryptionDBIVStorage(aesIVDb);


                // Do advise, from the code... Integrity, Malicious_Cmd_DB, Quarantine are all capable of generating a db is one is not provided.
                // 



                // Malicious DB Setup
                DatabaseHandler databaseMalHandler = new(_dbFolder, true);

                DatabaseConnector databaseHashHandler = new(Path.Combine(_dbFolder, "sighash.db"), true, true);

                // Dispose (Messily)
                databaseMalHandler = null;

                databaseHashHandler = null;



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
            // On any runs after first run, occur and a database is found to be missing, then this is an issue and alert it.
            if (Path.Exists(CreateFilePathInProjectDirectory("Databases")))
            {
                if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\\quarantine.db")))
                {
                    ErrorMessage("Quarantine DB missing, reinstall program.");
                }
                if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\\malicious_commands.db")))
                {
                    ErrorMessage("Malicious CMD DB missing, reinstall program.");
                }
                if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\\integrity_database.db")))
                {
                    ErrorMessage("Integrity DB missing, reinstall program.");
                }
                if (!Path.Exists(CreateFilePathInProjectDirectory("Databases\\integrity_database.db")))
                {
                    ErrorMessage("Signature DB missing, reinstall program.");
                }
            }
            else
            {
                ErrorMessage("Databases folder does not exist!");
                return false;
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
            return "test";
        }

        public bool FirstTimeRunning
        {
            get
            {
                return _firstSetup;
            }
        }
    }
}
