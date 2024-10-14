using SimpleAntivirus.FileHashScanning;
using System.Security.Cryptography;
using System.IO;

namespace TestingFileHash
{
    public class HasherTest
    {
        private Hasher _hasherStub;
        private string _directory;
        private string _hash;

        [SetUp]
        public void Setup()
        {
            _hasherStub = new Hasher();
            _directory = "C:\\Windows\\notepad.exe";
            _hash = "D11D30AD4F2780FFEE3626901BC50CCF5B20FC2D";
        }

        [Test]
        // Opens and hashes the file, returns hash of given file
        public void OpenHashFileTest()
        {
            Assert.That(_hasherStub.OpenHashFile(_directory), Is.EqualTo(_hash));
        }

        [Test]
        // Hashes file, returns the hash of the filestream
        public void HashFileTest()
        {
            FileStream fileStreamTest = File.Open(_directory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            Assert.That(_hasherStub.HashFile(fileStreamTest), Is.EqualTo(_hash));
        }
    }
}