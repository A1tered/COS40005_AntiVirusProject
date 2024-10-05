using NUnit.Framework;
using Moq;
using SimpleAntivirus.FileQuarantine;
using System.IO;
using System.Threading.Tasks;

[TestFixture]
public class QuarantineManagerTests
{
    private string _testOriginalDirectory;
    private string _testQuarantineDirectory;
    private FileMover _fileMover;
    private Mock<IDatabaseManager> _databaseManagerMock;
    private QuarantineManager _quarantineManager;

    [SetUp]
    public void Setup()
    {
        // Set up test directories
        _testOriginalDirectory = Path.Combine(Path.GetTempPath(), "TestOriginalFiles");
        _testQuarantineDirectory = Path.Combine(Path.GetTempPath(), "TestQuarantineFiles");

        // Ensure directories are clean
        if (Directory.Exists(_testOriginalDirectory))
            Directory.Delete(_testOriginalDirectory, true);
        if (Directory.Exists(_testQuarantineDirectory))
            Directory.Delete(_testQuarantineDirectory, true);

        Directory.CreateDirectory(_testOriginalDirectory);
        Directory.CreateDirectory(_testQuarantineDirectory);

        // Initialize the actual FileMover
        _fileMover = new FileMover();

        // Set up mock for the database manager
        _databaseManagerMock = new Mock<IDatabaseManager>();

        // Instantiate the QuarantineManager with the real FileMover and mocked DatabaseManager
        _quarantineManager = new QuarantineManager(_fileMover, _databaseManagerMock.Object, _testQuarantineDirectory);
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

    // Test 1: Ensure a file is moved to quarantine successfully
    [Test]
    public async Task QuarantineFileAsync_FileMovesSuccessfully()
    {
        // Arrange
        string filePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
        File.WriteAllText(filePath, "Test content"); // Simulate a file

        // Mock the IsWhitelistedAsync method to return false, meaning the file is not whitelisted
        _databaseManagerMock.Setup(m => m.IsWhitelistedAsync(filePath))
            .ReturnsAsync(false);

        // Act
        await _quarantineManager.QuarantineFileAsync(filePath, null, "filehash");

        // Assert: Verify that the file is moved to quarantine
        Assert.IsTrue(File.Exists(Path.Combine(_testQuarantineDirectory, "testfile.txt")), "File was not moved to quarantine.");

        // Verify that the quarantine information is being stored in the database
        _databaseManagerMock.Verify(m => m.StoreQuarantineInfoAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once, "StoreQuarantineInfoAsync was not called.");
    }

    // Test 2: Unquarantine a file successfully
    [Test]
    public async Task UnquarantineFileAsync_FileRestoresSuccessfully()
    {
        // Arrange
        string filePath = Path.Combine(_testQuarantineDirectory, "testfile.txt");
        string originalFilePath = Path.Combine(_testOriginalDirectory, "testfile.txt");
        File.WriteAllText(filePath, "Test content"); // Simulate a quarantined file

        // Mock the database to return the correct file paths
        _databaseManagerMock.Setup(m => m.GetQuarantinedFileByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((filePath, originalFilePath));

        // Act
        bool result = await _quarantineManager.UnquarantineFileAsync(1);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(File.Exists(originalFilePath)); // Ensure file is restored
        _databaseManagerMock.Verify(m => m.RemoveQuarantineEntryAsync(It.IsAny<int>()), Times.Once);
    }

    // Test 3: Handle file not found scenario in quarantine
    [Test]
    public async Task UnquarantineFileAsync_FileNotFound_ReturnsFalse()
    {
        // Arrange: Mock the database to return null for a file that is not found
        _databaseManagerMock.Setup(m => m.GetQuarantinedFileByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((ValueTuple<string, string>?)null);

        // Act: Try to unquarantine the file
        bool result = await _quarantineManager.UnquarantineFileAsync(1);

        // Assert: The method should return false, and no file entry should be removed from the database
        Assert.IsFalse(result);
        _databaseManagerMock.Verify(m => m.RemoveQuarantineEntryAsync(It.IsAny<int>()), Times.Never);
    }


}
