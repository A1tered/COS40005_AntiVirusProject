/**************************************************************************
 * File:        DatabaseConnector.cs
 * Author:      Joel Parks, others
 * Description: Manages all database related functions. Involves querying, adding, removing hashes.
 * Last Modified: 8/10/2024
 **************************************************************************/

using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;

namespace SimpleAntivirus.FileHashScanning
{
    /// <summary>
    /// Responsible for connecting to the database and handling queries. 
    /// 
    /// It formats data for return so upper classes do not have to handle any deserialisation.
    /// </summary>
    public class DatabaseConnector
    {
        private SqliteConnection _sqliteConnectionRepresentation;
        private string _tableName;
        public DatabaseConnector(string databaseDirectory, bool writeAccess = false, bool setupRun = false)
        {
            Open(databaseDirectory, writeAccess, setupRun);
        }

        public void Open(string databaseDirectory, bool writeAccess, bool setupRun = false)
        {

            ISetupService setupService = SetupService.GetExistingInstance();

            _tableName = "hashSignatures";

            if (!Directory.Exists("Databases"))
            {
                Directory.CreateDirectory("Databases");
            }
            SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder();
            if (!writeAccess)
            {
                stringBuilder.Add("Mode", SqliteOpenMode.ReadOnly);
            }
            else
            {
                stringBuilder.Add("Mode", SqliteOpenMode.ReadWriteCreate);
            }
            stringBuilder.Add("Data Source", $"{databaseDirectory}");
            stringBuilder.Add("Password", setupService.DbKey());
            _sqliteConnectionRepresentation = new SqliteConnection(stringBuilder.ToString());
            _sqliteConnectionRepresentation.Open();

            if (setupRun)
            {
                new SqliteCommand($"CREATE TABLE IF NOT EXISTS hashSignatures (sigHash TEXT);", _sqliteConnectionRepresentation).ExecuteNonQuery();
                SetupService.TransferContents(_sqliteConnectionRepresentation, System.IO.Directory.GetParent(databaseDirectory).ToString(), "sighash_initialisation_init.db", "hashSignatures");
            }
        }

        public void CleanUp()
        {
            _sqliteConnectionRepresentation.Close();
        }

        public bool QueryHash(string hash)
        {
            if (ConnectionSuccessful())
            {
                // Scan hash on system (Upper case) -> Hashes in database (lower case)
                //                                      Lower Case / Upper case
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@"
                SELECT * FROM hashSignatures WHERE sigHash = $hash COLLATE NOCASE;
                ");
                //commandCreation.Parameters.AddWithValue("$table", _tableName);
                commandCreation.Parameters.AddWithValue("$hash", hash);
                SqliteDataReader sqliteDataReader = commandCreation.ExecuteReader();
                sqliteDataReader.Read();
                if (sqliteDataReader.HasRows)
                {
                    string queryResult = sqliteDataReader.GetString(0);
                    return true;
                }
            }
            return false;
        }



        public int[] QueryHashBatch(string[] hash) // Attempt to efficiently do set of hashes, return index
        {
            SqliteDataReader readerObjectAsync;
            List<Task> _dbReaders = new();
            List<int> returnIndex = new();
            int index = 0;
            if (ConnectionSuccessful())
            {
                foreach (string hashComponent in hash)
                {
                    SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                    commandCreation.CommandText = (@"
                SELECT * FROM hashSignatures WHERE sigHash = $hash;
                ");
                    //commandCreation.Parameters.AddWithValue("$table", _tableName);
                    commandCreation.Parameters.AddWithValue("$hash", hashComponent);
                    _dbReaders.Add(commandCreation.ExecuteReaderAsync(CommandBehavior.SingleResult));
                }
                Task.WaitAll(_dbReaders.ToArray());
                foreach (Task<SqliteDataReader> taskReader in _dbReaders)
                {
                    readerObjectAsync = taskReader.Result;
                    readerObjectAsync.Read();
                    if (readerObjectAsync.HasRows)
                    {
                        string queryResult = readerObjectAsync.GetString(0);
                        returnIndex.Add(index);
                    }
                    index++;
                }
            }
            return returnIndex.ToArray();
        }


        public bool AddHash(string hash)
        {
            if (QueryHash(hash))
            {
                return false;
            }
            if (ConnectionSuccessful())
            {
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@"
                INSERT INTO hashSignatures VALUES ($sigHash);
                ");
                //commandCreation.Parameters.AddWithValue("$table", _tableName);
                commandCreation.Parameters.AddWithValue("$sigHash", hash);
                int sqliteDataReader = commandCreation.ExecuteNonQuery();
                if (sqliteDataReader > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool RemoveHash(string hash)
        {
            if (QueryHash(hash))
            {
                return false;
            }
            if (ConnectionSuccessful())
            {
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@"
                DELETE FROM hashSignatures WHERE sigHash = ($sigHash);
                ");
                //commandCreation.Parameters.AddWithValue("$table", _tableName);
                commandCreation.Parameters.AddWithValue("$sigHash", hash);
                int sqliteDataReader = commandCreation.ExecuteNonQuery();
                if (sqliteDataReader > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private bool ConnectionSuccessful() // If connection is ready for commands, returns true.
        {
            bool tempConnectionState = _sqliteConnectionRepresentation.State == System.Data.ConnectionState.Open;
            return (tempConnectionState);
        }
    }
}
