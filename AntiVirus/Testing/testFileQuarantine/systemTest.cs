using NUnit.Framework;
using Moq;
using System.IO;
using System.Threading.Tasks;
using SimpleAntivirus.FileQuarantine;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class MaliciousCodeScannerStubTests
    {
        private string _testOriginalDirectory;
        private string _testQuarantineDirectory;
        private Mock<IDatabaseManager> _databaseManagerMock;
        private QuarantineManager _quarantineManager;
        private MaliciousCodeScannerStub _scannerStub;
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

            // Initialize the stub
            _scannerStub = new MaliciousCodeScannerStub(_quarantineManager);
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_testOriginalDirectory))
                Directory.Delete(_testOriginalDirectory, true);
            if (Directory.Exists(_testQuarantineDirectory))
                Directory.Delete(_testQuarantineDirectory, true);
        }

        // Test: Stub successfully quarantines a file
        [Test]
        public async Task StubCanSuccessfullyQuarantineFile()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");  // Create test file

            // Mock database store operation
            _databaseManagerMock.Setup(m => m.StoreQuarantineInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act: The stub detects and quarantines the file
            bool result = await _scannerStub.DetectAndQuarantineFileAsync(filePath);

            // Assert: Verify the file was quarantined successfully
            Assert.IsTrue(result, "The file was not quarantined by the stub.");
            Assert.IsTrue(File.Exists(Path.Combine(_testQuarantineDirectory, "testfile.txt")), "File was not moved to quarantine.");
            _databaseManagerMock.Verify(m => m.StoreQuarantineInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "Quarantine metadata was not stored.");
        }

        // Test: Stub successfully unquarantines a file
        [Test]
        public async Task StubCanSuccessfullyUnquarantineFile()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
            string quarantinedFilePath = Path.Combine(_testQuarantineDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");  // Create test file

            // Mock database to retrieve file paths
            _databaseManagerMock.Setup(m => m.GetQuarantinedFileByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((quarantinedFilePath, filePath));

            // Mock database remove operation
            _databaseManagerMock.Setup(m => m.RemoveQuarantineEntryAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Move file to quarantine
            await _quarantineManager.QuarantineFileAsync(filePath, null, "filehash");

            // Act: The stub unquarantines the file
            bool result = await _scannerStub.UnquarantineFileAsync(1);

            // Assert: Verify the file was unquarantined successfully
            Assert.IsTrue(result, "The file was not unquarantined by the stub.");
            Assert.IsTrue(File.Exists(filePath), "File was not restored from quarantine.");
            _databaseManagerMock.Verify(m => m.RemoveQuarantineEntryAsync(It.IsAny<int>()), Times.Once, "Quarantine entry was not removed.");
        }
    }
}
