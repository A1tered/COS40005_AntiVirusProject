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
            _tableName = "hashTable";
            
            if (!Directory.Exists("Databases"))
            {
                Directory.CreateDirectory("Databases");
            }
            SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder();
            stringBuilder.Add("Mode", SqliteOpenMode.ReadOnly);
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
                SqliteCommand commandCreation = _sqliteConnectionRepresentation.CreateCommand();
                commandCreation.CommandText = (@"
                SELECT * FROM hashTable WHERE hash = $hash;
                ");
                //commandCreation.Parameters.AddWithValue("$table", _tableName);
                commandCreation.Parameters.AddWithValue("$hash", hash);
                SqliteDataReader sqliteDataReader = commandCreation.ExecuteReader();
                sqliteDataReader.Read();
                if (sqliteDataReader.HasRows)
                {
                    string queryResult = sqliteDataReader.GetString(0);
                    Console.WriteLine("query result");
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
