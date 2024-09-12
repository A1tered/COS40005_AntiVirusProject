using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

public class DatabaseManager : IDatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string createQuarantinedFilesTable = @"
                    CREATE TABLE IF NOT EXISTS QuarantinedFiles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FilePath TEXT NOT NULL,
                        OriginalFilePath TEXT NOT NULL,
                        QuarantineDate TEXT NOT NULL
                    );";

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
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }

    public async Task<bool> IsWhitelistedAsync(string filePath)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error checking whitelist status: {ex.Message}");
            return false;
        }
    }

    public async Task AddToWhitelistAsync(string filePath)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error adding to whitelist: {ex.Message}");
        }
    }

    public async Task RemoveFromWhitelistAsync(string filePath)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error removing from whitelist: {ex.Message}");
        }
    }

    public async Task<IEnumerable<string>> GetWhitelistAsync()
    {
        var whitelist = new List<string>();

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error retrieving whitelist: {ex.Message}");
        }

        return whitelist;
    }

    public async Task StoreQuarantineInfoAsync(string quarantinedFilePath, string originalFilePath)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error storing quarantine information: {ex.Message}");
        }
    }

    public async Task RemoveQuarantineEntryAsync(int id)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error removing quarantine entry: {ex.Message}");
        }
    }

    public async Task<(string QuarantinedFilePath, string OriginalFilePath)?> GetQuarantinedFileByIdAsync(int id)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error retrieving quarantined file: {ex.Message}");
        }

        return null;
    }

    public async Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath)>> GetAllQuarantinedFilesAsync()
    {
        var quarantinedFiles = new List<(int Id, string QuarantinedFilePath, string OriginalFilePath)>();

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

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
            Console.WriteLine($"Error retrieving quarantined files: {ex.Message}");
        }

        return quarantinedFiles;
    }
}
