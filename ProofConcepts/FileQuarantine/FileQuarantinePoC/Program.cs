using System;
using System.Collections.Generic;
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

        // Whitelist management (handled sequentially, can be skipped)
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

        // List of files to quarantine (simulating multiple flagged files)
        var filesToQuarantine = new List<string>
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile1.txt"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile2.txt"),
        };

        // Start quarantining files in parallel and track the tasks
        var quarantineTasks = new List<Task>();

        foreach (var filePath in filesToQuarantine)
        {
            quarantineTasks.Add(QuarantineFileWithCheck(filePath, quarantineManager));
        }

        // Wait for all quarantine tasks to complete
        try
        {
            await Task.WhenAll(quarantineTasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during quarantine: {ex.Message}");
        }

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

    // Function to check if a file exists and quarantine it
    static async Task QuarantineFileWithCheck(string filePath, IQuarantineManager quarantineManager)
    {
        try
        {
            // Check if the file exists, and if not, create it
            if (!File.Exists(filePath))
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Directory created at {directory}");
                }

                // Create the file
                await File.WriteAllTextAsync(filePath, "This is a suspicious file.");
                Console.WriteLine($"Test file created at {filePath}");
            }

            // Quarantine the file
            await quarantineManager.QuarantineFileAsync(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error quarantining file {filePath}: {ex.Message}");
        }
    }
}
