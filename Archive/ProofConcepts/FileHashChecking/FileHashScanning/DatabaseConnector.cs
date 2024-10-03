using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace FileHashScanning
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
        public DatabaseConnector(string databaseDirectory, bool writeAccess = false)
        {
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
                stringBuilder.Add("Mode", SqliteOpenMode.ReadWrite);
            }
            stringBuilder.Add("Data Source", $"{databaseDirectory}");
            _sqliteConnectionRepresentation = new SqliteConnection(stringBuilder.ToString());
            _sqliteConnectionRepresentation.Open();
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

        private bool ConnectionSuccessful() // If connection is ready for commands, returns true.
        {
            bool tempConnectionState = _sqliteConnectionRepresentation.State == System.Data.ConnectionState.Open;
            if (tempConnectionState == false)
            {
                Console.WriteLine("Connection not established, command rejected.");
            }
            return (tempConnectionState);
        }
    }
}
