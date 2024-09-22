/**************************************************************************
* File:        [DatabaseHandler].cs
* Author:      [Pawan]
* Description: [Handles SQLite database]
* Last Modified: [14/09/2024]
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace SimpleAntivirus.MaliciousCodeScanning
{
    public class DatabaseHandler
    {
        private readonly string connectionString;

        // Constructor to initialize the connection string
        public DatabaseHandler(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Version=3;";
            EnsureTableExists();
        }

        // Ensure the MaliciousCommands table exists, if not, create it
        private void EnsureTableExists()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS MaliciousCommands (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        command TEXT NOT NULL
                    );";

                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Fetch all malicious commands from the database
        public List<string> GetMaliciousCommands()
        {
            List<string> maliciousCommands = new List<string>();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT command FROM MaliciousCommands;";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        maliciousCommands.Add(reader["command"].ToString());
                    }
                }
            }

            return maliciousCommands;
        }

        // Insert malicious commands into the database
        public void InsertMaliciousCommand(string command)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string insertQuery = "INSERT INTO MaliciousCommands (command) VALUES (@command);";
                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@command", command);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
