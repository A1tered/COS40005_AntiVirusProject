public interface IDatabaseManager
{
    Task<bool> IsWhitelistedAsync(string filePath);
    Task StoreQuarantineInfoAsync(string quarantinedFilePath, string originalFilePath);
    Task RemoveQuarantineEntryAsync(int id);
    Task<(string QuarantinedFilePath, string OriginalFilePath)?> GetQuarantinedFileByIdAsync(int id);
    Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath)>> GetAllQuarantinedFilesAsync();
}
