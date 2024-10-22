using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.FileHashScanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingFileHash
{
    public class HunterTest
    {
        private DirectoryManager _directoryManagerStub;
        private Hunter _hunterStub;
        private CancellationToken TokenStub;
        private string _testFile;
        private string _testFile2;

        [SetUp]
        public void Setup() 
        {
            SetupService.GetInstance(true);
            _directoryManagerStub = new DirectoryManager();
            _hunterStub = new Hunter("C:\\TestDirectory", _directoryManagerStub.getDatabaseDirectory("sighash.db"), TokenStub);
            _testFile = "C:\\TestDirectory\\Anikdote - Turn It Up [NCS Release] (2).mp3";
            _testFile2 = "C:\\Windows\\notepad.exe";
        }

        [Test]
        // Returns true if violation is found
        public void ViolationFoundTest()
        {
            Assert.That(_hunterStub.CompareCycle(_testFile), Is.True);
        }

        [Test]
        // Returns false if no violation is found
        public void ViolationNotFoundTest()
        {
            Assert.That(_hunterStub.CompareCycle(_testFile2), Is.False);
        }
    }
}
