﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
namespace DatabaseFoundations
{
  
    public class DatabaseIntermediary
    {
        protected SqliteConnection _databaseConnection;


        /// <summary>
        /// Constructor
        /// Finds database folder, and opens connection to the database.
        /// </summary>
        /// <param name="databaseName">Name of database SQLite file</param>
        public DatabaseIntermediary(string databaseName)
        {
            string returnedDirectoryDatabase = FileDirectorySearcher(AppDomain.CurrentDomain.BaseDirectory, "Databases");
            string databaseSpecificPath;
            if (returnedDirectoryDatabase != null)
            {
                databaseSpecificPath = FileDirectorySearcher(returnedDirectoryDatabase, databaseName);
                SqliteConnectionStringBuilder connectionBuild = new();
                connectionBuild.DataSource = databaseSpecificPath;
                _databaseConnection = new SqliteConnection(connectionBuild.ConnectionString);
                _databaseConnection.Open();
            }
            else
            {
                // Create databases folder (Not to be done in real production environment, because we want to supply the database)
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases"));
                throw new Exception("Please put already made databases in folder");
            }
        }

        /// <summary>
        /// Private function that finds files/directories with certain names, within the provided directory.
        /// </summary>
        /// <param name="startDirectory">Name of database SQLite file</param>\
        /// <param name="term">Search term for what the desired file path should have</param>
        private string FileDirectorySearcher(string startDirectory, string term)
        {
            string[] filePaths = Directory.GetFiles(startDirectory).Concat(Directory.GetDirectories(startDirectory)).ToArray();

            string databasePath = null;
            // Find path that contains database.
            foreach (string path in filePaths)
            {
                if (Path.GetFileNameWithoutExtension(path).Contains(term))
                {
                    return path;
                }
            }
            return null;
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
        public long QueryAmount(string tableName)
        {
            if (DatabaseUsable())
            {
                SqliteCommand command = _databaseConnection.CreateCommand();
                // Concerns may arise from inserting tableName like this, however SQLite parameters does not support placement of table names,
                // do not allow user direct input for this function.
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
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
    }
}
