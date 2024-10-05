using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;
using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using SimpleAntivirus.IntegrityModule.Reactive;
using System.Diagnostics;
using TestingIntegrity.DummyClasses;
namespace TestingIntegrity
{
    public class IntegrityNonFunctionalTests
    {
        private IntegrityDatabaseIntermediary _integData;
        private IntegrityManagement _integrityManagement;
        private string fileProvided;
        [SetUp]
        public void Setup()
        {
            SetupService.GetInstance(null, true);
            _integData = new("IntegrityDatabase", true);
            _integrityManagement = new(_integData);
        }

        [Test]
        public async Task PerformanceTest()
        {
            string directoryFolder = @"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\TwentyThousandTest";
            _integData.DeleteAll();
            await _integrityManagement.AddBaseline(directoryFolder);
            string[] files = Directory.GetFiles(directoryFolder);
            for (int i = 0; i < 10; i++)
            {
                File.AppendAllText(files[i], "hi");
            }
            Stopwatch stopwatchHandler = new();
            stopwatchHandler.Start();
            List<IntegrityViolation> integrityViolations = await _integrityManagement.Scan(false);
            stopwatchHandler.Stop();
            Assert.That(integrityViolations, Has.Exactly(10).Items);
            TestContext.WriteLine($"Time scan taken: {stopwatchHandler.Elapsed}");
            Assert.That(stopwatchHandler.ElapsedMilliseconds, Is.LessThan(3000));
        }

        [Test]
        public async Task RobustnessTestDirectoryNonExistant()
        {
            bool returnData = await _integrityManagement.AddBaseline("C:\\Users\\yumcy\\OneDrive\\Desktop\\UniversitySubjects\\COS40006 Computing Technology Project B\\TestingGround\\NonExistantFolder");
            Assert.That(returnData, Is.False);
        }

    }
}
