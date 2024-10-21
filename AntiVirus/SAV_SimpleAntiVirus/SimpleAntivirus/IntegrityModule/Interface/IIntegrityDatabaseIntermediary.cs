/**************************************************************************
 * File:        IIntegrityDatabaseIntermediary.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Interacts with Parent (DatabaseIntermediary), to send function specific database commands.
 * Last Modified: 29/09/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;
using Microsoft.Data.Sqlite;

namespace SimpleAntivirus.IntegrityModule.Interface
{
    public interface IIntegrityDatabaseIntermediary : IDatabaseIntermediary
    {
        public event EventHandler<ProgressArgs> DataAddProgress;

        /// <summary>
        /// Creates the table, may do other setup tasks.
        /// </summary>
        /// <remarks>Only to be ran once at initialisation, any more and it will be at risk of easy tampering</remarks>
        public bool SetupDatabase();

        public Task CancelOperations();

        /// <summary>
        /// Deletes all entries from IntegrityTrack table
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll();

        /// <summary>
        /// Add integrity path, adds entry into IntegrityDatabase such as Hash info, file info, time of signature creation.
        /// </summary>
        /// <param name="path">Directory windows (file or directory)</param>
        /// <returns>False if nothing changed or most recent addition failed, True if no issues</returns>
        public Task<bool> AddEntry(string path, int amountPerSet);

        /// <summary>
        /// Remove items from database, whether that be directory and its contents, or a singular path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>(True) Items changed, (false) Items unchanged.</returns>
        public bool RemoveEntry(string path);

        /// <summary>
        /// Checks whether a directory exists in the integrity database.
        /// </summary>
        /// <param name="path">Windows directory</param>
        /// <returns>True/False depending on whether the entry exists within the database</returns>
        public bool CheckExistence(string path, SqliteTransaction trans = null);

        /// <summary>
        /// Get all info off a single row based on directory
        /// </summary>
        /// <param name="directory">Windows Directory</param>
        /// <returns>Directory, Hash, ModificationTime, SignatureCreation, OriginalSizeInBytes</returns>
        public Tuple<string, string, long, long, long> GetDirectoryInfo(string directory);


        /// <summary>
        /// Get a set of data from the database.
        /// </summary>
        /// <param name="set">Interval of set.</param>
        /// <param name="amountHandledPerSet">Amount per set.</param>
        /// <returns>Dictionary, hash pairs.</returns>
        public Dictionary<string, string> GetSetEntries(int set, int amountHandledPerSet);

        /// <summary>
        /// Get a set of data from the database that bear resemblance to the directory provided.
        /// </summary>
        /// <param name="directory">Finds entries that contain relevance to the directory provided</param>
        /// <example>C:/user5 would return all entries in that folder, and C:/user5/user2</example>
        /// <returns>Dictionary, hash pairs.</returns>
        public Dictionary<string, string> GetSetEntriesDirectory(string directory);

    }
}
