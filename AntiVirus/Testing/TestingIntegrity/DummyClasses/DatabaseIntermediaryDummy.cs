/**************************************************************************
 * File:        DatabaseIntermediary.cs
 * Author:      Christopher Thompson, others
 * Description: The parent class for all database readers, provides infrastructure to maintain a connection along with queries.
 * Last Modified: 6/08/2024
 * Libraries:   [Location Libraries / Dependencies]
 **************************************************************************/


using Microsoft.Data.Sqlite;
using System.IO;
using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.IntegrityModule.Interface;
using SimpleAntivirus.GUI.Services.Interface;
namespace TestingIntegrity.DummyClasses
{
  
    public class DatabaseIntermediaryDummy : IDatabaseIntermediary
    {
        protected SqliteConnection _databaseConnection;
        protected string _defaultTable;

        /// <summary>
        /// Constructor
        /// Finds database folder, and opens connection to the database.
        /// </summary>
        /// <param name="databaseName">Name of database SQLite file</param>
        public DatabaseIntermediaryDummy(string databaseName, bool makeDatabase = false, string defaultTable = "")
        {

        }

        public void Dispose()
        {
            
        }



        /// <summary>
        /// Private function, determines whether the database can be used.
        /// </summary>
        private bool DatabaseUsable()
        {
            return true;
        }

        /// <summary>
        /// Query the database for any command that does not return data, eg. Insert/Delete
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Int (The amount of rows changed)</returns>
        public int QueryNoReader(SqliteCommand query)
        {
            return 1;
        }

        /// <summary>
        /// Query the database for any command that RETURNS data, eg. SELECT
        /// </summary>
        /// <param name="query"></param>
        /// <returns>SqliteDataReader (Can have methods utilised upon it for more information)</returns>
        public SqliteDataReader QueryReader(SqliteCommand query)
        {
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
            return 2;
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
            return null;
        }

        /// <summary>
        /// When you delete items from SQLite, size does not decrease, you must call VACUUM to rebuild database and reduce size.
        /// </summary>
        public void Vacuum()
        {
            
        }

        public SqliteConnection Connection
        {
            get
            {
                return null;
            }
        }
    }
}
