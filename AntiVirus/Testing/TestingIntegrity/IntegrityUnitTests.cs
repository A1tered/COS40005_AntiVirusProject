/**************************************************************************
 * File:        IntegrityUnitTests.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with tests related to Integrity that are unit tests (isolated via dummy classes)
 * Last Modified: 8/10/2024
 **************************************************************************/

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

        /// <summary>
        /// This test tests IntegrityDataPooler and its check integrity function.
        /// </summary>
        [Test]
        public async Task CheckIntegrityTest()
        {
            IntegrityDataPooler poolerItem = new(new IntegrityDatabaseIntermediaryDummy("IntegrityTrack", true), 1, 100);

            List<IntegrityViolation> listGet = await poolerItem.CheckIntegrity(new CancellationToken());
            Assert.That(listGet, Has.Exactly(1).Items);
        }

        /// <summary>
        ///  This test checks IntegrityDataPooler and its check integrity directory function.
        /// </summary>
        [Test]
        public async Task CheckIntegrityDirectoryTest()
        {
            IntegrityDataPooler poolerItem = new(new IntegrityDatabaseIntermediaryDummy("IntegrityTrack", true), "");
            List<IntegrityViolation> listGet = await poolerItem.CheckIntegrityDirectory();
            Assert.That(listGet, Has.Exactly(1).Items);
        }


        /// <summary>
        /// This test checks IntegrityCycler and its ability to initiatescans.
        /// </summary>
        [Test]
        public async Task CyclerInitiateScanTest()
        {
            IntegrityCycler cyclerObject = new(new IntegrityDatabaseIntermediaryDummy("", true), new ViolationHandlerDummy());
            cyclerObject.SetPoolerType(typeof(IntegrityDataPoolerDummy));
            List<IntegrityViolation> violationlist = await cyclerObject.InitiateScan();
            Assert.That(violationlist, Has.Exactly(1).Items);
        }

        /// <summary>
        /// This is a primitive test to check that ReactiveControl prevents overlap by locking itself once run once.
        /// </summary>
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
