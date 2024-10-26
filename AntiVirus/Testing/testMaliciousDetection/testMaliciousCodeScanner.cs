using NUnit.Framework;
using SimpleAntivirus.MaliciousCodeScanning;
using System.IO;
using System.Threading.Tasks;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class MaliciousCodeScannerTests
    {
        private DatabaseHandler dbHandler;
        private Detector detector;
        private MaliciousCodeScanner scanner;
        private string testDirectory;

        [SetUp]
        public void Setup()
        {
            // Create the test directory
            testDirectory = Path.Combine(Path.GetTempPath(), "TestScanFiles");
            Directory.CreateDirectory(testDirectory);

            // Set up database handler and detector
            string dbFolder = Path.Combine(Path.GetTempPath(), "test_malicious_code_db");
            Directory.CreateDirectory(dbFolder);
            dbHandler = new DatabaseHandler(dbFolder);
            dbHandler.InsertMaliciousCommand("Invoke-Expression");
            detector = new Detector(dbHandler);

            // Set up the scanner
            scanner = new MaliciousCodeScanner(dbHandler, detector);

            // Create test files
            File.WriteAllText(Path.Combine(testDirectory, "safe.txt"), "This is a safe file.");
            File.WriteAllText(Path.Combine(testDirectory, "malicious.txt"), "This file contains Invoke-Expression");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test directory
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Test]
        public async Task TestScanDirectory()
        {
            // Run the directory scan
            await scanner.ScanDirectoryAsync(testDirectory);

            // If it gets here without exceptions, the test passes
            Assert.Pass();
        }
    }
}
