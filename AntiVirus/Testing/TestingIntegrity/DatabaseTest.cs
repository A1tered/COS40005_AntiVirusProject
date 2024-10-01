using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;
using SimpleAntivirus.IntegrityModule.Db;
namespace TestingIntegrity
{
    public class DatabaseTest
    {
        private IntegrityDatabaseIntermediary _integData;
        private string fileProvided;
        [SetUp]
        public void Setup()
        {
            SetupService.GetInstance(null, true);
            _integData = new IntegrityDatabaseIntermediary("IntegrityDatabase", true);
            fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\Testing\\TestingIntegrity\\hashExample.txt";
        }

        [TearDown]
        public void TearUp()
        {
            _integData.Dispose();
            _integData = null;
        }

        [Test]
        public void InsertRowTest()
        {
            SqliteCommand sqliteCommand = new();
            _integData.DeleteAll();
            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided}', 'B', 10000, 1111, 1111)";
            Assert.That(_integData.QueryNoReader(sqliteCommand), Is.EqualTo(1));
        }

        [Test]
        public void QueryReaderTestInstance()
        {
            SqliteCommand sqliteCommand = new();
            _integData.DeleteAll();
            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided}', 'B', 10000, 1111, 1111)";
            _integData.QueryNoReader(sqliteCommand);
            sqliteCommand.CommandText = $"SELECT * FROM IntegrityTrack";
            Assert.That(_integData.QueryReader(sqliteCommand), Is.Not.Null);
        }

        [Test]
        public void SelectRowTest()
        {
            SqliteCommand sqliteCommand = new();
            _integData.DeleteAll();
            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided}', 'B', 10000, 1111, 1111)";
            _integData.QueryNoReader(sqliteCommand);
            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided} b b', 'B', 10000, 1111, 1111)";
            _integData.QueryNoReader(sqliteCommand);
            SqliteCommand otherCommand = new();
            Assert.That(_integData.QueryAmount("IntegrityTrack"), Is.EqualTo(2));
        }
    }
}
