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

        // Whitelist management
        Console.WriteLine("Do you want to add or remove files from the whitelist? (add/remove/list/skip)");
        string action = Console.ReadLine()?.ToLower();

        if (action == "add")
        {
            Console.WriteLine("Enter the path to whitelist:");
            string path = Console.ReadLine();
            await databaseManager.AddToWhitelistAsync(path);
        }
        else if (action == "remove")
        {
            Console.WriteLine("Enter the path to remove from whitelist:");
            string path = Console.ReadLine();
            await databaseManager.RemoveFromWhitelistAsync(path);
        }
        else if (action == "list")
        {
            var whitelist = await databaseManager.GetWhitelistAsync();
            Console.WriteLine("Whitelisted files and folders:");
            foreach (var file in whitelist)
            {
                Console.WriteLine(file);
            }
        }

        // Path to the suspicious file (provided externally)
        string filePathToQuarantine = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile.txt");

        // Check if the file exists, and if not, create it
        if (!File.Exists(filePathToQuarantine))
        {
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filePathToQuarantine);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Directory created at {directory}");
            }

            // Create the file
            await File.WriteAllTextAsync(filePathToQuarantine, "This is a suspicious file.");
            Console.WriteLine($"Test file created at {filePathToQuarantine}");
        }

        // Simulate scanning and quarantining the detected file
        await quarantineManager.QuarantineFileAsync(filePathToQuarantine);

        // Output the stored data (quarantined files) to the command line
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
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
        }
    }
}
