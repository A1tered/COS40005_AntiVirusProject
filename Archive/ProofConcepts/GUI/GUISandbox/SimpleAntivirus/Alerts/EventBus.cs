using System.Threading.Tasks;

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
