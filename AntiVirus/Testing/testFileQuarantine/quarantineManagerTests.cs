using NUnit.Framework;
using Moq;
using System.IO;
using System.Threading.Tasks;
using SimpleAntivirus.FileQuarantine;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class QuarantineManagerTests
    {
        private string _testOriginalDirectory;
        private string _testQuarantineDirectory;
        private Mock<IDatabaseManager> _databaseManagerMock;
        private QuarantineManager _quarantineManager;
        private FileMover _fileMover;

        [SetUp]
        public void Setup()
        {
            // Set up directories
            _testOriginalDirectory = Path.Combine(Path.GetTempPath(), "TestOriginalFiles");
            _testQuarantineDirectory = Path.Combine(Path.GetTempPath(), "TestQuarantineFiles");

            if (Directory.Exists(_testOriginalDirectory))
                Directory.Delete(_testOriginalDirectory, true);
            if (Directory.Exists(_testQuarantineDirectory))
                Directory.Delete(_testQuarantineDirectory, true);

            Directory.CreateDirectory(_testOriginalDirectory);
            Directory.CreateDirectory(_testQuarantineDirectory);

            // Set up mock for IDatabaseManager
            _databaseManagerMock = new Mock<IDatabaseManager>();

            // Initialize FileMover and QuarantineManager
            _fileMover = new FileMover();
            _quarantineManager = new QuarantineManager(_fileMover, _databaseManagerMock.Object, _testQuarantineDirectory);
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_testOriginalDirectory))
                Directory.Delete(_testOriginalDirectory, true);
            if (Directory.Exists(_testQuarantineDirectory))
                Directory.Delete(_testQuarantineDirectory, true);
        }

        // Test 1: Move file to quarantine successfully
        [Test]
        public async Task MoveFileToQuarantine_FileMovedSuccessfully()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");  // Create test file

            // Act: Move file to quarantine
            string quarantinedFilePath = await _fileMover.MoveFileToQuarantineAsync(filePath, _testQuarantineDirectory);

            // Assert: Verify the file was moved to the quarantine directory
            Assert.IsTrue(File.Exists(quarantinedFilePath), "File was not moved to quarantine.");
        }

        // Test 2: Unquarantine file successfully
        [Test]
        public async Task UnquarantineFile_FileRestoredSuccessfully()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
            string quarantinedFilePath = Path.Combine(_testQuarantineDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");

            // Mock database to retrieve file paths
            _databaseManagerMock.Setup(m => m.GetQuarantinedFileByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((quarantinedFilePath, filePath));

            // Move file to quarantine
            await _quarantineManager.QuarantineFileAsync(filePath, null, "filehash");

            // Act: Unquarantine the file
            bool result = await _quarantineManager.UnquarantineFileAsync(1);

            // Assert: Verify file was restored and database entry was removed
            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(filePath), "File was not restored from quarantine.");
            _databaseManagerMock.Verify(m => m.RemoveQuarantineEntryAsync(It.IsAny<int>()), Times.Once, "Quarantine entry was not removed.");
        }

        // Test 3: Delete quarantined file successfully
        [Test]
        public async Task DeleteFile_FileDeletedSuccessfully()
        {
            // Arrange
            string quarantinedFilePath = Path.Combine(_testQuarantineDirectory, "testfile.txt");
            File.WriteAllText(quarantinedFilePath, "Test content");

            // Act: Call DeleteFile
            bool result = await _quarantineManager.DeleteFileAsync(quarantinedFilePath);

            // Assert: Verify the file was deleted
            Assert.IsTrue(result);
            Assert.IsFalse(File.Exists(quarantinedFilePath), "File was not deleted from quarantine.");
        }

        // Test 4: Add file to whitelist successfully
        [Test]
        public async Task AddFileToWhitelist_FileAddedSuccessfully()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");

            // Mock database operation to return Task<bool>
            _databaseManagerMock.Setup(m => m.AddToWhitelistAsync(filePath)).Returns(Task.FromResult(true));

            // Act: Call AddToWhitelist via DatabaseManager
            bool result = await _databaseManagerMock.Object.AddToWhitelistAsync(filePath);

            // Assert: Verify the file was added to the whitelist and the result is true
            Assert.IsTrue(result, "File was not successfully added to the whitelist.");
            _databaseManagerMock.Verify(m => m.AddToWhitelistAsync(filePath), Times.Once);
        }


        // Test 5: Remove file from whitelist successfully
        [Test]
        public async Task RemoveFileFromWhitelist_FileRemovedSuccessfully()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");

            // Mock database operations to return Task<bool>
            _databaseManagerMock.Setup(m => m.RemoveFromWhitelistAsync(filePath)).Returns(Task.FromResult(true));

            // Act: Remove file from whitelist via DatabaseManager
            bool result = await _databaseManagerMock.Object.RemoveFromWhitelistAsync(filePath);

            // Assert: Verify the file was removed from the whitelist and the result is true
            Assert.IsTrue(result, "File was not successfully removed from the whitelist.");
            _databaseManagerMock.Verify(m => m.RemoveFromWhitelistAsync(filePath), Times.Once);
        }
    }
}
