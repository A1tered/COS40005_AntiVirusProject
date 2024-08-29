using System;
using System.IO;
using System.Threading.Tasks;

public class FileMover
{
    public async Task<string> MoveFileToQuarantineAsync(string sourcePath, string quarantineDirectory)
    {
        string fileName = Path.GetFileName(sourcePath);
        string quarantinePath = Path.Combine(quarantineDirectory, fileName);

        if (File.Exists(quarantinePath))
        {
            Console.WriteLine("File already exists in quarantine. Overwriting...");
        }

        await Task.Run(() => File.Move(sourcePath, quarantinePath, overwrite: true));
        Console.WriteLine($"File moved to quarantine: {quarantinePath}");

        return quarantinePath;
    }
}