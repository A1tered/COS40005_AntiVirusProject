using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingIntegrity
{
    public class IntegrityCheckingTest
    {
        private IntegrityManagement _integrityManagement;
        private IntegrityDatabaseIntermediary _integDatabase;
        private string _fileProvided;
        [SetUp]
        public void Setup()
        {
            SetupService.GetInstance(null, true);
            _integDatabase = new("IntegrityDatabase", true);
            _integrityManagement = new(_integDatabase);
            _fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\Testing\\TestingIntegrity\\hashExample.txt";
        }

        [TearDown]
        public void TearUp()
        {
            _integDatabase = null;
        }

        [Test]
        public async Task DeleteAllGetAmount()
        {
            _integrityManagement.ClearDatabase();
            await _integrityManagement.AddBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.GreaterThanOrEqualTo(1));
            _integDatabase.DeleteAll();
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.EqualTo(0));
        }

        [Test]
        public async Task AddEntryTest()
        {
            await _integDatabase.AddEntry(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles\12853b.jpg", 100);
        }

        //[Test]
        //public async Task CheckerTest()
        //{
        //    _integrityManagement.ClearDatabase();
        //    await _integrityManagement.AddBaseline(@"C:\\Users\\yumcy\\OneDrive\\Desktop\\UniversitySubjects\\COS40006 Computing Technology Project B\\TestingGround\\HundredIntegrityFiles");
        //    IntegrityDataPooler pooler = new(_integDatabase,0,100);
        //    List<IntegrityViolation> listItem = await pooler.CheckIntegrity();
        //    Assert.That(listItem.Count(), Is.EqualTo(1));
        //}
    }
}
