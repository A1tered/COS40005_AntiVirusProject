/**************************************************************************
* File:        [DatabaseHandler].cs
* Author:      [Pawan]
* Description: [Handles SQLite database]
* Last Modified: [14/09/2024]
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace SimpleAntivirus.MaliciousCodeScanning
{
    public class DatabaseHandler
    {
        private readonly string connectionString;

        // Constructor to initialize the connection string
        public DatabaseHandler(string dbPath)
        {
            connectionString = $"Data Source={dbPath}";
            EnsureTableExists();
        }

        // Ensure the MaliciousCommands table exists, if not, create it
        private void EnsureTableExists()
        {
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS MaliciousCommands (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        command TEXT NOT NULL
                    );";

                using (SqliteCommand cmd = new SqliteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Fetch all malicious commands from the database
        public List<string> GetMaliciousCommands()
        {
            List<string> maliciousCommands = new List<string>();

            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT command FROM MaliciousCommands;";

                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                using (SqliteDataReader reader = cmd.ExecuteReader())
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
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                string insertQuery = "INSERT INTO MaliciousCommands (command) VALUES (@command);";
                using (SqliteCommand cmd = new SqliteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@command", command);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
