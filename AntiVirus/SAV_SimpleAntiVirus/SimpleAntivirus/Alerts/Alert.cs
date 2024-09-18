/**************************************************************************
 * File:        Alert.cs
 * Author:      Zachary Smith, Joel Parks
 * Description: Creates the structure of an alert and displays it when called
 * Last Modified: 10/09/2024
 * NuGet Packages: Microsoft.Toolkit.Uwp.Notifications
 **************************************************************************/

using System;
using Microsoft.Toolkit.Uwp.Notifications;

/// <summary>
/// Contains the structure of an alert (Component, Severity, Message, SuggestedAction, TimeStamp) and sends a toast notification to display the alert when called.
/// </summary>

namespace SimpleAntivirus.Alerts;
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

    public void DisplayAlert(bool aggregateAlert = false, int aggregateAmount = 0)
    {
        if (!aggregateAlert)
        {
            new ToastContentBuilder()
                .AddCustomTimeStamp(Timestamp)
                .AddText($"Alert: {Component} [Severity: {Severity}]")
                .AddText(Message)
                .AddText($"Suggested Action: {SuggestedAction}")
                .Show();
            System.Diagnostics.Debug.WriteLine($"[ALERT - {Timestamp}] [Component: {Component}] [Severity: {Severity}]\nMessage: {Message}\nSuggested Action: {SuggestedAction}\n");
        }
        else
        {
             new ToastContentBuilder()
            .AddCustomTimeStamp(Timestamp)
            .AddText($"Alert: {Component} [Severity: {Severity}]")
            .AddText($"Message: There are {aggregateAmount - 1}+ new alerts for {Component}")
            .AddText("Suggested Action: Review protection history immediately!")
            .Show();
        }
    }
}