using NUnit.Framework;
using SimpleAntivirus.FileQuarantine;
using System.IO;
using System.Threading.Tasks;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class FileMoverTests
    {
        private string _testOriginalDirectory;
        private string _testQuarantineDirectory;
        private FileMover _fileMover;

        [SetUp]
        public void Setup()
        {
            // Set up test directories
            _testOriginalDirectory = Path.Combine(Path.GetTempPath(), "TestOriginalFiles");
            _testQuarantineDirectory = Path.Combine(Path.GetTempPath(), "TestQuarantineFiles");

            // Ensure directories are clean before each test
            if (Directory.Exists(_testOriginalDirectory))
                Directory.Delete(_testOriginalDirectory, true);
            if (Directory.Exists(_testQuarantineDirectory))
                Directory.Delete(_testQuarantineDirectory, true);

            Directory.CreateDirectory(_testOriginalDirectory);
            Directory.CreateDirectory(_testQuarantineDirectory);

            // Initialize FileMover (this comes from the project you uploaded)
            _fileMover = new FileMover();
        }

        [TearDown]
        public void Cleanup()
        {
            // Clean up test directories after each test
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

        // Test 2: Move file from quarantine back to original location successfully
        [Test]
        public async Task MoveFileFromQuarantine_FileRestoredSuccessfully()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
            string quarantinedFilePath = Path.Combine(_testQuarantineDirectory, "testfile.txt");
            File.WriteAllText(filePath, "Test content");  // Create test file

            // Move the file to quarantine first
            await _fileMover.MoveFileToQuarantineAsync(filePath, _testQuarantineDirectory);

            // Act: Move the file back from quarantine to original location
            string restoredFilePath = await _fileMover.MoveFileToQuarantineAsync(quarantinedFilePath, _testOriginalDirectory);

            // Assert: Verify the file was moved back to its original location
            Assert.IsTrue(File.Exists(restoredFilePath), "File was not restored from quarantine.");
        }
    }
}
