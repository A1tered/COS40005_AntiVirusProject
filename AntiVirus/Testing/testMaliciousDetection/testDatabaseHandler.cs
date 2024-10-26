using NUnit.Framework;
using System.IO;
using SimpleAntivirus.MaliciousCodeScanning;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class DatabaseHandlerTests
    {
        private string dbFolder;
        private DatabaseHandler dbHandler;

        [SetUp]
        public void Setup()
        {
            // Set up an SQLite database for testing
            dbFolder = Path.Combine(Path.GetTempPath(), "test_malicious_code_db");
            Directory.CreateDirectory(dbFolder);
            dbHandler = new DatabaseHandler(dbFolder);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test database
            if (Directory.Exists(dbFolder))
            {
                Directory.Delete(dbFolder, true);
            }
        }

        [Test]
        public void TestInsertAndRetrieveMaliciousCommand()
        {
            // Insert a malicious command
            string command = "Invoke-Expression";
            dbHandler.InsertMaliciousCommand(command);

            // Retrieve all malicious commands
            var commands = dbHandler.GetMaliciousCommands();

            // Assert that the command is retrieved successfully
            Assert.Contains(command, commands);
        }
    }
}
