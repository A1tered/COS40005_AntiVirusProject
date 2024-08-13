using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class FileHasher
{
    public async Task<string> CalculateFileHashAsync(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = await sha256.ComputeHashAsync(fs);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
