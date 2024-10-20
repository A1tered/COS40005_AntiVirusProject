using NUnit.Framework;
using SimpleAntivirus.FileQuarantine;
using System.Threading.Tasks;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        private IDatabaseManager _databaseManager;
        private string _databasePath;

        [SetUp]
        public void Setup()
        {
            // Use a test database file in a temporary location
            _databasePath = Path.Combine(Path.GetTempPath(), "testDatabase.db");

            // Initialize DatabaseManager with the test database
            _databaseManager = new DatabaseManager(_databasePath);
        }

        [Test]
        public async Task StoreQuarantineInfoAsync_ShouldStoreDataSuccessfully()
        {
            // Arrange
            string quarantinedFilePath = "C:\\path\\to\\quarantinedFile.txt";
            string originalFilePath = "C:\\path\\to\\originalFile.txt";

            // Act
            await _databaseManager.StoreQuarantineInfoAsync(quarantinedFilePath, originalFilePath);

            // Assert
            var result = await _databaseManager.GetQuarantinedFileByIdAsync(1); // Assuming it's the first entry
            Assert.NotNull(result);
            Assert.AreEqual(quarantinedFilePath, result.Value.QuarantinedFilePath);
            Assert.AreEqual(originalFilePath, result.Value.OriginalFilePath);
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up test database file after tests
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
    }
}
