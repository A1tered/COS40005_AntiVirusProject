using System.Threading.Tasks;

public class EventBus
{
    private readonly AlertManager _alertManager;

    public EventBus(AlertManager alertManager)
    {
        _alertManager = alertManager;
    }

    public async Task PublishAsync(string component, string message)
    {
        var alert = new Alert(component, message);
        await _alertManager.LogAndDisplayAlertAsync(alert);
    }
}
