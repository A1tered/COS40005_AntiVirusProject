using DatabaseFoundations.IntegrityRelated;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFoundations
{
    public class IntegrityDatabaseIntermediary : DatabaseIntermediary
    {
        public IntegrityDatabaseIntermediary(string databaseName, bool firstRun, string key = "") : base(databaseName, firstRun, "IntegrityTrack", key)
        {
            // AntiTampering will need to ensure that this is only run at initialisation!!!
            if (firstRun)
            {
                SetupDatabase();
            }
        }

        /// <summary>
        /// Creates the table, may do other setup tasks.
        /// </summary>
        /// <remarks>Only to be ran once at initialisation, any more and it will be at risk of easy tampering</remarks>
        public bool SetupDatabase()
        {
            SqliteCommand command = new();
            command.CommandText = @$"CREATE TABLE IF NOT EXISTS
            {_defaultTable}(directory text PRIMARY KEY, hash text, modificationTime int, signatureCreation int, originalSize int)";
            return QueryNoReader(command) > 0;
        }

        /// <summary>
        /// Deletes all entries from IntegrityTrack table
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            SqliteCommand command = new();
            command.CommandText = $"DELETE FROM {_defaultTable}";
            return QueryNoReader(command) > 0;
        }

        private List<string> AsyncAdd(string[] givenPaths, int id, SqliteTransaction trans)
        {
            Console.WriteLine($"Started {id}");
            bool success = false;
            List<string> failureList = new();
            foreach (string cyclePath in givenPaths)
            {
                //Console.WriteLine($"ADD: {cyclePath}");
                SqliteCommand command = new();
                command.CommandText = @$"REPLACE INTO {_defaultTable} VALUES($path, $hash, $modTime, $sigCreation, $orgSize)";
                command.Transaction = trans;
                string getHash = FileInfoRequester.HashFile(cyclePath);
                Tuple<long, long> fileInfo = FileInfoRequester.RetrieveFileInfo(cyclePath);
                if (CheckExistence(cyclePath, trans))
                {
                    Console.WriteLine("Warning, replacing existing entry");
                }
                if (getHash != "")
                {
                    command.Parameters.AddWithValue("$path", cyclePath);
                    command.Parameters.AddWithValue("$hash", getHash);
                    command.Parameters.AddWithValue("$modTime", fileInfo.Item1);
                    command.Parameters.AddWithValue("$sigCreation", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
                    command.Parameters.AddWithValue("$orgSize", fileInfo.Item2);
                    success = QueryNoReader(command) > 0;
                    if (success != true)
                    {
                        failureList.Add(cyclePath);
                    }
                }
            }
            Console.WriteLine($"Completed add set - {id}");
            return failureList;
        }

        /// <summary>
        /// Add integrity path, adds entry into IntegrityDatabase such as Hash info, file info, time of signature creation.
        /// </summary>
        /// <param name="path">Directory windows (file or directory)</param>
        /// <returns>False if nothing changed or most recent addition failed, True if no issues</returns>
        public bool AddEntry(string path, int amountPerSet)
        {
            List<string> pathProcess =  FileInfoRequester.PathCollector(path);
            List<Task<List<string>>> taskManager = new();
            List<string> tempPathCreator = new();
            int idTracker = 0;
            int maxIds = pathProcess.Count() / amountPerSet;
            Console.WriteLine($"Sets required: {maxIds}");
            SqliteTransaction transactionCreate = _databaseConnection.BeginTransaction(); // remove if not good
            for (int v = 0; v < pathProcess.Count(); v++)
            {
                tempPathCreator.Add(pathProcess[v]);
                //Console.WriteLine($"AddEntry: {pathProcess[v]}");
                if (v % amountPerSet == 0 && v != 0)
                {
                    string[] pathArray = tempPathCreator.ToArray();
                    int tempInt = idTracker;
                    Console.WriteLine(idTracker);
                    taskManager.Add(Task.Run(() => AsyncAdd(pathArray, tempInt, transactionCreate)));
                    tempPathCreator.Clear();
                    idTracker++;
                }
            }
            // Deal with remainders
            taskManager.Add(Task.Run(() => AsyncAdd(tempPathCreator.ToArray(),idTracker, transactionCreate)));
            // We need a protection, if baseline fails to add...
            Task.WaitAll(taskManager.ToArray());
            transactionCreate.Commit();
            Console.WriteLine("Completed");
            return true;
        }

        public bool RemoveEntry(string path)
        {
            List<string> pathsToRemove = FileInfoRequester.PathCollector(path);
            bool success = false;
            int progress = 0;
            int maxProgress = pathsToRemove.Count();
            foreach (string pathCycle in pathsToRemove)
            {
                Console.Write($"\r Deleted {progress}/{maxProgress}");
                progress++;
                SqliteCommand command = new();
                command.CommandText = @$"DELETE FROM {_defaultTable} WHERE directory=$directorySet";
                command.Parameters.AddWithValue("$directorySet", pathCycle);
                success = QueryNoReader(command) > 0;
            }
            return success;
        }

        /// <summary>
        /// Checks whether a directory exists in the integrity database.
        /// </summary>
        /// <param name="path">Windows directory</param>
        /// <returns>True/False depending on whether the entry exists within the database</returns>
        public bool CheckExistence(string path, SqliteTransaction trans = null)
        {
            SqliteCommand command = new();
            command.CommandText = @$"SELECT directory FROM {_defaultTable} WHERE directory = $directorySet";
            command.Parameters.AddWithValue("$directorySet", path);
            command.Transaction = trans;
            SqliteDataReader reader = QueryReader(command);
            if (reader != null)
            {
                return reader.HasRows;
            }
            return false;
        }

        /// <summary>
        /// Get all info off a single row based on directory
        /// </summary>
        /// <param name="directory">Windows Directory</param>
        /// <returns>Directory, Hash, ModificationTime, SignatureCreation, OriginalSizeInBytes</returns>
        public Tuple<string, string, long, long, long> GetDirectoryInfo(string directory)
        {
            SqliteCommand command = new();
            command.CommandText = $"SELECT * FROM {_defaultTable} WHERE directory = $directorySet";
            command.Parameters.AddWithValue("$directorySet", directory);
            SqliteDataReader dataReader = QueryReader(command);
            if (dataReader.Read())
            {
                return new Tuple<string, string, long, long, long>(dataReader.GetString(0), dataReader.GetString(1), dataReader.GetInt64(2), dataReader.GetInt64(3), dataReader.GetInt64(4));
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, string> GetSetEntries(int set, int amountHandledPerSet)
        {
            SqliteCommand command = new();
            Dictionary<string, string> returnDictionary = new();
            command.CommandText = @$"SELECT * FROM {_defaultTable} LIMIT $limit OFFSET $offset";
            int setUp = set + 1;
            command.Parameters.AddWithValue("$limit", amountHandledPerSet);
            command.Parameters.AddWithValue("$offset", (setUp - 1) * amountHandledPerSet);
            SqliteDataReader dataReader = QueryReader(command);
            int amount = 0;
            if (dataReader != null)
            {
                while (dataReader.Read())
                {
                    // Directory -> Hash
                    returnDictionary[dataReader.GetString(0)] = dataReader.GetString(1);
                    amount++;
                }
            }
            return returnDictionary;
        }

    }
}
