/**************************************************************************
 * File:        DatabaseIntermediary.cs
 * Author:      Christopher Thompson, others
 * Description: The parent class for all database readers, provides infrastructure to maintain a connection along with queries.
 * Last Modified: 6/08/2024
 * Libraries:   [Location Libraries / Dependencies]
 **************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using DatabaseFoundations.IntegrityRelated;
namespace DatabaseFoundations
{
  
    public class DatabaseIntermediary
    {
        protected SqliteConnection _databaseConnection;
        protected string _defaultTable;

        /// <summary>
        /// Constructor
        /// Finds database folder, and opens connection to the database.
        /// </summary>
        /// <param name="databaseName">Name of database SQLite file</param>
        public DatabaseIntermediary(string databaseName, bool makeDatabase = false, string defaultTable = "")
        {
            // Find database folder
            string returnedDirectoryDatabase = FileInfoRequester.FileDirectorySearcher(AppDomain.CurrentDomain.BaseDirectory, "Databases");
            string databaseSpecificPath;
            _defaultTable = defaultTable;
            // If database folder does not exist, make one.
            if (returnedDirectoryDatabase == null)
            {
                returnedDirectoryDatabase = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases")).FullName;
            }
            // Does the database exist within Database folder?
            databaseSpecificPath = FileInfoRequester.FileDirectorySearcher(returnedDirectoryDatabase, databaseName);
            if (databaseSpecificPath != null || makeDatabase)
            {
                // Make database.
                if (databaseSpecificPath == null)
                {
                    databaseSpecificPath = Path.Combine(returnedDirectoryDatabase, databaseName);
                }
                SqliteConnectionStringBuilder connectionBuild = new();
                Console.WriteLine(databaseSpecificPath);
                connectionBuild.DataSource = databaseSpecificPath;
                connectionBuild.Mode = SqliteOpenMode.ReadWriteCreate;
                _databaseConnection = new SqliteConnection(connectionBuild.ConnectionString);
                _databaseConnection.Open();
                SqliteCommand commandPragma = new();
                commandPragma.CommandText = "PRAGMA journal_mode=WAL";
                QueryNoReader(commandPragma);

            }
            else
            {
                // If the database is not allowed to be created, then return error if it does not exist.
                throw new Exception("Database non-existent");
            }
        }

        ~DatabaseIntermediary()
        {
            _databaseConnection.Close();
        }

        /// <summary>
        /// Private function, determines whether the database can be used.
        /// </summary>
        private bool DatabaseUsable()
        {
            if (_databaseConnection != null)
            {
                if (_databaseConnection.State == System.Data.ConnectionState.Open)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Query the database for any command that does not return data, eg. Insert/Delete
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Int (The amount of rows changed)</returns>
        public int QueryNoReader(SqliteCommand query)
        {
            if (DatabaseUsable())
            {
                query.Connection = _databaseConnection;
                return query.ExecuteNonQuery();
            }
            return 0;
        }

        /// <summary>
        /// Query the database for any command that RETURNS data, eg. SELECT
        /// </summary>
        /// <param name="query"></param>
        /// <returns>SqliteDataReader (Can have methods utilised upon it for more information)</returns>
        public SqliteDataReader QueryReader(SqliteCommand query)
        {
            if (DatabaseUsable())
            {
                query.Connection = _databaseConnection;
                return query.ExecuteReader();
            }
            return null;
        }

        /// <summary>
        /// Get the amount of rows in the table
        /// </summary>
        /// <remarks>
        /// tableName should not be provided by the user (SQLite Injection Vulnerable)
        /// </remarks>
        /// <returns></returns>
        public long QueryAmount(string tableName = null)
        {
            string insertTable = tableName;
            if (tableName == null)
            {
                insertTable = _defaultTable;
            }
            if (DatabaseUsable())
            {
                SqliteCommand command = _databaseConnection.CreateCommand();
                // Concerns may arise from inserting tableName like this, however SQLite parameters does not support placement of table names,
                // do not allow user direct input for this function.
                command.CommandText = $"SELECT COUNT(*) FROM {insertTable}";
                SqliteDataReader returnInfo = command.ExecuteReader();
                returnInfo.Read();
                return returnInfo.GetInt64(0);
            }
            return -1;
        }


        // Multidimensional array
        // Import debug display here for example (Note to self)

        /// <summary>
        /// Similar to QueryReader, however provides output as a multidimensional array, if one does not want to deal with SqliteDataReader
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Two Dimensional Array that contains string, table-like structure eg. [0][0] (first row, first entry)</returns>
        public List<List<string>> QueryReaderAsText(SqliteCommand query)
        {
            List<List<string>> output = new();
            List<string> row = new();
            if (DatabaseUsable())
            {
                query.Connection = _databaseConnection;
                SqliteDataReader dataReader = query.ExecuteReader();
                while (dataReader.Read())
                {
                    row = new();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        row.Add(dataReader.GetString(i));
                    }
                    output.Add(row);
                }
            }
            return output;
        }

        /// <summary>
        /// When you delete items from SQLite, size does not decrease, you must call VACUUM to rebuild database and reduce size.
        /// </summary>
        public void Vacuum()
        {
            SqliteCommand command = _databaseConnection.CreateCommand();
            command.CommandText = "VACUUM";
            QueryNoReader(command);
        }
    }
}
