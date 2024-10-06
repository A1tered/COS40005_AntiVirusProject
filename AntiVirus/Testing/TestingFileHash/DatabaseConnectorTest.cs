using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.FileHashScanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingFileHash
{
    public class DatabaseConnectorTest
    {
        private DirectoryManager _directoryManagerStub;
        private DatabaseConnector _databaseConnectorStub;
        private Hasher _hasherStub;
        private string _testFile;
        private string _testFile2;
        private string _testHash;
        private string _testHash2;

        [SetUp]
        public void Setup() 
        {
            SetupService.GetInstance(null, true);
            _directoryManagerStub = new DirectoryManager();
            _hasherStub = new Hasher();
            _databaseConnectorStub = new DatabaseConnector(_directoryManagerStub.getDatabaseDirectory("sighash.db"), true, false);
            _testFile = "C:\\TestDirectory\\Anikdote - Turn It Up [NCS Release] (2).mp3";
            _testFile2 = "C:\\Windows\\notepad.exe";
            _testHash = "31453a490dcf8027fd2fc5211fd4c647eed6e8b8";
            _testHash2 = "41453a490dcf8027fd2fc5211fd4c647eed6e8b8";
        }

        [Test]
        // If hash is found in the database, return true
        public void HashFoundTest()
        {
            Assert.That(_databaseConnectorStub.QueryHash(_hasherStub.OpenHashFile(_testFile)), Is.True);
        }

        [Test]
        // If hash is not found in the database, return false
        public void HashNotFoundTest()
        {
            Assert.That(_databaseConnectorStub.QueryHash(_hasherStub.OpenHashFile(_testFile2)), Is.False);
        }

        [Test]
        // Tests that a hash can be added.
        public void AddHashTest()
        {
            _databaseConnectorStub.AddHash(_testHash);
            Assert.That(_databaseConnectorStub.QueryHash(_testHash), Is.True);
            _databaseConnectorStub.RemoveHash(_testHash2);
            
        }

        [Test]
        // Tests that a hash can be removed.
        public void RemoveHashTest()
        {
            // Test
            _databaseConnectorStub.RemoveHash(_testHash2);
            Assert.That(_databaseConnectorStub.QueryHash(_testHash2), Is.False);
        }
    }
}