using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class QuarantineManager
{
    private readonly FileHasher _fileHasher;
    private readonly FileMover _fileMover;
    private readonly List<string> _whitelist = new List<string>();
    private readonly Dictionary<string, FileMetadata> _metadataStorage = new Dictionary<string, FileMetadata>();

    public QuarantineManager(FileHasher fileHasher, FileMover fileMover)
    {
        _fileHasher = fileHasher;
        _fileMover = fileMover;
    }

    public async Task QuarantineFileAsync(string filePath, string quarantinePath)
    {
        if (await IsWhitelistedAsync(filePath))
        {
            Console.WriteLine("File is whitelisted and will not be quarantined.");
            return;
        }

        string fileHash = await _fileHasher.CalculateFileHashAsync(filePath);
        await _fileMover.MoveFileToQuarantineAsync(filePath, quarantinePath);
        var metadata = new FileMetadata
        {
            FilePath = quarantinePath,
            Hash = fileHash,
            QuarantineDate = DateTime.Now,
            OriginalPermissions = "rw-r--r--", // Example permissions, should be fetched from the file
            ThreatLevel = "High" // Example threat level
        };
        _metadataStorage[quarantinePath] = metadata;
    }

    public async Task UnquarantineFileAsync(string quarantinePath, string originalPath)
    {
        await _fileMover.MoveFileFromQuarantineAsync(quarantinePath, originalPath);
        _metadataStorage.Remove(quarantinePath);
    }

    public async Task DeleteQuarantinedFileAsync(string quarantinePath)
    {
        File.Delete(quarantinePath);
        _metadataStorage.Remove(quarantinePath);
    }

    public async Task<FileMetadata> GetFileMetadataAsync(string filePath)
    {
        _metadataStorage.TryGetValue(filePath, out var metadata);
        return await Task.FromResult(metadata);
    }

    public async Task AddToWhitelistAsync(string filePath)
    {
        if (!_whitelist.Contains(filePath))
        {
            _whitelist.Add(filePath);
            await Task.CompletedTask;
        }
    }

    public async Task RemoveFromWhitelistAsync(string filePath)
    {
        if (_whitelist.Contains(filePath))
        {
            _whitelist.Remove(filePath);
            await Task.CompletedTask;
        }
    }

    public async Task<bool> IsWhitelistedAsync(string filePath)
    {
        return await Task.FromResult(_whitelist.Contains(filePath));
    }
}

public class FileMetadata
{
    public string FilePath { get; set; }
    public string Hash { get; set; }
    public DateTime QuarantineDate { get; set; }
    public string OriginalPermissions { get; set; }
    public string ThreatLevel { get; set; }
}
