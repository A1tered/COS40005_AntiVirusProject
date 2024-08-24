using System;

public class Alert
{
    public int Id { get; set; } // Auto-incremented ID for the database
    public string Component { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }

    public Alert(string component, string message)
    {
        Component = component;
        Message = message;
        Timestamp = DateTime.Now;
    }

    public void DisplayAlert()
    {
        Console.WriteLine($"[ALERT - {Timestamp}] [Component: {Component}]\nMessage: {Message}\n");
    }
}
