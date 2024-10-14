using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace IntegrityCheckingFromScratch
{
    public class DatabaseHandler
    {
        private int triesLimit;
        protected SqliteConnection _databaseConnection;
        private string _databasePath;
        private string encryptionKey = "";
        public DatabaseHandler(string databaseName)
        {
            triesLimit = 8;
            _databasePath = FindDatabase(databaseName);
            Console.WriteLine($"Specific Database Directory Loaded: {_databasePath}");
            if (_databasePath != null)
            {
                SqliteConnectionStringBuilder connectionParameters = new SqliteConnectionStringBuilder();
                connectionParameters.DataSource = _databasePath;
                _databaseConnection = new SqliteConnection(connectionParameters.ConnectionString);
                _databaseConnection.Open();
            }
        }

        private string DirectorySearcher(List<string> pathCandidates, string term)
        {
            foreach (string pathItem in pathCandidates)
            {
                if (pathItem.Contains("Databases"))
                {
                    return pathItem;
                }
            }
            return null;
        }

        public string FindDatabase(string databaseName)
        {
            int tries = 0;
            string tempReturn = "";
            // Getting current directory appears to be fought upon online... the line below may need to be changed if compiling causes issues.
            string movingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            while (tries < triesLimit)
            {
                tempReturn = DirectorySearcher(Directory.GetDirectories(movingDirectory).ToList(), "Databases");
                if (tempReturn != null)
                {
                    Console.WriteLine($"Found databases path: {tempReturn}");
                    return DirectorySearcher(Directory.GetFiles(tempReturn).ToList(), databaseName);
                }
                movingDirectory = Directory.GetParent(movingDirectory).FullName;
                Console.WriteLine(movingDirectory);
                tries++;
            }
            // Maybe change this to get root directory???
            
            return "";
        }

        public bool ExecuteNonReturnQuery(SqliteCommand queryCommand)
        {
            // Ensure database connection is valid before attempting to execute query.
            if (_databaseConnection.State == System.Data.ConnectionState.Open)
            {
                queryCommand.Connection = _databaseConnection;
                // Return true if rows actually changed at all.
                return (queryCommand.ExecuteNonQuery() > 0);
            }
            else
            {
                return false;
            }
        }

        public SqliteDataReader ExecuteReturnQuery(SqliteCommand returnCommand)
        {
            if (_databaseConnection.State == System.Data.ConnectionState.Open)
            {
                returnCommand.Connection = _databaseConnection;
                return returnCommand.ExecuteReader();
            }
            return null;
        }
    }
}
