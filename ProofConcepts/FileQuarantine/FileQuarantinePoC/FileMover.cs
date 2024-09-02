using System;
using System.IO;
using System.Threading.Tasks;

public class FileMover
{
    public async Task<string> MoveFileToQuarantineAsync(string sourcePath, string quarantineDirectory)
    {
        try
        {
            string fileName = Path.GetFileName(sourcePath);
            string quarantinePath = Path.Combine(quarantineDirectory, fileName);

            if (File.Exists(quarantinePath))
            {
                Console.WriteLine("File already exists in quarantine. Overwriting...");
            }

            // Ensure the quarantine directory exists
            if (!Directory.Exists(quarantineDirectory))
            {
                Directory.CreateDirectory(quarantineDirectory);
                Console.WriteLine($"Quarantine directory created at {quarantineDirectory}");
            }

            await Task.Run(() => File.Move(sourcePath, quarantinePath, overwrite: true));
            Console.WriteLine($"File moved to quarantine: {quarantinePath}");

            return quarantinePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving file to quarantine: {ex.Message}");
            throw;
        }
    }
}
