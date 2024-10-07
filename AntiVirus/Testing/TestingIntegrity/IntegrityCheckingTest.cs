/**************************************************************************
 * File:        IntegrityCheckingTests.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with integration tests related to IntegrityModule.
 * Last Modified: 8/10/2024
 **************************************************************************/


using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;

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
            _selfModifyingDirectory = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "self_modifying_testfolder");
            _fileProvided = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "hashExample.txt");
        }

        /// <summary>
        /// Create files for ease of testing.
        /// </summary>
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

        /// <summary>
        /// This function is a test function, it allows ease to change the state of files being monitored by integrity easily.
        /// </summary>
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


        /// <summary>
        /// This test checks multiple functions at once, these include AddBaseline, QueryAmount and DeleteAll. If any one of these functions fail, the test
        /// should fail.
        /// </summary>
        [Test]
        public async Task DeleteAllGetAmount()
        {
            _integrityManagement.ClearDatabase();
            await _integrityManagement.AddBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.GreaterThanOrEqualTo(1));
            _integDatabase.DeleteAll();
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.EqualTo(0));
        }

        /// <summary>
        /// This test checks whether IntegrityManagement (top level class) is able to add to the database correctly.
        /// </summary>
        [Test]
        public async Task AddBaselineRemoveBaseline()
        {
            _integrityManagement.ClearDatabase();
            await _integrityManagement.AddBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.GreaterThanOrEqualTo(1));
            _integrityManagement.RemoveBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityTrack"), Is.EqualTo(0));
        }


        /// <summary>
        /// This test checks multiple features which includes adding to the database, and then scanning the database and ensuring that the exact amount of violations
        /// is returned.
        /// </summary>
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


        /// <summary>
        /// This test is responsible for testing IntegrityDataPooler, and its ability to scan multiple items at once.
        /// </summary>
        [Test]
        public async Task CheckIntegrityTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityDataPooler datapooler = new IntegrityDataPooler(_integDatabase, 0, 100);
            List<IntegrityViolation> violationList = await datapooler.CheckIntegrity(new CancellationToken());
            Assert.That(violationList, Has.Exactly(5).Items);
        }

        /// <summary>
        /// This test is responsible for testing IntegrityDataPooler and its ability to scan its contents.
        /// </summary>
        [Test]
        public async Task CheckIntegritySingleTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityDataPooler datapooler = new IntegrityDataPooler(_integDatabase, Path.Combine(_selfModifyingDirectory, "testModifyingDocument0.txt"));
            List<IntegrityViolation> violationList = await datapooler.CheckIntegrityDirectory();
            Assert.That(violationList, Has.Exactly(1).Items);
        }

        /// <summary>
        /// This test utilises IntegrityCycler, and its InitiateScan functionality.
        /// </summary>
        [Test]
        public async Task IntegrityCyclerTest()
        {

            // We use a previous test to generate a incorrect entry, to trigger a violation.
            await CheckIntegrityFileTest();
            IntegrityCycler cycler = new IntegrityCycler(_integDatabase, new ViolationHandler());
            List<IntegrityViolation> resultViolations = await cycler.InitiateScan();
            Assert.That(resultViolations, Has.Exactly(5).Items);
        }



        /// <summary>
        /// This test is responsible for adding entry via IntegrityDatabaseIntermediary.
        /// </summary>
        [Test]
        public async Task AddEntryTest()
        {
            _integDatabase.DeleteAll();
            string fileCheck = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "testingFolder\\testitem2.txt");
            Assert.That(_integDatabase.CheckExistence(fileCheck), Is.False);
            await _integDatabase.AddEntry(fileCheck, 100);
            Assert.That(_integDatabase.CheckExistence(fileCheck), Is.True);
        }
    }
}
