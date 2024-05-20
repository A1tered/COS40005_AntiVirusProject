using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
namespace FindTheHash
{
    public class DatabaseConnector
    {
        private SqliteConnection _sqliteConnectionRepresentation;
        private string _tableName;
        public DatabaseConnector(string databaseDirectory)
        {
            _tableName = "integrityTrack";
            
            if (!Directory.Exists("Databases"))
            {
                Directory.CreateDirectory("Databases");
            }
            SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder();
            stringBuilder.Add("Mode", SqliteOpenMode.ReadWrite);
            stringBuilder.Add("Data Source", $"{databaseDirectory}");
            _sqliteConnectionRepresentation = new SqliteConnection(stringBuilder.ToString());
            _sqliteConnectionRepresentation.Open();
        }

        public void CleanUp()
        {
            _sqliteConnectionRepresentation.Close();
        }

        public Tuple<string, long, long, long> QueryDirectoryData(string directory)
        {
            if (ConnectionSuccessful())
            {
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@$"
                SELECT * FROM {_tableName} WHERE directory = $directory;
                ");
                //commandCreation.Parameters.AddWithValue("$table", _tableName);
                commandCreation.Parameters.AddWithValue("$directory", directory);
                SqliteDataReader sqliteDataReader = commandCreation.ExecuteReader();
                sqliteDataReader.Read();
                if (sqliteDataReader.HasRows)
                {
                    string hash = sqliteDataReader.GetString(1);
                    long modificationTime = sqliteDataReader.GetInt64(2);
                    long signatureTime = sqliteDataReader.GetInt64(3);
                    long sizeBytes = sqliteDataReader.GetInt64(4);
                    return new Tuple<string, long, long, long>(hash, modificationTime, signatureTime, sizeBytes);
                }
            }
            return new Tuple<string, long, long, long>("", 0, 0, 0);
        }

        public List<String> QueryDirectories()
        {
            if (ConnectionSuccessful())
            {
                List<string> directoryReturn = new();
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@$"
                SELECT * FROM {_tableName};
                ");
                SqliteDataReader sqliteDataReader = commandCreation.ExecuteReader();

                while (sqliteDataReader.Read())
                {
                    directoryReturn.Add(sqliteDataReader.GetString(0));
                }
                return directoryReturn;
            }
            return new List<string>();
        }

        public bool EmptyDatabase()
        {
            if (ConnectionSuccessful())
            {
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@$"DELETE FROM integrityTrack");
                int sqliteResult = commandCreation.ExecuteNonQuery();
                return (sqliteResult > 0); // If successful, amount of rows should be bigger than 0.
            }
            return false;
        }

        public bool AddIntegrityEntry(string directory, string hash, long modificationTime, long signatureTime, long sizeBytes)
        {
            if (ConnectionSuccessful())
            {
                Tuple<string, long, long, long> result = QueryDirectoryData(directory);
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                if (result.Item1 == "") // No existent entry
                {
                    Console.WriteLine("No record, adding new record.");
                    commandCreation.CommandText = (@$"INSERT INTO {_tableName} VALUES ($directory, $hash, $modificationTime, $signatureTime, $size) ;
                ");
                }
                else // Update existing value
                {
                    Console.WriteLine("Existent entry, updating entry.");
                    commandCreation.CommandText = (@$"UPDATE {_tableName} SET directory = $directory, hash = $hash, modificationTime = $modificationTime, signatureTime = $signatureTime, size = $size WHERE hash=$replaceHash And signatureTime=$replaceSignatureTime ;");
                    commandCreation.Parameters.AddWithValue("$replaceHash", result.Item1);
                    commandCreation.Parameters.AddWithValue("$replaceSignatureTime", result.Item3);
                }
                commandCreation.Parameters.AddWithValue("$directory", directory);
                commandCreation.Parameters.AddWithValue("$hash", hash);
                commandCreation.Parameters.AddWithValue("$modificationTime", modificationTime);
                commandCreation.Parameters.AddWithValue("$signatureTime", signatureTime);
                commandCreation.Parameters.AddWithValue("$size", sizeBytes);
                int sqliteResult = commandCreation.ExecuteNonQuery();
                return (sqliteResult > 0); // If successful, amount of rows should be bigger than 0.
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
