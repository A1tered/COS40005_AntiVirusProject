using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;

public class AlertManager
{
    private readonly string _connectionString;

    public AlertManager()
    {
        _connectionString = "Data Source=alerts.db;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists("./alerts.db"))
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE Alerts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Component TEXT NOT NULL,
                        Severity TEXT NOT NULL,
                        Message TEXT NOT NULL,
                        SuggestedAction TEXT NOT NULL,
                        Timestamp TEXT NOT NULL
                    )";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public async Task StoreAlertAsync(Alert alert)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            string insertQuery = @"
                INSERT INTO Alerts (Component, Severity, Message, SuggestedAction, Timestamp)
                VALUES (@Component, @Severity, @Message, @SuggestedAction, @Timestamp)";

            using (var command = new SqliteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Component", alert.Component);
                command.Parameters.AddWithValue("@Severity", alert.Severity);
                command.Parameters.AddWithValue("@Message", alert.Message);
                command.Parameters.AddWithValue("@SuggestedAction", alert.SuggestedAction);
                command.Parameters.AddWithValue("@Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<List<Alert>> GetAllAlertsAsync()
    {
        var alerts = new List<Alert>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();

            string selectQuery = "SELECT * FROM Alerts";

            using (var command = new SqliteCommand(selectQuery, connection))
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
        }

        return alerts;
    }

    public async Task LogAndDisplayAlertAsync(Alert alert)
    {
        await StoreAlertAsync(alert);
        alert.DisplayAlert();
    }
}
