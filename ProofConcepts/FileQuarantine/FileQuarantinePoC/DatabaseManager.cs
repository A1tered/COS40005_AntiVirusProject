using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string databasePath)
    {
        // Ensure the directory for the database exists
        string databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!Directory.Exists(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
            Console.WriteLine($"Created directory for database at: {databaseDirectory}");
        }

        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open(); // Open the connection

                string createQuarantinedFilesTable = @"
                    CREATE TABLE IF NOT EXISTS QuarantinedFiles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FilePath TEXT NOT NULL,
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
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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

    public async Task StoreQuarantineInfoAsync(string filePath)
    {
        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string insertQuery = @"
                    INSERT INTO QuarantinedFiles (FilePath, QuarantineDate)
                    VALUES (@FilePath, @QuarantineDate)";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FilePath", filePath);
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

    public async Task<Dictionary<int, string>> PrintQuarantinedFilesAsync()
    {
        var quarantinedFiles = new Dictionary<int, string>();

        try
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM QuarantinedFiles";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("\nList of Quarantined Files:");
                        while (await reader.ReadAsync())
                        {
                            int id = reader.GetInt32(0);
                            string filePath = reader.GetString(1);
                            string quarantineDate = reader.GetString(2);

                            Console.WriteLine($"ID: {id}, File Path: {filePath}, Quarantine Date: {quarantineDate}");
                            quarantinedFiles[id] = filePath;
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
