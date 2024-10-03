using System.Threading.Tasks;

public class IntegrityCheckerStub
{
    private readonly EventBus _eventBus;

    public IntegrityCheckerStub(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task CheckFileIntegrityAsync()
    {
        // Simulate a file integrity check failure
        await Task.Delay(1000); // Simulate some processing time
        await _eventBus.PublishAsync("IntegrityChecker", "High", "File integrity check failed for file ABC.", "Restore the file from backup or contact support.");
    }
}