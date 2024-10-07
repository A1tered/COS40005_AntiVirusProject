/**************************************************************************
* File:        EventBus.cs
* Author:      Zachary Smith
* Description: Handles the bus to which functionalities can call to push alerts.
* Last Modified: 8/10/2024
**************************************************************************/
namespace SimpleAntivirus.Alerts;
public class EventBus
{
    private readonly AlertManager _alertManager;

    public EventBus(AlertManager alertManager)
    {
        _alertManager = alertManager;
    }

    public async Task PublishAsync(string component, string severity, string message, string suggestedAction)
    {
        var alert = new Alert(component, severity, message, suggestedAction);
        await _alertManager.LogAndDisplayAlertAsync(alert);
    }
}
