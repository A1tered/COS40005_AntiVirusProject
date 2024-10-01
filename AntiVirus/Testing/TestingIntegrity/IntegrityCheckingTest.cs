using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
        private string _selfModifyingDirectory;
        [SetUp]
        public void Setup()
        {
            SetupService.GetInstance(null, true);
            _integDatabase = new("IntegrityDatabase", true);
            _integrityManagement = new(_integDatabase);
            _selfModifyingDirectory = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\Testing\\TestingIntegrity\\self_modifying_testfolder";
            _fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\Testing\\TestingIntegrity\\hashExample.txt";
        }

        private string[] CreateFiles(int amountCreate)
        {
            List<string> directoriesCreated = new();
            string directoryTemp;
            for (int i = 0; i < amountCreate; i++)
            {
                directoryTemp = Path.Combine(_selfModifyingDirectory, $"testModifyingDocument{i}.txt");
                using (StreamWriter streamFile = new StreamWriter(directoryTemp))
                {
                    streamFile.WriteLine("im different");
                }
                directoriesCreated.Add(directoryTemp);
            }
            return directoriesCreated.ToArray();
        }

        private void ScrewFiles(string[] filesScrewed, int amountScrewed)
        {
            int amountScrewCounter = amountScrewed;
            foreach (string directory in filesScrewed)
            {
                if (amountScrewCounter <= 0)
                {
                    break;
                }
                using (StreamWriter streamFile = new StreamWriter(directory))
                {
                    streamFile.WriteLine("are you still there? :3");
                    amountScrewCounter -= 1;
                }
            }
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
        public async Task AddBaselineRemoveBaseline()
        {
            _integrityManagement.ClearDatabase();
            await _integrityManagement.AddBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.GreaterThanOrEqualTo(1));
            _integrityManagement.RemoveBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.EqualTo(0));
        }


        [Test]
        public async Task CheckIntegrityFileTest()
        {
            _integrityManagement.ClearDatabase();

            // Create file
            string[] screwDirectories = CreateFiles(10);

            foreach (string directory in screwDirectories)
            {
                await _integrityManagement.AddBaseline(directory);
            }

            List<IntegrityViolation> violationList = await _integrityManagement.Scan();

            Assert.That(violationList, Has.Exactly(0).Items);

            ScrewFiles(screwDirectories, 5);

            violationList = await _integrityManagement.Scan();

            Assert.That(violationList, Has.Exactly(5).Items);
        }


        // Data Pooler Specific Test
        [Test]
        public async Task CheckIntegrityTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityDataPooler datapooler = new IntegrityDataPooler(_integDatabase, 0, 100);
            List<IntegrityViolation> violationList = await datapooler.CheckIntegrity(new CancellationToken());
            Assert.That(violationList, Has.Exactly(5).Items);
        }

        // Data Pooler Specific Test
        [Test]
        public async Task CheckIntegritySingleTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityDataPooler datapooler = new IntegrityDataPooler(_integDatabase, Path.Combine(_selfModifyingDirectory, "testModifyingDocument0.txt"));
            List<IntegrityViolation> violationList = await datapooler.CheckIntegrityDirectory();
            Assert.That(violationList, Has.Exactly(1).Items);
        }

        // Integrity Cycler Test
        [Test]
        public async Task IntegrityCyclerTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityCycler cycler = new IntegrityCycler(_integDatabase, new ViolationHandler());
            List<IntegrityViolation> resultViolations = await cycler.InitiateScan();
            Assert.That(resultViolations, Has.Exactly(5).Items);
        }



        // Integ Inter
        [Test]
        public async Task AddEntryTest()
        {
            _integDatabase.DeleteAll();
            string fileCheck = @"C:\Users\yumcy\OneDrive\Desktop\Github Repositories\Technology Project A\COS40005_AntiVirusProject\AntiVirus\Testing\TestingIntegrity\testingFolder\testitem2.txt";
            Assert.That(_integDatabase.CheckExistence(fileCheck), Is.False);
            await _integDatabase.AddEntry(fileCheck, 100);
            Assert.That(_integDatabase.CheckExistence(fileCheck), Is.True);
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
