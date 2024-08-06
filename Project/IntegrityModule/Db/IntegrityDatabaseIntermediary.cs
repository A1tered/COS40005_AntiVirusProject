using DatabaseFoundations.IntegrityRelated;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseFoundations
{
    public class IntegrityDatabaseIntermediary : DatabaseIntermediary
    {
        public IntegrityDatabaseIntermediary(string databaseName, bool firstRun) : base(databaseName, firstRun)
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
            command.CommandText = @"CREATE TABLE IF NOT EXISTS
            IntegrityTrack(directory text PRIMARY KEY, hash text, modificationTime int, signatureCreation int, originalSize int)";
            return QueryNoReader(command) > 0;
        }

        /// <summary>
        /// Deletes all entries from IntegrityTrack table
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll()
        {
            SqliteCommand command = new();
            command.CommandText = "DELETE FROM IntegrityTrack";
            return QueryNoReader(command) > 0;
        }

        /// <summary>
        /// Add integrity path, adds entry into IntegrityDatabase such as Hash info, file info, time of signature creation.
        /// </summary>
        /// <param name="path">Directory windows (file or directory)</param>
        /// <returns>False if nothing changed or most recent addition failed, True if no issues</returns>
        public bool AddEntry(string path)
        {
            List<string> pathProcess = new();
            Queue<string> directoryProcess = new();
            bool success = false;
            // If for any process to access status, or for debug console:
            int progress = 0;
            int maxSize = 0;
            string tempPathUnpack = "";
            if (Directory.Exists(path))
            {
                Directory.GetDirectories(path).ToList().ForEach(directoryProcess.Enqueue);
                // Item is directory, so process contents
                Directory.GetFiles(path).ToList<string>().ForEach(pathProcess.Add);
                while (directoryProcess.Count() > 0 && pathProcess.Count() < 10000)
                {
                    tempPathUnpack = directoryProcess.Dequeue();
                    Directory.GetDirectories(tempPathUnpack).ToList().ForEach(directoryProcess.Enqueue);
                    Directory.GetFiles(tempPathUnpack).ToList<string>().ForEach(pathProcess.Add);
                }
            }
            else
            {
                pathProcess.Add(path);
            }
            maxSize = pathProcess.Count();
            foreach (string cyclePath in pathProcess)
            {
                Console.Write($"\r{progress}/{maxSize}");
                progress++;
                SqliteCommand command = new();
                string getHash = FileInfoRequester.HashFile(cyclePath);
                Tuple<long, long> fileInfo = FileInfoRequester.RetrieveFileInfo(cyclePath);
                if (CheckExistence(cyclePath))
                {
                    Console.WriteLine("Warning, replacing existing entry");
                }
                if (getHash != "")
                {
                    command.CommandText = @"REPLACE INTO IntegrityTrack VALUES($path, $hash, $modTime, $sigCreation, $orgSize)";
                    command.Parameters.AddWithValue("$path", cyclePath);
                    command.Parameters.AddWithValue("$hash", getHash);
                    command.Parameters.AddWithValue("$modTime", fileInfo.Item1);
                    command.Parameters.AddWithValue("$sigCreation", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
                    command.Parameters.AddWithValue("$orgSize", fileInfo.Item2);
                    success = QueryNoReader(command) > 0;
                }
            }
            return success;
        }

        public bool RemoveEntry(string path)
        {
            List<string> pathsToRemove = new();
            bool success = false;
            int progress = 0;
            int maxProgress;
            if (Directory.Exists(path))
            {
               pathsToRemove = Directory.GetFiles(path).ToList();
            }
            else
            {
                pathsToRemove.Add(path);
            }
            maxProgress = pathsToRemove.Count();
            foreach (string pathCycle in pathsToRemove)
            {
                Console.Write($"\r Deleted {progress}/{maxProgress}");
                progress++;
                SqliteCommand command = new();
                command.CommandText = @"DELETE FROM IntegrityTrack WHERE directory=$directorySet";
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
        public bool CheckExistence(string path)
        {
            SqliteCommand command = new();
            command.CommandText = @"SELECT directory FROM IntegrityTrack WHERE directory = $directorySet";
            command.Parameters.AddWithValue("$directorySet", path);
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
            command.CommandText = "SELECT * FROM IntegrityTrack WHERE directory = $directorySet";
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
            command.CommandText = @"SELECT * FROM IntegrityTrack"; // LIMIT $limit OFFSET $offset
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
