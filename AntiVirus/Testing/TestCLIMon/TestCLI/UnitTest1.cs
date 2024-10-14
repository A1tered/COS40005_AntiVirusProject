using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleAntivirus.CLIMonitoring;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;

namespace SimpleAntivirus.CLIMonitoring.Tests
{
    public class TestProcessFilter
    {
        private CLIMonitor _cliMonitor;
        
        [SetUp]
        public void SetUp()
        {
            _cliMonitor = new CLIMonitor(null);
        }

        [Test]
        public void TestProcessNameTrue()
        {
            // Arrange
            string powershellProcess = "powershell";
            string cmdProcess = "cmd";

            // Act
            bool isPowershellRelevant = _cliMonitor.IsRelevantProcess(powershellProcess, 1234);
            bool isCmdRelevant = _cliMonitor.IsRelevantProcess(cmdProcess, 5678);

            // Assert
            Assert.IsTrue(isPowershellRelevant);
            Assert.IsTrue(isCmdRelevant);
        }

        [Test]
        public void TestProcessNameFalse()
        {
            // Arrange
            string TestProcess = "chrome";

            bool CheckRelevantProcess = _cliMonitor.IsRelevantProcess(TestProcess, 5678);

            // Assert
            Assert.IsFalse(CheckRelevantProcess);
       
        }
    }


    public class TestEventMonitoringAccess
    {
        private CLIMonitor _cliMonitor;

        [SetUp]
        public void SetUp()
        {
            // Initialize CLIMonitor
            _cliMonitor = new CLIMonitor(null);
        }

        [Test]
        public void InitializeSession_ShouldInitializeSession()
        {
            // Act: Call InitializeSession
            _cliMonitor.InitializeSession();

            // Assert: Check that the session is initialized
            Assert.NotNull(_cliMonitor.Session, "Session should not be null after initialization.");
        }

       
    }

    public class RegistryTraceData
    {
        //Used for TestAlertOutput to create an object that mimics the properties of an Event Process
        public int ProcessID { get; set; }
        public string KeyName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public class TestAlertOutput
    {
        private CLIMonitor _cliMonitor;

        [SetUp]
        public void SetUp()
        {
            _cliMonitor = new CLIMonitor(null);
        }

        [Test]
        public async Task TestOutput()
        {
            // Arrange: Create custom registry event data
            var registryData = new RegistryTraceData
            {
                ProcessID = 1234,
                KeyName = "HKEY_LOCAL_MACHINE\\Software\\Test",
                TimeStamp = DateTime.Now
            };

            // Act: Call TrackRegistryEvent to trigger the internal logic
            await _cliMonitor.TrackRegistryEvent(registryData, "Set Value");

            // Expected values for comparison
            string expectedProcessName = "powershell"; // As an example if you're mocking GetProcessNameById
            string expectedAction = "Set Value";
            string expectedRegistryPath = "HKEY_LOCAL_MACHINE\\Software\\Test";

            // Assert: Verify that the event was processed correctly
            // Assuming _cliMonitor stores processed events in _registrySummary (you need to adjust this according to your actual implementation)

            Assert.IsTrue(_cliMonitor.RegistrySummary.ContainsKey(expectedRegistryPath), "The registry path was not correctly processed.");

            // Retrieve the processed registry summary entry
            var registrySummary = _cliMonitor.RegistrySummary[expectedRegistryPath];

            Assert.AreEqual(expectedProcessName, registrySummary.ProcessName, "The process name was not processed correctly.");
            Assert.AreEqual(1, registrySummary.SetValueCount, "The Set Value action count was incorrect.");
            Assert.AreEqual(1234, registrySummary.ProcessID, "The process ID was not processed correctly.");
        }

    }



}
