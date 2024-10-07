/**************************************************************************
 * File:        DatabaseTest.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with tests specific to DatabaseIntermediary class.
 * Last Modified: 8/10/2024
 **************************************************************************/

using Microsoft.Data.Sqlite;
using NUnit.Framework.Internal;
using SimpleAntivirus.GUI.Services;
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
            fileProvided = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "hashExample.txt");
        }

        [TearDown]
        public void TearUp()
        {
            _integData.Dispose();
            _integData = null;
        }

        /// <summary>
        /// Tests whether the DatabaseIntermediary (non-specific) can correctly insert data into itself.
        /// </summary>
        [Test]
        public void InsertRowTest()
        {
            SqliteCommand sqliteCommand = new();
            _integData.DeleteAll();
            sqliteCommand.CommandText = $"REPLACE INTO IntegrityTrack VALUES('{fileProvided}', 'B', 10000, 1111, 1111)";
            Assert.That(_integData.QueryNoReader(sqliteCommand), Is.EqualTo(1));
        }

        /// <summary>
        ///  Tests whether non-specific database intermediary can correctly take queries with no issues.
        /// </summary>
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

        /// <summary>
        /// Tests whether non specific database intermediary has correctly taken insert queries correctly.
        /// </summary>
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
