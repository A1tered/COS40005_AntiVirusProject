/**************************************************************************
 * File:        IntegrityDatabaseIntermediary.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interacts with Parent (DatabaseIntermediary), to send function specific database commands.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.IntegrityModule.DataTypes;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Diagnostics;
using SimpleAntivirus.IntegrityModule.Interface;

namespace TestingIntegrity.DummyClasses
{
    public class IntegrityDatabaseIntermediaryDummy : DatabaseIntermediaryDummy, IIntegrityDatabaseIntermediary
    {
        public IntegrityDatabaseIntermediaryDummy(string databaseName, bool firstRun) : base(databaseName, firstRun, "IntegrityTrack")
        {

        }

        public event EventHandler<ProgressArgs> DataAddProgress;

        /// <summary>
        /// Creates the table, may do other setup tasks.
        /// </summary>
        /// <remarks>Only to be ran once at initialisation, any more and it will be at risk of easy tampering</remarks>
        public bool SetupDatabase()
        {
            return true;
        }

        public async Task CancelOperations()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes all entries from IntegrityTrack table
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            return true;
        }

        /// <summary>
        /// Add integrity path, adds entry into IntegrityDatabase such as Hash info, file info, time of signature creation.
        /// </summary>
        /// <param name="path">Directory windows (file or directory)</param>
        /// <returns>False if nothing changed or most recent addition failed, True if no issues</returns>
        public async Task<bool> AddEntry(string path, int amountPerSet)
        {
            return true;
        }

        /// <summary>
        /// Remove items from database, whether that be directory and its contents, or a singular path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>(True) Items changed, (false) Items unchanged.</returns>
        public bool RemoveEntry(string path)
        {
            return true;
        }

        /// <summary>
        /// Checks whether a directory exists in the integrity database.
        /// </summary>
        /// <param name="path">Windows directory</param>
        /// <returns>True/False depending on whether the entry exists within the database</returns>
        public bool CheckExistence(string path, SqliteTransaction trans = null)
        {
            return false;
        }

        /// <summary>
        /// Get all info off a single row based on directory
        /// </summary>
        /// <param name="directory">Windows Directory</param>
        /// <returns>Directory, Hash, ModificationTime, SignatureCreation, OriginalSizeInBytes</returns>
        public Tuple<string, string, long, long, long> GetDirectoryInfo(string directory)
        {
            return new Tuple<string, string, long, long, long>(directory, "holder", 0, 0, 0);
        }


        /// <summary>
        /// Get a set of data from the database.
        /// </summary>
        /// <param name="set">Interval of set.</param>
        /// <param name="amountHandledPerSet">Amount per set.</param>
        /// <returns>Dictionary, hash pairs.</returns>
        public Dictionary<string, string> GetSetEntries(int set, int amountHandledPerSet)
        {
            Dictionary<string, string> returnDictionary = new();
            returnDictionary[@"C:\Users\yumcy\OneDrive\Desktop\Github Repositories\Technology Project A\COS40005_AntiVirusProject\AntiVirus\Testing\TestingIntegrity\testingFolder\testitem1.txt"] = "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709";
            returnDictionary[@"C:\Users\yumcy\OneDrive\Desktop\Github Repositories\Technology Project A\COS40005_AntiVirusProject\AntiVirus\Testing\TestingIntegrity\testingFolder\testitem2.txt"] = "an_incorrect_hash";
            return returnDictionary;
        }

        /// <summary>
        /// Get a set of data from the database that bear resemblance to the directory provided.
        /// </summary>
        /// <param name="directory">Finds entries that contain relevance to the directory provided</param>
        /// <example>C:/user5 would return all entries in that folder, and C:/user5/user2</example>
        /// <returns>Dictionary, hash pairs.</returns>
        public Dictionary<string, string> GetSetEntriesDirectory(string directory)
        {
            Dictionary<string, string> returnDictionary = new();
            returnDictionary[@"C:\Users\yumcy\OneDrive\Desktop\Github Repositories\Technology Project A\COS40005_AntiVirusProject\AntiVirus\Testing\TestingIntegrity\testingFolder\testitem1.txt"] = "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709";
            returnDictionary[@"C:\Users\yumcy\OneDrive\Desktop\Github Repositories\Technology Project A\COS40005_AntiVirusProject\AntiVirus\Testing\TestingIntegrity\testingFolder\testitem2.txt"] = "an_incorrect_hash";
            return returnDictionary;
        }

    }
}
