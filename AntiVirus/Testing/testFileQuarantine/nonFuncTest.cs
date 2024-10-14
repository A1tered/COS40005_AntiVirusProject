using NUnit.Framework;
using Moq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SimpleAntivirus.FileQuarantine;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class QuarantineManagerPerformanceTests
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

        // Test: Measure the performance of QuarantineFileAsync
        [Test]
        public async Task QuarantineFile_PerformanceTest()
        {
            // Arrange
            string filePath = Path.Combine(_testOriginalDirectory, "largeTestFile.txt");
            File.WriteAllText(filePath, new string('A', 10 * 1024 * 1024));  // Create a large 10 MB test file

            // Ensure the file is NOT in the whitelist
            _databaseManagerMock.Setup(m => m.IsWhitelistedAsync(filePath))
                .ReturnsAsync(false);

            // Mock database store operation
            _databaseManagerMock.Setup(m => m.StoreQuarantineInfoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act: Measure the time it takes to quarantine the file
            Stopwatch stopwatch = Stopwatch.StartNew();
            await _quarantineManager.QuarantineFileAsync(filePath, null, "filehash");
            stopwatch.Stop();

            // Assert: Verify the file was quarantined and check performance
            Assert.IsTrue(File.Exists(Path.Combine(_testQuarantineDirectory, "largeTestFile.txt")), "File was not quarantined.");
            Assert.Less(stopwatch.ElapsedMilliseconds, 1000, "Quarantine operation took too long."); // Set an arbitrary threshold (e.g., 1 second)
        }
    }
}
