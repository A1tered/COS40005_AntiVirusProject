using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;
using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Db;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using SimpleAntivirus.IntegrityModule.Reactive;
using TestingIntegrity.DummyClasses;
namespace TestingIntegrity
{
    public class GUIUnitTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task SetupServiceTest()
        {
            ISetupService setupService = SetupService.GetInstance(null, true);
            bool getResult = await setupService.Run();
            Assert.That(setupService.ProgramCooked, Is.True);
        }
    }
}
