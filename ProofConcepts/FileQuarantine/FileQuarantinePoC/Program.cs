using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Use a writable directory for testing, such as the user's Documents folder
        string quarantineDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quarantine");
        string databasePath = Path.Combine(quarantineDirectory, "quarantine.db");

        FileMover fileMover = new FileMover();
        DatabaseManager databaseManager = new DatabaseManager(databasePath);
        QuarantineManager quarantineManager = new QuarantineManager(fileMover, databaseManager, quarantineDirectory);

        // Path to the suspicious file (provided externally)
        string filePathToQuarantine = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile.txt");

        try
        {
            // Ensure the directory for the suspicious file exists
            string suspiciousFileDirectory = Path.GetDirectoryName(filePathToQuarantine);
            if (!Directory.Exists(suspiciousFileDirectory))
            {
                Directory.CreateDirectory(suspiciousFileDirectory);
                Console.WriteLine($"Created directory: {suspiciousFileDirectory}");
            }

            // Check if the file exists; if not, create it
            if (!File.Exists(filePathToQuarantine))
            {
                // Create a test file for demonstration purposes
                await File.WriteAllTextAsync(filePathToQuarantine, "This is a suspicious file.");
                Console.WriteLine($"Test file created at {filePathToQuarantine}");
            }
            else
            {
                Console.WriteLine($"Test file already exists at {filePathToQuarantine}");
            }

            // Simulate another functionality (MaliciousCodeScanner) detecting a dangerous file
            MaliciousCodeScannerStub scannerStub = new MaliciousCodeScannerStub(quarantineManager);
            await scannerStub.ScanAndQuarantineAsync(filePathToQuarantine);

            // Output the stored data (quarantined files) to the command line
            var quarantinedFiles = await databaseManager.PrintQuarantinedFilesAsync();

            // Ask user for ID of file to unquarantine
            Console.WriteLine("\nEnter the ID of the file you want to unquarantine:");
            if (int.TryParse(Console.ReadLine(), out int id) && quarantinedFiles.ContainsKey(id))
            {
                // Prompt the user for the original location of the file (to restore it)
                Console.WriteLine("Enter the original location where the file should be restored:");
                string originalLocation = Console.ReadLine();

                // Unquarantine the selected file
                await quarantineManager.UnquarantineFileAsync(id, quarantinedFiles[id], originalLocation);

                // Output the updated list of quarantined files
                await databaseManager.PrintQuarantinedFilesAsync();
            }
            else
            {
                Console.WriteLine("Invalid ID entered.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
