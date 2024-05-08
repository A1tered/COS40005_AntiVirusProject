using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace FileHashChecking
{
    public class Database
    {
        private SqliteConnection _connection;
        private string _tableName;
        public Database(string databaseDir)
        {
            _tableName = "hashSignatures";

            if (!Directory.Exists("Databases"))
            {
                Directory.CreateDirectory("Databases");
            }

            try
            {
                SqliteConnectionStringBuilder stringBuilder = new SqliteConnectionStringBuilder();
                stringBuilder.Add("Mode", SqliteOpenMode.ReadOnly);
                stringBuilder.Add("Data Source", databaseDir);
                _connection = new SqliteConnection(stringBuilder.ToString());
                _connection.Open();
            }
            catch (Exception ex) 
            {
                return;
            }
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        private bool ConnectionSuccessful()
        {
            bool tempConnState = _connection.State == System.Data.ConnectionState.Open;
            if (tempConnState == false) 
            {
                Console.WriteLine("Connection not established, query refused.");
            }
            return tempConnState;
        }

        public bool Scan(string hash)
        {
            if(ConnectionSuccessful())
            {
                try 
                {
                    SqliteCommand command = _connection.CreateCommand();
                    command.CommandText = (@"
                SELECT * FROM hashSignatures WHERE sigHash = $hash;
                ");

                    // command.Paramaters.AddWithValue("$table", _tableName_);
                    command.Parameters.AddWithValue("$hash", hash);
                    SqliteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        string queryResult = reader.GetString(0);
                        Console.Clear();
                        if (queryResult != "1")
                        {
                            Console.WriteLine($"Threats found! {queryResult} threats have been quarantined.");
                        }
                        else if (queryResult == "1") 
                        {
                            Console.WriteLine($"Threats found! {queryResult} threat has been quarantined.");
                        }
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No threats detected.");
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
