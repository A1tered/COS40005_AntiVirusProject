using System;
using System.Threading.Tasks;

// Simulates a component that interacts with the quarantine manager
public class MaliciousCodeScannerStub
{
    private readonly IQuarantineManager _quarantineManager;

    /// <summary>
    /// Constructor for the MaliciousCodeScannerStub. Requires an instance of IQuarantineManager to quarantine detected files.
    /// </summary>
    /// <param name="quarantineManager">The quarantine manager to handle detected files.</param>
    public MaliciousCodeScannerStub(IQuarantineManager quarantineManager)
    {
        _quarantineManager = quarantineManager;
    }

    /// <summary>
    /// Simulates scanning a file and sending it to quarantine if it is flagged as malicious.
    /// Other components should replace this stub with their own scanning logic.
    /// </summary>
    /// <param name="filePath">The full path of the file to scan.</param>
    /// <returns>An asynchronous task that completes when the file is quarantined.</returns>
    public async Task ScanAndQuarantineAsync(string filePath)
    {
        // Simulate detecting a malicious file
        Console.WriteLine($"MaliciousCodeScanner: Detected a potentially dangerous file at {filePath}");

        // Send the file to the QuarantineManager for quarantining
        await _quarantineManager.QuarantineFileAsync(filePath);
    }
}
