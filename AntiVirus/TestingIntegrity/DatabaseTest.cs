using DatabaseFoundations;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingIntegrity
{
    public class DatabaseTest
    {
        private IntegrityDatabaseIntermediary _integData;
        private string fileProvided;
        [SetUp]
        public void Setup()
        {
            _integData = new IntegrityDatabaseIntermediary("IntegrityDatabase", true);
            fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\TestingIntegrity\\hashExample.txt";
        }

        [TearDown]
        public void TearUp()
        {
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
