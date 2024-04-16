using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Data.Sqlite;

namespace DatabaseProofOfConcept
{
    public class Database
    {
        private SqliteConnection _sqliteConnectionRepresentation;
        private string[] _typeEnforcer;
        private int _idTracker;
        /// <summary>
        /// Initialize database name
        /// </summary>
        /// <param name="databaseName"></param>
        public Database(string databaseName = "Example2.db") // Constructor, makes a new database / connects to existing database
        {
            if (!Directory.Exists("Databases"))
            {
                Directory.CreateDirectory("Databases");
            }
            _idTracker = 0;
            SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder();
            _typeEnforcer = ["NULL", "INTEGER", "REAL", "BLOB", "TEXT"];
            stringBuilder.Add("Mode", SqliteOpenMode.ReadWriteCreate);
            stringBuilder.Add("Data Source", $"Databases/databaseName");
            _sqliteConnectionRepresentation = new SqliteConnection(stringBuilder.ToString());
            _sqliteConnectionRepresentation.Open();
        }
        /// <summary>
        /// Create table, tableName == Table of Database, Datavalues are name type, name type, name type
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataValues"></param>
        /// <returns></returns>
        /// 
        public string ArrayNameTypeTransformer(string[] name, string[] type)
        {
            StringBuilder stringBuild = new StringBuilder();
            if (name.Count() == type.Count())
            {
                for (int indexer = 0; indexer < name.Count(); indexer++)
                {
                    stringBuild.Append($"{name[indexer]} {type[indexer]}");
                    if (indexer < name.Count() - 1) // Add comma seperator till end.
                    {
                        stringBuild.Append(",");
                    }
                }
            }
            return (stringBuild.ToString());
        }
        /// <summary>
        /// OUTPUT: (column, column, column) VALUES (value, value, value)
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ArrayColumnValueTransformer(string[] column, string[] value)
        {
            StringBuilder stringBuild = new StringBuilder();
            if (column.Count() == value.Count())
            {
                stringBuild.Append("(");
                for (int indexer = 0; indexer < column.Count();indexer++)
                {
                    stringBuild.Append($"{column[indexer]}");
                    if (indexer < column.Count() - 1)
                    {
                        stringBuild.Append(",");
                    }
                }
                stringBuild.Append(")");
                stringBuild.Append(" VALUES(");
                for (int indexer = 0; indexer < column.Count(); indexer++)
                {
                    
                    stringBuild.Append($"'{value[indexer]}'");
                    if (indexer < column.Count() - 1)
                    {
                        stringBuild.Append(",");
                    }
                }
                stringBuild.Append(");");
            }
            return stringBuild.ToString();
        }

        public bool CreateTable(string tableName, string[] name, string[] type)
        {
            // Unpack
            string parameterExtract = ArrayNameTypeTransformer(name, type);
            if (ConnectionSuccessful())
            {
                try
                {
                    using (SqliteCommand sqliteCommandRepresentation = _sqliteConnectionRepresentation.CreateCommand())
                    {
                        sqliteCommandRepresentation.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName}({parameterExtract});";
                        Console.WriteLine(sqliteCommandRepresentation.CommandText);
                        sqliteCommandRepresentation.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (SqliteException exception)
                {
                    Console.WriteLine(exception.Message); // Debug error.
                    return false;
                }
            }
            return false;
        }

        public string BasicQuery(string tableName, string[] columns)
        {
            // Unpack
            if (ConnectionSuccessful())
            {
                StringBuilder stringBuildColumns = new StringBuilder();
                // Quick Unpack
                for (int indexer = 0; indexer < columns.Count(); indexer++)
                {
                    stringBuildColumns.Append(columns[indexer]);
                    if (indexer < columns.Count() - 1)
                    {
                        stringBuildColumns.Append(",");
                    }
                }



                try
                {
                    using (SqliteCommand sqliteCommandRepresentation = _sqliteConnectionRepresentation.CreateCommand())
                    {
                        sqliteCommandRepresentation.CommandText = $"SELECT {stringBuildColumns} FROM {tableName}";
                        Console.WriteLine(sqliteCommandRepresentation.CommandText);
                        SqliteDataReader dataReader = sqliteCommandRepresentation.ExecuteReader();
                        StringBuilder dataResultBuilder = new();
                        while (dataReader.Read())
                        {
                            for (int dataIndex = 0; dataIndex < dataReader.FieldCount; dataIndex++)
                            {
                                dataResultBuilder.Append(dataReader.GetString(dataIndex));
                                if (dataIndex < dataReader.FieldCount - 1)
                                {
                                    dataResultBuilder.Append(" | ");
                                }
                            }
                            dataResultBuilder.Append("\n");
                        }
                        return dataResultBuilder.ToString();
                        
                    }
                }
                catch (SqliteException exception)
                {
                    Console.WriteLine(exception.Message); // Debug error.
                    return "null";
                }
            }
            return "null";
        }

        public bool InsertValuesExplicit(string tableName, string[] columns, string[] values)
        {
            _idTracker += 1;
            string dataValues = ArrayColumnValueTransformer(columns, values);
            using (SqliteCommand sqliteCommandRepresentation = _sqliteConnectionRepresentation.CreateCommand())
            {
                sqliteCommandRepresentation.CommandText = $"INSERT INTO {tableName}{dataValues}";
                Console.WriteLine(sqliteCommandRepresentation.CommandText);
                try
                {
                    sqliteCommandRepresentation.ExecuteNonQuery();
                    return true;
                }
                catch (SqliteException exception)
                {
                    Console.WriteLine(exception.Message);
                    return false;
                }
            }
        }

        bool ConnectionSuccessful() // If connection is ready for commands, returns true.
        {
            bool tempConnectionState = _sqliteConnectionRepresentation.State == System.Data.ConnectionState.Open;
            if (tempConnectionState == false)
            {
                Console.WriteLine("Connection not established, command rejected.");
            }
            return (tempConnectionState);
        }

        public int IDCurrent
        {
            get { return _idTracker; }
        }
    }
}
