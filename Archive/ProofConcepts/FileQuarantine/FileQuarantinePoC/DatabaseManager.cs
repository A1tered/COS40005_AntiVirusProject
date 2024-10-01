using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

public class DatabaseManager : IDatabaseManager
{
    private readonly string _connectionString;

    // Constructor initializes the database connection string and ensures tables are created
    public DatabaseManager(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    // Ensures the quarantine and whitelist tables are created in the database
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

                Console.WriteLine("Database initialized successfully.");
            }
        }
        catch (Exception ex)
        {
            // Handle any database initialization errors
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }

    // Checks if a file is whitelisted by querying the database
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
            // Handle errors while checking whitelist status
            Console.WriteLine($"Error checking whitelist status: {ex.Message}");
            return false;
        }
    }

    // Adds a file to the whitelist by inserting its path into the database
    public async Task AddToWhitelistAsync(string filePath)
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

                Console.WriteLine($"Added to whitelist: {filePath}");
            }
        }
        catch (Exception ex)
        {
            // Handle errors while adding to the whitelist
            Console.WriteLine($"Error adding to whitelist: {ex.Message}");
        }
    }

    // Removes a file from the whitelist by deleting its path from the database
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

                Console.WriteLine($"Removed from whitelist: {filePath}");
            }
        }
        catch (Exception ex)
        {
            // Handle errors while removing from the whitelist
            Console.WriteLine($"Error removing from whitelist: {ex.Message}");
        }
    }

    // Retrieves all whitelisted files from the database
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
            // Handle errors while retrieving whitelist
            Console.WriteLine($"Error retrieving whitelist: {ex.Message}");
        }

        return whitelist;
    }

    // Stores information about a quarantined file in the database
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

                Console.WriteLine("Quarantine information stored in the database.");
            }
        }
        catch (Exception ex)
        {
            // Handle errors while storing quarantine information
            Console.WriteLine($"Error storing quarantine information: {ex.Message}");
        }
    }

    // Removes a quarantine entry from the database by its ID
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

                Console.WriteLine($"Quarantine entry with ID {id} has been removed from the database.");
            }
        }
        catch (Exception ex)
        {
            // Handle errors while removing quarantine entry
            Console.WriteLine($"Error removing quarantine entry: {ex.Message}");
        }
    }

    // Retrieves a quarantined file by its ID from the database
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
            // Handle errors while retrieving quarantined file information
            Console.WriteLine($"Error retrieving quarantined file: {ex.Message}");
        }

        return null;
    }

    // Retrieves all quarantined files from the database
    public async Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath)>> GetAllQuarantinedFilesAsync()
    {
        var quarantinedFiles = new List<(int Id, string QuarantinedFilePath, string OriginalFilePath)>();

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Query the database for all quarantined files
                string query = "SELECT Id, FilePath, OriginalFilePath FROM QuarantinedFiles";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string quarantinedFilePath = reader.GetString(1);
                            string originalFilePath = reader.GetString(2);

                            quarantinedFiles.Add((id, quarantinedFilePath, originalFilePath));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle errors while retrieving all quarantined files
            Console.WriteLine($"Error retrieving quarantined files: {ex.Message}");
        }

        return quarantinedFiles;
    }
}
