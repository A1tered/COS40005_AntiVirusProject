namespace SimpleAntivirus.FileQuarantine
{
    public interface IDatabaseManager
    {
        /// Checks if a file is present in the whitelist.

        /// <param name="filePath">The full path of the file to check.</param>
        /// <returns>True if the file is whitelisted, otherwise false.</returns>
        Task<bool> IsWhitelistedAsync(string filePath);

        /// Stores information about a quarantined file in the database.

        /// <param name="quarantinedFilePath">The path where the file is quarantined.</param>
        /// <param name="originalFilePath">The original path of the file before quarantining.</param>
        Task StoreQuarantineInfoAsync(string quarantinedFilePath, string originalFilePath);

        /// Removes a quarantine entry from the database.

        /// <param name="id">The ID of the quarantined file entry to remove.</param>
        Task RemoveQuarantineEntryAsync(int id);

        /// Retrieves the quarantine details of a file by its ID.

        /// <param name="id">The ID of the quarantined file.</param>
        /// <returns>A tuple containing the quarantined file's path and its original location, or null if not found.</returns>
        Task<(string QuarantinedFilePath, string OriginalFilePath)?> GetQuarantinedFileByIdAsync(int id);

        /// Retrieves all quarantined files from the database.

        /// <returns>A list of all quarantined files, including their IDs, quarantined paths, and original paths.</returns>
        Task<IEnumerable<(int Id, string QuarantinedFilePath, string OriginalFilePath)>> GetAllQuarantinedFilesAsync();

        /// Adds a file to the whitelist in the database.

        /// <param name="filePath">The full path of the file to add to the whitelist.</param>
        Task AddToWhitelistAsync(string filePath);

        /// Removes a file from the whitelist in the database.

        /// <param name="filePath">The full path of the file to remove from the whitelist.</param>
        Task RemoveFromWhitelistAsync(string filePath);

        /// Retrieves all whitelisted files from the database.

        /// <returns>A list of file paths that are whitelisted.</returns>
        Task<IEnumerable<string>> GetWhitelistAsync();
    }
}
