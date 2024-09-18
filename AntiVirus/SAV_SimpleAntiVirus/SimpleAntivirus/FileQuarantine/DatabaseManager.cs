using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Diagnostics;


namespace SimpleAntivirus.FileQuarantine
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor for DatabaseManager. Initializes the database connection string.
        /// Ensures the database directory and file are created if they do not exist.
        /// </summary>
        /// <param name="databasePath">The path to the SQLite database file.</param>
        public DatabaseManager(string databasePath)
        {
            // Ensure the directory exists before proceeding
            string directory = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.WriteLine($"Database directory created at {directory}");
            }

            _connectionString = $"Data Source={databasePath}";
            InitializeDatabase();
        }

        /// <summary>
        /// Initializes the SQLite database by creating the necessary tables if they do not exist.
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    // Create the QuarantinedFiles table if it does not exist
                    string createQuarantinedFilesTable = @"
                    CREATE TABLE IF NOT EXISTS QuarantinedFiles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FilePath TEXT NOT NULL,
                        OriginalFilePath TEXT NOT NULL,
                        QuarantineDate TEXT NOT NULL
                    );";

                    // Create the WhitelistedFiles table if it does not exist
                    string createWhitelistedFilesTable = @"
                    CREATE TABLE IF NOT EXISTS WhitelistedFiles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FilePath TEXT NOT NULL
                    );";

                    using (var command = new SqliteCommand(createQuarantinedFilesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (var command = new SqliteCommand(createWhitelistedFilesTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Debug.WriteLine("Database initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                // Log any errors encountered during database initialization
                Debug.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a file is whitelisted by querying the database.
        /// </summary>
        /// <param name="filePath">The full path of the file to check.</param>
        /// <returns>True if the file is whitelisted, otherwise false.</returns>
        public async Task<bool> IsWhitelistedAsync(string filePath)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the file exists in the whitelist
                    string query = "SELECT COUNT(*) FROM WhitelistedFiles WHERE FilePath = @FilePath";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking whitelist status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Adds a file to the whitelist by inserting its path into the database.
        /// </summary>
        /// <param name="filePath">The full path of the file to add to the whitelist.</param>
        public async Task<bool> AddToWhitelistAsync(string filePath)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Insert the file path into the whitelist table
                    string insertQuery = "INSERT INTO WhitelistedFiles (FilePath) VALUES (@FilePath)";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        await command.ExecuteNonQueryAsync();
                    }

                    Debug.WriteLine($"Added to whitelist: {filePath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding to whitelist: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes a file from the whitelist by deleting its path from the database.
        /// </summary>
        /// <param name="filePath">The full path of the file to remove from the whitelist.</param>
        public async Task RemoveFromWhitelistAsync(string filePath)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Remove the file path from the whitelist table
                    string deleteQuery = "DELETE FROM WhitelistedFiles WHERE FilePath = @FilePath";

                    using (var command = new SqliteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FilePath", filePath);
                        await command.ExecuteNonQueryAsync();
                    }

                    Debug.WriteLine($"Removed from whitelist: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing from whitelist: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all whitelisted files from the database.
        /// </summary>
        /// <returns>A list of file paths that are whitelisted.</returns>
        public async Task<IEnumerable<string>> GetWhitelistAsync()
        {
            var whitelist = new List<string>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Retrieve all file paths from the whitelist table
                    string query = "SELECT FilePath FROM WhitelistedFiles";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string filePath = reader.GetString(0);
                                whitelist.Add(filePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving whitelist: {ex.Message}");
            }

            return whitelist;
        }

        /// <summary>
        /// Stores information about a quarantined file in the database.
        /// </summary>
        /// <param name="quarantinedFilePath">The path where the file is quarantined.</param>
        /// <param name="originalFilePath">The original path of the file before quarantining.</param>
        public async Task StoreQuarantineInfoAsync(string quarantinedFilePath, string originalFilePath)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Insert quarantine information into the QuarantinedFiles table
                    string insertQuery = @"
                    INSERT INTO QuarantinedFiles (FilePath, OriginalFilePath, QuarantineDate)
                    VALUES (@FilePath, @OriginalFilePath, @QuarantineDate)";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FilePath", quarantinedFilePath);
                        command.Parameters.AddWithValue("@OriginalFilePath", originalFilePath);
                        command.Parameters.AddWithValue("@QuarantineDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        await command.ExecuteNonQueryAsync();
                    }

                    Debug.WriteLine("Quarantine information stored in the database.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error storing quarantine information: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a quarantine entry from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the quarantined file entry to remove.</param>
        public async Task RemoveQuarantineEntryAsync(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Delete the quarantine entry from the QuarantinedFiles table
                    string deleteQuery = "DELETE FROM QuarantinedFiles WHERE Id = @Id";

                    using (var command = new SqliteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    Debug.WriteLine($"Quarantine entry with ID {id} has been removed from the database.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing quarantine entry: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the quarantine details of a file by its ID.
        /// </summary>
        /// <param name="id">The ID of the quarantined file.</param>
        /// <returns>A tuple containing the quarantined file's path and its original location, or null if not found.</returns>
        public async Task<(string QuarantinedFilePath, string OriginalFilePath)?> GetQuarantinedFileByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query the database for the quarantined file information by ID
                    string query = "SELECT FilePath, OriginalFilePath FROM QuarantinedFiles WHERE Id = @Id";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (reader.GetString(0), reader.GetString(1));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving quarantined file: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Retrieves all quarantined files from the database.
        /// </summary>
        /// <returns>A list of all quarantined files, including their IDs, quarantined paths, and original paths.</returns>
        public async Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath, string QuarantineDate)>> GetAllQuarantinedFilesAsync()
        {
            var quarantinedFiles = new List<(int Id, string QuarantinedFilePath, string OriginalFilePath, string QuarantineDate)>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query the database for all quarantined files
                    string query = "SELECT Id, FilePath, OriginalFilePath, QuarantineDate FROM QuarantinedFiles";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string quarantinedFilePath = reader.GetString(1);
                                string originalFilePath = reader.GetString(2);
                                string quarantineDate = reader.GetString(3);

                                quarantinedFiles.Add((id, quarantinedFilePath, originalFilePath, quarantineDate));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving quarantined files: {ex.Message}");
            }

            return quarantinedFiles;
        }

        /// <summary>
        /// Retrieves original file path and date quarantined for all quarantined files from the database.
        /// </summary>
        /// <returns>A list of all quarantined files, including their original paths and date quarantined..</returns>
        public async Task<IEnumerable<(int Id, string OriginalFilePath, string QuarantineDate)>> GetQuarantinedFileDataAsync()
        {
            var quarantinedFiles = new List<(int Id, string OriginalFilePath, string QuarantineDate)>();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query the database for all quarantined files
                    string query = "SELECT Id, OriginalFilePath, QuarantineDate FROM QuarantinedFiles";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string originalFilePath = reader.GetString(1);
                                string quarantineDate = reader.GetString(2);

                                quarantinedFiles.Add((id, originalFilePath, quarantineDate));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving quarantined files: {ex.Message}");
            }

            return quarantinedFiles;
        }
    }
}
