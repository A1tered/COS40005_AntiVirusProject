using System;

namespace AlertHandler;
public class Alert
{
    public int Id { get; set; } // Auto-incremented ID for the database
    public string Component { get; set; }
    public string Severity { get; set; } // New property for severity
    public string Message { get; set; }
    public string SuggestedAction { get; set; } // New property for suggested action
    public DateTime Timestamp { get; set; }

    public Alert(string component, string severity, string message, string suggestedAction)
    {
        Component = component;
        Severity = severity;
        Message = message;
        SuggestedAction = suggestedAction;
        Timestamp = DateTime.Now;
    }

    public void DisplayAlert()
    {
        Console.WriteLine($"[ALERT - {Timestamp}] [Component: {Component}] [Severity: {Severity}]\nMessage: {Message}\nSuggested Action: {SuggestedAction}\n");
    }
}
