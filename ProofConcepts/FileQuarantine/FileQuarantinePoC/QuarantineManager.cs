using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class QuarantineManager
{
    private readonly FileMover _fileMover;
    private readonly DatabaseManager _databaseManager;
    private readonly string _quarantineDirectory;

    public QuarantineManager(FileMover fileMover, DatabaseManager databaseManager, string quarantineDirectory)
    {
        _fileMover = fileMover;
        _databaseManager = databaseManager;
        _quarantineDirectory = quarantineDirectory;

        // Ensure quarantine directory exists
        if (!Directory.Exists(_quarantineDirectory))
        {
            Directory.CreateDirectory(_quarantineDirectory);
        }
    }

    public async Task QuarantineFileAsync(string filePath)
    {
        // Check if the file is already whitelisted
        if (await _databaseManager.IsWhitelistedAsync(filePath))
        {
            Console.WriteLine("File is whitelisted and will not be quarantined.");
            return;
        }

        // Move the file to the quarantine directory
        string quarantinePath = await _fileMover.MoveFileToQuarantineAsync(filePath, _quarantineDirectory);

        // Remove file permissions using PowerShell
        RemoveFilePermissionsUsingPowerShell(quarantinePath);

        // Store quarantine information in the database
        await _databaseManager.StoreQuarantineInfoAsync(quarantinePath);
    }

    private void RemoveFilePermissionsUsingPowerShell(string filePath)
    {
        string command = $"icacls \"{filePath}\" /inheritance:r /remove:g \"Everyone\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine($"Permissions removed from {filePath} using PowerShell.");
        }
        else
        {
            Console.WriteLine($"Failed to remove permissions from {filePath}. Error: {process.StandardOutput.ReadToEnd()}");
        }
    }
}
