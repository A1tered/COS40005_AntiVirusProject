using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    // Entry point of the application
    static async Task Main(string[] args)
    {
        // Define directories and paths
        // "quarantineDirectory" is where quarantined files are stored
        string quarantineDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quarantine");
        // "databasePath" defines the path to the SQLite database that will store file information
        string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "quarantine.db");

        // Initialize managers for file operations, database handling, and quarantining
        FileMover fileMover = new FileMover();
        IDatabaseManager databaseManager = new DatabaseManager(databasePath);
        IQuarantineManager quarantineManager = new QuarantineManager(fileMover, databaseManager, quarantineDirectory);

        // Whitelist management: Allow the user to add, remove, or list files from the whitelist
        Console.WriteLine("Do you want to add or remove files from the whitelist? (add/remove/list/skip)");
        string action = Console.ReadLine()?.ToLower();

        if (action == "add")
        {
            // Add a file to the whitelist
            Console.WriteLine("Enter the path to whitelist:");
            string path = Console.ReadLine();
            await databaseManager.AddToWhitelistAsync(path);
        }
        else if (action == "remove")
        {
            // Remove a file from the whitelist
            Console.WriteLine("Enter the path to remove from whitelist:");
            string path = Console.ReadLine();
            await databaseManager.RemoveFromWhitelistAsync(path);
        }
        else if (action == "list")
        {
            // List all whitelisted files
            var whitelist = await databaseManager.GetWhitelistAsync();
            Console.WriteLine("Whitelisted files and folders:");
            foreach (var file in whitelist)
            {
                Console.WriteLine(file);
            }
        }

        // Define a list of files that need to be quarantined
        var filesToQuarantine = new List<string>
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile1.txt"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path", "To", "SuspiciousFile2.txt"),
        };

        // Start quarantining files concurrently by tracking each task
        var quarantineTasks = new List<Task>();

        foreach (var filePath in filesToQuarantine)
        {
            // For each file, initiate the quarantine process asynchronously
            quarantineTasks.Add(QuarantineFileWithCheck(filePath, quarantineManager));
        }

        // Wait for all quarantine tasks to complete before continuing
        try
        {
            await Task.WhenAll(quarantineTasks);
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the quarantine process
            Console.WriteLine($"Error occurred during quarantine: {ex.Message}");
        }

        // Output all quarantined files after processing
        var quarantinedFiles = await quarantineManager.GetQuarantinedFilesAsync();
        Console.WriteLine("\nQuarantined Files:");
        foreach (var file in quarantinedFiles)
        {
            Console.WriteLine($"ID: {file.Id}, Quarantined File: {file.QuarantinedFilePath}, Original Location: {file.OriginalFilePath}");
        }

        // Prompt the user to unquarantine a file by entering its ID
        Console.WriteLine("\nEnter the ID of the file you want to unquarantine:");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            // Unquarantine the file with the given ID
            await quarantineManager.UnquarantineFileAsync(id);
        }
        else
        {
            Console.WriteLine("Invalid ID entered.");
        }
    }

    // QuarantineFileWithCheck method ensures the file exists before attempting to quarantine it
    static async Task QuarantineFileWithCheck(string filePath, IQuarantineManager quarantineManager)
    {
        try
        {
            // Check if the file exists; if not, create it
            if (!File.Exists(filePath))
            {
                // Ensure the directory for the file exists
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Directory created at {directory}");
                }

                // Create a new file with sample content to simulate a suspicious file
                await File.WriteAllTextAsync(filePath, "This is a suspicious file.");
                Console.WriteLine($"Test file created at {filePath}");
            }

            // Quarantine the file by moving it and updating the database
            await quarantineManager.QuarantineFileAsync(filePath);
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during file checking or quarantining
            Console.WriteLine($"Error quarantining file {filePath}: {ex.Message}");
        }
    }
}
