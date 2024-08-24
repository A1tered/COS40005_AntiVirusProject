using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

public class AlertManager
{
    private readonly string _connectionString;

    public AlertManager()
    {
        _connectionString = "Data Source=alerts.db;Version=3;";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists("./alerts.db"))
        {
            SQLiteConnection.CreateFile("alerts.db");

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE Alerts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Component TEXT NOT NULL,
                        Message TEXT NOT NULL,
                        Timestamp TEXT NOT NULL
                    )";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public async Task StoreAlertAsync(Alert alert)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            string insertQuery = @"
                INSERT INTO Alerts (Component, Message, Timestamp)
                VALUES (@Component, @Message, @Timestamp)";

            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Component", alert.Component);
                command.Parameters.AddWithValue("@Message", alert.Message);
                command.Parameters.AddWithValue("@Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public async Task<List<Alert>> GetAllAlertsAsync()
    {
        var alerts = new List<Alert>();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            string selectQuery = "SELECT * FROM Alerts";

            using (var command = new SQLiteCommand(selectQuery, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var alert = new Alert(
                            reader["Component"].ToString(),
                            reader["Message"].ToString())
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
