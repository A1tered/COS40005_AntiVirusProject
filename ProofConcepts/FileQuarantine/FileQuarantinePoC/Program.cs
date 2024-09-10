using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Define directories and paths
        string quarantineDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quarantine");
        string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db");

        // Initialize managers
        FileMover fileMover = new FileMover();
        IDatabaseManager databaseManager = new DatabaseManager(databasePath);
        IQuarantineManager quarantineManager = new QuarantineManager(fileMover, databaseManager, quarantineDirectory);

        // Create the MaliciousCodeScannerStub and scan for files
        MaliciousCodeScannerStub scannerStub = new MaliciousCodeScannerStub(quarantineManager);

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

            // Simulate scanning and quarantining the detected file
            await scannerStub.ScanAndQuarantineAsync(filePathToQuarantine);

            // Output the stored data (quarantined files) to the command line
<<<<<<< Updated upstream
            var quarantinedFiles = await databaseManager.PrintQuarantinedFilesAsync();

            // Ask user for ID of file to unquarantine
            Console.WriteLine("\nEnter the ID of the file you want to unquarantine:");
            if (int.TryParse(Console.ReadLine(), out int id) && quarantinedFiles.ContainsKey(id))
            {
                // Unquarantine the selected file using the stored original location
                var fileData = quarantinedFiles[id];
                await quarantineManager.UnquarantineFileAsync(id, fileData.QuarantinedFilePath, fileData.OriginalFilePath);

                // Output the updated list of quarantined files
                await databaseManager.PrintQuarantinedFilesAsync();
=======
            var quarantinedFiles = await quarantineManager.GetQuarantinedFilesAsync();
            Console.WriteLine("\nQuarantined Files:");
            foreach (var file in quarantinedFiles)
            {
                Console.WriteLine($"ID: {file.Id}, Quarantined File: {file.QuarantinedFilePath}, Original Location: {file.OriginalFilePath}");
            }

            // Prompt to unquarantine a file
            Console.WriteLine("\nEnter the ID of the file you want to unquarantine:");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                await quarantineManager.UnquarantineFileAsync(id);
>>>>>>> Stashed changes
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
