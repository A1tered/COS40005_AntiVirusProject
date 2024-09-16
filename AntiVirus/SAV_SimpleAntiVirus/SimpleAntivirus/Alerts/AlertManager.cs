using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace SimpleAntivirus.Alerts;
public class AlertManager
{
    protected SqliteConnection _databaseConnection;

    // Seconds (60 = 1 minute)
    // How long to keep track of the amount of violations
    private long _violationAmountTimeFrame;
    private long _violationAmountTimeTracker;
    // How long to supress violations.
    private long _violationTimeFrame;
    ConcurrentDictionary<string, int> _violationIncidents;
    ConcurrentDictionary<string, long> _aggregateViolationTimeSent;
    public AlertManager()
    {
        _violationAmountTimeFrame = 60;
        _violationTimeFrame = 10;
        _violationAmountTimeTracker = 0;
        _violationIncidents = new();
        _aggregateViolationTimeSent = new();
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

    private int CompareSecoundTime(long time)
    {
        return Convert.ToInt32((Environment.TickCount64 - time) / 1000);
    }

    public async Task LogAndDisplayAlertAsync(Alert alert)
    {
        await StoreAlertAsync(alert);
        // Code to check for too many alerts at once.

        // Restart time, for how long violations are kept.
        if (CompareSecoundTime(_violationAmountTimeTracker) > _violationAmountTimeFrame)
        {
            _violationIncidents.Clear();
            _violationAmountTimeFrame = Environment.TickCount64;
        }

        // Check if tracking amount of violations.
        if (_violationIncidents.ContainsKey(alert.Component))
        {
            _violationIncidents[alert.Component] += 1;

            // If within time frame, then we can send an alert, if not we must wait.
            if (CompareSecoundTime(_aggregateViolationTimeSent[alert.Component]) > _violationTimeFrame)
            {
                // This is where we can indicate a warning for multiple alerts.
                alert.DisplayAlert(true, _violationIncidents[alert.Component]);
                _aggregateViolationTimeSent[alert.Component] = Environment.TickCount64;
                // Erase values
            }

        }
        else
        {
            _aggregateViolationTimeSent[alert.Component] = 0;
            _violationIncidents[alert.Component] = 1;
            alert.DisplayAlert();
        }
    }
}
