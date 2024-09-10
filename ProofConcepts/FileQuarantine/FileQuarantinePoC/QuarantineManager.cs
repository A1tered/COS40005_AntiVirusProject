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
            Console.WriteLine($"Quarantine directory created at {_quarantineDirectory}");
        }
    }

    // Quarantine a file and remove its permissions
    public async Task QuarantineFileAsync(string filePath)
    {
        try
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
            await _databaseManager.StoreQuarantineInfoAsync(quarantinePath, filePath);

            // Log the file location securely
            LogQuarantinedFileLocation(quarantinePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during quarantine process: {ex.Message}");
        }
    }

    // Restore file permissions before unquarantining
    private void RestoreFilePermissionsUsingPowerShell(string filePath)
    {
        try
        {
            string command = $"icacls \"{filePath}\" /grant Everyone:F";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Permissions restored for {filePath} using PowerShell.");
            }
            else
            {
                Console.WriteLine($"Failed to restore permissions for {filePath}. Error: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error restoring file permissions: {ex.Message}");
        }
    }

    // Remove file permissions using PowerShell
    private void RemoveFilePermissionsUsingPowerShell(string filePath)
    {
        try
        {
            string command = $"icacls \"{filePath}\" /inheritance:r /remove:g \"Everyone\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Permissions removed from {filePath} using PowerShell.");
            }
            else
            {
                Console.WriteLine($"Failed to remove permissions from {filePath}. Error: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing file permissions: {ex.Message}");
        }
    }

    // Unquarantine the file and restore its permissions before moving it
    public async Task UnquarantineFileAsync(int id, string quarantinedFilePath, string originalFilePath)
    {
        try
        {
            // Restore the file permissions before attempting to move it
            RestoreFilePermissionsUsingPowerShell(quarantinedFilePath);

            // Move the file back to its original location
            if (File.Exists(quarantinedFilePath))
            {
                // Ensure the directory for the original location exists
                string originalDirectory = Path.GetDirectoryName(originalFilePath);
                if (!Directory.Exists(originalDirectory))
                {
                    Directory.CreateDirectory(originalDirectory);
                    Console.WriteLine($"Created directory: {originalDirectory}");
                }

                File.Move(quarantinedFilePath, originalFilePath);
                Console.WriteLine($"File unquarantined and moved back to: {originalFilePath}");

                // Remove the quarantine entry from the database
                await _databaseManager.RemoveQuarantineEntryAsync(id);
            }
            else
            {
                Console.WriteLine($"File not found in quarantine: {quarantinedFilePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during unquarantine process: {ex.Message}");
        }
    }

    private void LogQuarantinedFileLocation(string filePath)
    {
        try
        {
            string logFilePath = Path.Combine(_quarantineDirectory, "quarantine_log.txt");
            string logEntry = $"[{DateTime.Now}] Quarantined file located at: {filePath}";

            // Log the file location securely
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            Console.WriteLine("Quarantined file location logged securely.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging quarantined file location: {ex.Message}");
        }
    }
}
