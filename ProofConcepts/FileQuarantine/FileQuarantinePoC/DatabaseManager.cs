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

                // Commented out the drop table command to preserve data
                // string dropQuarantinedFilesTable = "DROP TABLE IF EXISTS QuarantinedFiles";

                // Updated table schema to include OriginalFilePath
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

                // Ensure the QuarantinedFiles table has the correct schema
                // If the table doesn't exist, it will be created with the correct schema
                // If it already exists but doesn't have the OriginalFilePath column, it needs to be altered

                // Adding the column if it doesn't exist
                try
                {
                    string checkForOriginalFilePath = @"
                        PRAGMA table_info(QuarantinedFiles);";

                    bool columnExists = false;

                    using (var command = new SqliteCommand(checkForOriginalFilePath, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader.GetString(1); // The second column is the name
                                if (columnName == "OriginalFilePath")
                                {
                                    columnExists = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!columnExists)
                    {
                        // Add the OriginalFilePath column if it doesn't exist
                        string addOriginalFilePathColumn = @"
                            ALTER TABLE QuarantinedFiles ADD COLUMN OriginalFilePath TEXT";

                        using (var command = new SqliteCommand(addOriginalFilePathColumn, connection))
                        {
                            command.ExecuteNonQuery();
                            Console.WriteLine("OriginalFilePath column added to QuarantinedFiles table.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error adding OriginalFilePath column: {e.Message}");
                }

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

    public async Task<Dictionary<int, (string QuarantinedFilePath, string OriginalFilePath)>> PrintQuarantinedFilesAsync()
    {
        var quarantinedFiles = new Dictionary<int, (string QuarantinedFilePath, string OriginalFilePath)>();

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
                            string quarantinedFilePath = reader.GetString(1);
                            string originalFilePath = reader.GetString(2);
                            string quarantineDate = reader.GetString(3);

                            Console.WriteLine($"ID: {id}, Quarantined File: {quarantinedFilePath}, Original File: {originalFilePath}, Quarantine Date: {quarantineDate}");
                            quarantinedFiles[id] = (quarantinedFilePath, originalFilePath);
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
