public interface IQuarantineManager
{
    Task QuarantineFileAsync(string filePath);
    Task UnquarantineFileAsync(int id);
    Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath)>> GetQuarantinedFilesAsync();
}
