/**************************************************************************
* File:        AlertManager.cs
* Author:      Zachary Smith & (minor changes by Christopher)
* Description: Handles requests for alerts, and storage of alerts.
* Last Modified: 8/10/2024
**************************************************************************/

using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;
using System.IO;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;

namespace SimpleAntivirus.Alerts;
public class AlertManager
{
    protected SqliteConnection _databaseConnection;

    // Intended to raise alert, to notify window to update.
    public event EventHandler NewAlert = delegate { };

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
        ISetupService setupService = SetupService.GetExistingInstance();
        _violationAmountTimeFrame = 60;
        _violationTimeFrame = 10;
        _violationAmountTimeTracker = 0;
        _violationIncidents = new();
        _aggregateViolationTimeSent = new();
        string databasePath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases"), "alerts.db"); // Specify the database path
        _databaseConnection = new SqliteConnection($"Data Source={databasePath};Password={setupService.DbKey()}"); // Use Data Source for the SQLite connection string
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
    }

    public async Task ClearDatabase()
    {
        using (SqliteCommand sqliteCommand = new SqliteCommand("DELETE From Alerts", _databaseConnection))
        {
            await sqliteCommand.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Fun little function, that gets all the alerts that occured within a second timeframe, eg 60, would get the count of all alerts
    /// sent in the past minute.
    /// </summary>
    /// <param name="timeframeGapSeconds"></param>
    /// <returns></returns>
    public async Task<int> GetAlertsWithinPastTimeFrame(int timeframeGapSeconds = 120)
    {
        int amountAlert = 0;
        using (SqliteCommand command = new SqliteCommand("SELECT * FROM Alerts", _databaseConnection))
        {
            using (SqliteDataReader dataReader = await command.ExecuteReaderAsync())
            {
                while (await dataReader.ReadAsync())
                {
                    //DEBUG: System.Diagnostics.Debug.WriteLine($"Reformat and Display: {DateTime.Parse(dataReader["Timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")}");
                    DateTime compareFromDatabase = DateTime.Parse(dataReader["Timestamp"].ToString());
                    long alertTime = Convert.ToInt64(new TimeSpan(compareFromDatabase.Ticks).TotalSeconds);
                    long currentTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                    // If there is an alert that is identical in the database that has already been sent within the timeframe gap, send out.
                    if (currentTime - alertTime < timeframeGapSeconds)
                    {
                        amountAlert += 1;
                    }
                }
            }

        }
        return amountAlert;
    }

    /// <summary>
    /// This checks if the alert already exists in database within a time frame, may help prevent identical alerts being sent
    /// </summary>
    /// <param name="message">Alert object (utilises message and timeframe)</param>
    /// <param name="timeframeGapSeconds">Only check within a timeframe of seconds, alerts after this gap won't be considered
    /// as identical.</param>
    /// <remarks>This was necessary, as everytime reactive control was triggered it would send a batch of all violations<para/>
    /// eg, if there were 10 violations, then every time u changed 1 file, another 10 violations would be added to the database.</remarks>
    /// <returns>True (Identical alerts found in timeframe) <para/> False (No identical alerts found in timeframe)</returns>
    public async Task<bool> CheckForIdenticalAlertInTimeFrame(Alert alertItem, int timeframeGapSeconds = 120)
    {
        using (SqliteCommand command = new SqliteCommand("SELECT * FROM Alerts WHERE Message = $whereStatement", _databaseConnection))
        {
            command.Parameters.AddWithValue("$whereStatement", alertItem.Message);
            using (SqliteDataReader dataReader = await command.ExecuteReaderAsync())
            {
                while (await dataReader.ReadAsync())
                {
                    //DEBUG: System.Diagnostics.Debug.WriteLine($"Reformat and Display: {DateTime.Parse(dataReader["Timestamp"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")}");
                    DateTime compareFromDatabase = DateTime.Parse(dataReader["Timestamp"].ToString());
                    TimeSpan tickDifferenceObject = new(alertItem.Timestamp.Ticks - compareFromDatabase.Ticks);
                    // If there is an alert that is identical in the database that has already been sent within the timeframe gap, send out.
                    if (tickDifferenceObject.TotalSeconds < timeframeGapSeconds)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    public async Task StoreAlertAsync(Alert alert)
    {
        bool cancelAlert = false;
        // Check whether the an identical alert has already been sent in the last minute (Only for integrity checking
        if (alert.Component == "Integrity Checking")
        {
            cancelAlert = await CheckForIdenticalAlertInTimeFrame(alert);
        }
        //await _databaseConnection.OpenAsync();

        // If alert has not be cancelled then...
        if (cancelAlert == false) {
            NewAlert.Invoke(this, new EventArgs());
        string insertQuery = @"
            INSERT INTO Alerts (Component, Severity, Message, SuggestedAction, Timestamp)
            VALUES (@Component, @Severity, @Message, @SuggestedAction, @Timestamp)";

            var command = new SqliteCommand(insertQuery, _databaseConnection);
            command.Parameters.AddWithValue("@Component", alert.Component);
            command.Parameters.AddWithValue("@Severity", alert.Severity);
            command.Parameters.AddWithValue("@Message", alert.Message);
            command.Parameters.AddWithValue("@SuggestedAction", alert.SuggestedAction);
            command.Parameters.AddWithValue("@Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }

       // _databaseConnection.Close();
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
                // Erase values
                _aggregateViolationTimeSent[alert.Component] = Environment.TickCount64;
                // This is where we can indicate a warning for multiple alerts.
                alert.DisplayAlert(true, _violationIncidents[alert.Component]);
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
