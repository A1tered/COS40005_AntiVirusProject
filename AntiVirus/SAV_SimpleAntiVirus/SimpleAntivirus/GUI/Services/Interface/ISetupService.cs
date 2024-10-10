using Microsoft.Data.Sqlite;


namespace SimpleAntivirus.GUI.Services.Interface
{
    /// <summary>
    /// Mainly helpful for Anti-Tampering, set up of keys and what not, but will provide any setup routines that need to be done when
    /// running the program <para/>
    /// Has operations for first run, and run every time.
    /// <para/> Provides error messages if things are missing, ideal for when we decide to publish the software.
    /// </summary>
    public interface ISetupService
    {

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        /// <param name="naviWindow"></param>
        /// <returns></returns>
        public static abstract ISetupService GetInstance(IServiceProvider serviceSet, bool testingMode = false);

        /// <summary>
        /// Get SetupService without creating a new one.
        /// </summary>
        /// <returns></returns>
        public static abstract ISetupService GetExistingInstance();

        public void AddToConfig(string key, int value);


        /// <summary>
        /// Given key, get value from config file, if KEY cannot be found, -1 will be returned.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetFromConfig(string key);



        /// <summary>
        /// On closure, update config file. Find key, and update value.
        /// </summary>
        public Task UpdateConfig();



        public Task<bool> Run();

        /// <summary>
        /// We are assuming, on the first run that the current items will already exist: <para/>
        /// * Database Folder <para/>
        /// * Hash Database <para/>
        /// * Malicious Commands Database <para/>
        /// This function is to create <para/>
        /// Config File, Key for database, key for encryption
        /// </summary>
        /// <returns></returns>
        public Task<bool> FirstRun();

        /// <summary>
        /// Ran everytime the program is started up
        /// </summary>
        /// <returns>True - Program cannot detect any issues <para/> False - Program files are missing / tampering detected</returns>
        public bool GeneralRun();


        /// <summary>
        /// Load table from another database, into newly encrypted database.
        /// </summary>
        /// <param name="sqliteConnection"> Existing database connection</param>
        /// <param name="initialisationDatabaseFolder">Where databases are stored</param>
        /// <param name="fillerDatabaseName">What is the name of the database that is prefilled and not encrypted</param>
        /// <param name="table">Name of the table to transfer</param>
        public abstract static void TransferContents(SqliteConnection sqliteConnection, string initialisationDatabaseFolder, string fillerDatabaseName, string table);

        public string DbKey();

        public bool FirstTimeRunning{ get;}

        public bool ProgramCooked{ get; }

        public void Y2k38Problem();
    }
}
