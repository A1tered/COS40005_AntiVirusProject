using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string quarantineDirectory = "C:\\Quarantine";
        string databasePath = "quarantine.db";

        FileMover fileMover = new FileMover();
        DatabaseManager databaseManager = new DatabaseManager(databasePath);
        QuarantineManager quarantineManager = new QuarantineManager(fileMover, databaseManager, quarantineDirectory);

        // Simulate another functionality (MaliciousCodeScanner) detecting a dangerous file
        MaliciousCodeScannerStub scannerStub = new MaliciousCodeScannerStub(quarantineManager);

        string filePathToQuarantine = "C:\\Path\\To\\SuspiciousFile.txt";

        await scannerStub.ScanAndQuarantineAsync(filePathToQuarantine);
    }
}