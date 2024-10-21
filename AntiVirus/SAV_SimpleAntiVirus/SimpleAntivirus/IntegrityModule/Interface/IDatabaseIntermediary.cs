/**************************************************************************
 * File:        IDatabaseIntermediary.cs
 * Author:      Christopher Thompson, others
 * Description: Interface for DatabaseIntermediary
 * Last Modified: 29/09/2024
 **************************************************************************/


using Microsoft.Data.Sqlite;
namespace SimpleAntivirus.IntegrityModule.Interface
{

    public interface IDatabaseIntermediary : IDisposable
    {
        public void Dispose();

        /// <summary>
        /// Query the database for any command that does not return data, eg. Insert/Delete
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Int (The amount of rows changed)</returns>
        public int QueryNoReader(SqliteCommand query);

        /// <summary>
        /// Query the database for any command that RETURNS data, eg. SELECT
        /// </summary>
        /// <param name="query"></param>
        /// <returns>SqliteDataReader (Can have methods utilised upon it for more information)</returns>
        public SqliteDataReader QueryReader(SqliteCommand query);

        /// <summary>
        /// Get the amount of rows in the table
        /// </summary>
        /// <remarks>
        /// tableName should not be provided by the user (SQLite Injection Vulnerable)
        /// </remarks>
        /// <returns></returns>
        public long QueryAmount(string tableName = null);


        // Multidimensional array
        // Import debug display here for example (Note to self)

        /// <summary>
        /// Similar to QueryReader, however provides output as a multidimensional array, if one does not want to deal with SqliteDataReader
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Two Dimensional Array that contains string, table-like structure eg. [0][0] (first row, first entry)</returns>
        public List<List<string>> QueryReaderAsText(SqliteCommand query);

        /// <summary>
        /// When you delete items from SQLite, size does not decrease, you must call VACUUM to rebuild database and reduce size.
        /// </summary>
        public void Vacuum();

        public SqliteConnection Connection { get; }
    }
}
