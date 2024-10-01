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

namespace SimpleAntivirus.IntegrityModule.Db
{
    public class IntegrityDatabaseIntermediary : DatabaseIntermediary
    {
        public IntegrityDatabaseIntermediary(string databaseName, bool firstRun) : base(databaseName, firstRun, "IntegrityTrack")
        {
            // AntiTampering will need to ensure that this is only run at initialisation!!!
            if (firstRun)
            {
                SetupDatabase();
            }
        }

        public event EventHandler<ProgressArgs> DataAddProgress;

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

        /// <summary>
        /// Adds all the paths to the database.
        /// </summary>
        /// <param name="givenPaths">Array of string paths.</param>
        /// <param name="id">id provided by AddEntry function.</param>
        /// <param name="trans">SQLiteTransaction reference.</param>
        /// <returns>Path failed to be added.</returns>
        private async Task<bool> AsyncAdd(string[] givenPaths, int id, SqliteTransaction trans, CancellationToken cancelToken)
        {
            System.Diagnostics.Debug.WriteLine($"Started {id}");
            bool noFailure = true;
            string failurePath = "";
            // Get hashes beforehand
            List<Task<string>> taskHandlerHash = new();
            // Get all hashes first.
            List<string> hashSet = await FileInfoRequester.HashSet(givenPaths.ToList());
            if (hashSet.Contains(""))
            {
                // An item failed to be hashed, so its a failure, return false.
                return false;
            }
            int index = 0;
            foreach (string cyclePath in givenPaths)
            {
                cancelToken.ThrowIfCancellationRequested();
                //Debug.WriteLine($"ADD: {cyclePath}");
                SqliteCommand command = new();
                command.CommandText = @$"REPLACE INTO {_defaultTable} VALUES($path, $hash, $modTime, $sigCreation, $orgSize)";
                command.Transaction = trans;
                string getHash = hashSet[index];
                Tuple<long, long> fileInfo = FileInfoRequester.RetrieveFileInfo(cyclePath);
                if (CheckExistence(cyclePath, trans))
                {
                    System.Diagnostics.Debug.WriteLine("Warning, replacing existing entry");
                }
                command.Parameters.AddWithValue("$path", cyclePath);
                command.Parameters.AddWithValue("$hash", getHash);
                command.Parameters.AddWithValue("$modTime", fileInfo.Item1);
                command.Parameters.AddWithValue("$sigCreation", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
                command.Parameters.AddWithValue("$orgSize", fileInfo.Item2);
                if (QueryNoReader(command) <= 0)
                {
                    // Failure detected
                    noFailure = false;
                    failurePath = cyclePath;
                    break;
                }
                index++;
            }
            if (noFailure == false)
            {
                System.Diagnostics.Debug.WriteLine($"Failure to add {failurePath}, changes will be reverted.");
                return false;
            }
            System.Diagnostics.Debug.WriteLine($"Completed add set - {id}");
            return true;
        }

        /// <summary>
        /// Add integrity path, adds entry into IntegrityDatabase such as Hash info, file info, time of signature creation.
        /// </summary>
        /// <param name="path">Directory windows (file or directory)</param>
        /// <returns>False if nothing changed or most recent addition failed, True if no issues</returns>
        public async Task<bool> AddEntry(string path, int amountPerSet)
        {
            List<string> pathProcess =  FileInfoRequester.PathCollector(path);
            List<Task<bool>> taskManager = new();
            List<string> tempPathCreator = new();
            // percent calc
            int completedTasks = 0;

            int idTracker = 0;
            int maxIds = pathProcess.Count() / amountPerSet;
            // Cancellation control variables
            bool forceFailure = false;
            CancellationTokenSource cancelToken = new();
            System.Diagnostics.Debug.WriteLine($"Sets required: {maxIds}");
            using (SqliteTransaction transactionCreate = _databaseConnection.BeginTransaction())
            { // remove if not good
                for (int v = 0; v < pathProcess.Count(); v++)
                {
                    tempPathCreator.Add(pathProcess[v]);
                    //Debug.WriteLine($"AddEntry: {pathProcess[v]}");
                    if (v % amountPerSet == 0 && v != 0)
                    {
                        string[] pathArray = tempPathCreator.ToArray();
                        int tempInt = idTracker;
                        System.Diagnostics.Debug.WriteLine(idTracker);
                        taskManager.Add(Task.Run(() => AsyncAdd(pathArray, tempInt, transactionCreate, cancelToken.Token)));
                        tempPathCreator.Clear();
                        idTracker++;
                    }
                }
                // Deal with remainders
                taskManager.Add(Task.Run(() => AsyncAdd(tempPathCreator.ToArray(), idTracker, transactionCreate, cancelToken.Token)));
                // We need a protection, if baseline fails to add...
                while (taskManager.Count() > 0)
                {
                    try
                    {
                        await Task.WhenAny(taskManager.ToArray());
                        
                    }
                    catch (OperationCanceledException e)
                    {
                        System.Diagnostics.Debug.WriteLine("operation cancelled");
                    }
                    foreach (Task<bool> taskItem in taskManager)
                    {
                        // Add each failed path to list.
                        if (taskItem.IsCompleted)
                        {
                            completedTasks++;
                            if (taskItem.IsCanceled)
                            {
                                break;
                            }
                            if (await taskItem == false)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                System.Diagnostics.Debug.WriteLine("Rollbacking Database due to adding directory error");
                                Console.ResetColor();
                                forceFailure = true;
                                cancelToken.Cancel();
                                break;
                            }
                        }
                    }
                    ProgressArgs argCreate = new();
                    argCreate.Progress = ((float)completedTasks / maxIds) * 100;
                    DataAddProgress.Invoke(this, argCreate);
                    // Remove all completed tasks;
                    taskManager.RemoveAll(x => x.IsCompleted);
                }
                if (forceFailure)
                {
                    taskManager.Clear();
                    transactionCreate.Rollback();
                    cancelToken.Dispose();
                    return false;
                }
                transactionCreate.Commit();
                System.Diagnostics.Debug.WriteLine("Completed");
                return true;
            }
        }

        /// <summary>
        /// Remove items from database, whether that be directory and its contents, or a singular path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>(True) Items changed, (false) Items unchanged.</returns>
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
                // Directory, Hash, ModificationTime, SignatureCreation, OriginalSize
                return new Tuple<string, string, long, long, long>(dataReader.GetString(0), dataReader.GetString(1), dataReader.GetInt64(2), dataReader.GetInt64(3), dataReader.GetInt64(4));
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Get a set of data from the database.
        /// </summary>
        /// <param name="set">Interval of set.</param>
        /// <param name="amountHandledPerSet">Amount per set.</param>
        /// <returns>Dictionary, hash pairs.</returns>
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

        /// <summary>
        /// Get a set of data from the database that bear resemblance to the directory provided.
        /// </summary>
        /// <param name="directory">Finds entries that contain relevance to the directory provided</param>
        /// <example>C:/user5 would return all entries in that folder, and C:/user5/user2</example>
        /// <returns>Dictionary, hash pairs.</returns>
        public Dictionary<string, string> GetSetEntriesDirectory(string directory)
        {
            SqliteCommand command = new();
            Dictionary<string, string> returnDictionary = new();
            command.CommandText = @$"SELECT * FROM {_defaultTable} WHERE directory LIKE $like";
            command.Parameters.AddWithValue("$like", $"{directory}%");
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
