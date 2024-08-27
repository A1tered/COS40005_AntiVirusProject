using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

public class AlertManager
{
    protected SqliteConnection _databaseConnection;

    public AlertManager()
    {
        string databasePath = "alerts.db"; // Specify the database path
        _databaseConnection = new SqliteConnection($"Data Source={databasePath}"); // Use Data Source for the SQLite connection string
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        _databaseConnection.Open();

        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Alerts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Component TEXT NOT NULL,
                Severity TEXT NOT NULL,
                Message TEXT NOT NULL,
                SuggestedAction TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            )";

        using (var command = new SqliteCommand(createTableQuery, _databaseConnection))
        {
            command.ExecuteNonQuery();
        }

        _databaseConnection.Close();
    }

    public async Task StoreAlertAsync(Alert alert)
    {
        await _databaseConnection.OpenAsync();

        string insertQuery = @"
            INSERT INTO Alerts (Component, Severity, Message, SuggestedAction, Timestamp)
            VALUES (@Component, @Severity, @Message, @SuggestedAction, @Timestamp)";

        using (var command = new SqliteCommand(insertQuery, _databaseConnection))
        {
            command.Parameters.AddWithValue("@Component", alert.Component);
            command.Parameters.AddWithValue("@Severity", alert.Severity);
            command.Parameters.AddWithValue("@Message", alert.Message);
            command.Parameters.AddWithValue("@SuggestedAction", alert.SuggestedAction);
            command.Parameters.AddWithValue("@Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }

        _databaseConnection.Close();
    }

    public async Task<List<Alert>> GetAllAlertsAsync()
    {
        var alerts = new List<Alert>();

        await _databaseConnection.OpenAsync();

        string selectQuery = "SELECT * FROM Alerts";

        using (var command = new SqliteCommand(selectQuery, _databaseConnection))
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var alert = new Alert(
                        reader["Component"].ToString(),
                        reader["Severity"].ToString(),
                        reader["Message"].ToString(),
                        reader["SuggestedAction"].ToString())
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Timestamp = DateTime.Parse(reader["Timestamp"].ToString())
                    };

                    alerts.Add(alert);
                }
            }
        }

        _databaseConnection.Close();

        return alerts;
    }

    public async Task LogAndDisplayAlertAsync(Alert alert)
    {
        await StoreAlertAsync(alert);
        alert.DisplayAlert();
    }
}
