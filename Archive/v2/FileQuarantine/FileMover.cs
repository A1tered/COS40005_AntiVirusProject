using System;
using System.IO;
using System.Threading.Tasks;

public class FileMover
{
    // Moves a file from its original location to the quarantine directory
    public async Task<string> MoveFileToQuarantineAsync(string sourcePath, string quarantineDirectory)
    {
        try
        {
            string fileName = Path.GetFileName(sourcePath);
            string quarantinePath = Path.Combine(quarantineDirectory, fileName);

            // If the file already exists in quarantine, notify the user
            if (File.Exists(quarantinePath))
            {
                Console.WriteLine("File already exists in quarantine. Overwriting...");
            }

            // Ensure the quarantine directory exists before moving the file
            if (!Directory.Exists(quarantineDirectory))
            {
                Directory.CreateDirectory(quarantineDirectory);
                Console.WriteLine($"Quarantine directory created at {quarantineDirectory}");
            }

            // Move the file asynchronously to the quarantine directory
            await Task.Run(() => File.Move(sourcePath, quarantinePath, overwrite: true));
            Console.WriteLine($"File moved to quarantine: {quarantinePath}");

            return quarantinePath;
        }
        catch (Exception ex)
        {
            // Handle any errors during file moving
            Console.WriteLine($"Error moving file to quarantine: {ex.Message}");
            throw;
        }
    }
}
