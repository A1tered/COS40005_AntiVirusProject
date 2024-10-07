/**************************************************************************
 * File:        IntegrityNonFunctionalTests.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with non-functional tests related to the whole IntegrityModule.
 * Last Modified: 8/10/2024
 **************************************************************************/

using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.IntegrityModule.ControlClasses;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using System.Diagnostics;
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

        /// <summary>
        /// Tests the performance of scanning around 20,000 items and measures the time taken. Due to the size of the test folder, this test needs to be
        /// altered if ran on another system.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Ensure there is no exception if you enter a directory that does not exist. Considering the test requires directories that does not exist,
        /// running this on another computer should have no issues.
        /// </summary>
        [Test]
        public async Task RobustnessTestDirectoryNonExistant()
        {
            bool returnData = await _integrityManagement.AddBaseline("C:\\Users\\yumcy\\OneDrive\\Desktop\\UniversitySubjects\\COS40006 Computing Technology Project B\\TestingGround\\NonExistantFolder");
            Assert.That(returnData, Is.False);
        }

    }
}
