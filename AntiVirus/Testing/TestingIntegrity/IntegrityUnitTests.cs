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
    public class IntegrityUnitTests
    {
        private IntegrityDatabaseIntermediary _integData;
        private string fileProvided;
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task CheckIntegrityTest()
        {
            IntegrityDataPooler poolerItem = new(new IntegrityDatabaseIntermediaryDummy("IntegrityTrack", true), 1, 100);

            List<IntegrityViolation> listGet = await poolerItem.CheckIntegrity(new CancellationToken());
            Assert.That(listGet, Has.Exactly(1).Items);
        }

        [Test]
        public async Task CheckIntegrityDirectoryTest()
        {
            IntegrityDataPooler poolerItem = new(new IntegrityDatabaseIntermediaryDummy("IntegrityTrack", true), "");
            List<IntegrityViolation> listGet = await poolerItem.CheckIntegrityDirectory();
            Assert.That(listGet, Has.Exactly(1).Items);
        }


        // Cycler

        [Test]
        public async Task CyclerInitiateScanTest()
        {
            IntegrityCycler cyclerObject = new(new IntegrityDatabaseIntermediaryDummy("", true), new ViolationHandlerDummy());
            cyclerObject.SetPoolerType(typeof(IntegrityDataPoolerDummy));
            List<IntegrityViolation> violationlist = await cyclerObject.InitiateScan();
            Assert.That(violationlist, Has.Exactly(1).Items);
        }

        // ViolationHandler

        [Test]
        public void ReactiveControlLockup()
        {
            IntegrityDatabaseIntermediaryDummy dummyIntegInter = new("IntegrityTrack", true);
            ReactiveControl reactiveControlSetup = new(dummyIntegInter, new IntegrityCyclerDummy(dummyIntegInter, new ViolationHandlerDummy()));
            Assert.That(reactiveControlSetup.Initialize(), Is.True);
            Assert.That(reactiveControlSetup.Initialize(), Is.False);
        }
    }
}
