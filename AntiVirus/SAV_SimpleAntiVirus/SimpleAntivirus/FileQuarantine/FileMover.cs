using System.IO;
using System.Diagnostics;


namespace SimpleAntivirus.FileQuarantine
{
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
                    Debug.WriteLine("File already exists in quarantine. Overwriting...");
                }

                // Ensure the quarantine directory exists before moving the file
                if (!Directory.Exists(quarantineDirectory))
                {
                    Directory.CreateDirectory(quarantineDirectory);
                    Debug.WriteLine($"Quarantine directory created at {quarantineDirectory}");
                }

                // Move the file asynchronously to the quarantine directory
                await Task.Run(() =>
                {
                    try
                    {
                        File.Move(sourcePath, quarantinePath, overwrite: true);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine($"Cannot move file {sourcePath} due to an unauthorized access exception. Skipping...");
                        quarantinePath = null;
                        return;
                    }
                });
                Debug.WriteLine($"File moved to quarantine: {quarantinePath}");
                return quarantinePath;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Unauthorized Access Exception at file: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Handle any errors during file moving
                Debug.WriteLine($"Error moving file to quarantine: {ex.Message}");
                throw;
            }
        }
    }
}
