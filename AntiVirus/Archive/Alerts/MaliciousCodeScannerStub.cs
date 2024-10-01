using System.Threading.Tasks;
namespace AlertHandler;
public class MaliciousCodeScannerStub
{
    private readonly EventBus _eventBus;

    public MaliciousCodeScannerStub(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task ScanForMaliciousCodeAsync()
    {
        // Simulate detecting malicious code
        await Task.Delay(1000); // Simulate some processing time
        await _eventBus.PublishAsync("MaliciousCodeScanner", "Critical", "Malicious code detected in file XYZ.", "Remove the file immediately.");
    }
}