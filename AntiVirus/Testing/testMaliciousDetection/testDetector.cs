using NUnit.Framework;
using SimpleAntivirus.MaliciousCodeScanning;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class DetectorTests
    {
        private Detector detector;
        private DatabaseHandler dbHandler;

        [SetUp]
        public void Setup()
        {
            // Set up the database handler and detector for testing
            string dbFolder = Path.Combine(Path.GetTempPath(), "test_malicious_code_db");
            Directory.CreateDirectory(dbFolder);
            dbHandler = new DatabaseHandler(dbFolder);
            detector = new Detector(dbHandler);

            // Insert some malicious commands
            dbHandler.InsertMaliciousCommand("Invoke-Expression");
        }

        [Test]
        public void TestDetectMaliciousCommand()
        {
            // File content with a malicious command
            string fileContent = "This file contains Invoke-Expression";
            bool isMalicious = detector.ContainsMaliciousCommands(fileContent);

            // Assert that the file is detected as malicious
            Assert.IsTrue(isMalicious);
        }

        [Test]
        public void TestDetectSafeContent()
        {
            // File content without any malicious command
            string fileContent = "This is a safe file.";
            bool isMalicious = detector.ContainsMaliciousCommands(fileContent);

            // Assert that the file is detected as safe
            Assert.IsFalse(isMalicious);
        }
    }
}
