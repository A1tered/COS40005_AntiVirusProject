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
            string relativeDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            fileProvided = Path.Join(relativeDirectory, "hashExample.txt");
            SetupService.GetInstance(true);
            _integData = new IntegrityDatabaseIntermediary("IntegrityDatabase", true);
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
            sqliteCommand.CommandText = $"SELECT * FROM IntegrityTrack";
            Assert.That(_integData.QueryReader(sqliteCommand).HasRows, Is.False);
            sqliteCommand = new();

            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided}', 'B', 10000, 1111, 1111)";
            _integData.QueryNoReader(sqliteCommand);

            sqliteCommand.CommandText = $"SELECT * FROM IntegrityTrack";
            Assert.That(_integData.QueryReader(sqliteCommand).HasRows, Is.True);
        }

        [Test]
        public void QueryAmountTest()
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
