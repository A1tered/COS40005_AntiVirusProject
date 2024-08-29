using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

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
        }
    }

    public async Task<bool> IsWhitelistedAsync(string filePath)
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

    public async Task StoreQuarantineInfoAsync(string filePath)
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
        }
    }
}
