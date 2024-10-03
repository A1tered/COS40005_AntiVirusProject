
using NUnit.Framework;
using SimpleAntivirus.Alerts;
using System.Threading.Tasks;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class EventBusTests
    {
        private EventBus _eventBus;
        private AlertManager _alertManager;

        [SetUp]
        public void Setup()
        {
            _alertManager = new AlertManager();
            _eventBus = new EventBus(_alertManager);
        }

        [Test]
        public void InitializationTest()
        {
            Assert.IsNotNull(_eventBus, "EventBus initialize correctly.");
            Assert.IsNotNull(_alertManager, "AlertManager initialized.");
        }

        [Test]
        public async Task PublishAsyncTest()
        {
            await _alertManager.ClearDatabase(); // Clear any existing alerts to ensure a clean test
            await _eventBus.PublishAsync("EventComponent", "Warning", "EventBus test message", "Action needed");

            var alerts = await _alertManager.GetAllAlertsAsync();

            Assert.IsNotNull(alerts, "Alerts list should not be null.");
            Assert.IsTrue(alerts.Exists(a => a.Component == "EventComponent" && a.Message == "EventBus test message"),"Alert published via EventBus.");
        }
    }

    [TestFixture]
    public class AlertManagerTests
    {
        private AlertManager _alertManager;

        [SetUp]
        public void Setup()
        {
            _alertManager = new AlertManager();
        }

        [Test]
        public void InitializationTest()
        {
            Assert.IsNotNull(_alertManager, "AlertManager initialize correctly.");
        }

        [Test]
        public async Task StoreAlertAsyncTest()
        {
            var alert = new Alert("ComponentTest", "Medium", "Test Store Alert", "Action Test");
            await _alertManager.StoreAlertAsync(alert);
            var alerts = await _alertManager.GetAllAlertsAsync();

            Assert.IsTrue(alerts.Exists(a => a.Component == "ComponentTest" && a.Message == "Test Store Alert"),"Alert stored successfully in the database.");
        }

        [Test]
        public async Task CheckForIdenticalAlertInTimeFrameTest()
        {
            var alert = new Alert("ComponentTest", "Low", "Identical Alert Test", "Action Test");
            await _alertManager.StoreAlertAsync(alert);
            var result = await _alertManager.CheckForIdenticalAlertInTimeFrame(alert, 120);

            Assert.IsTrue(result, "Alert found within the specified time frame.");
        }

        [Test]
        public async Task ClearDatabaseTest()
        {
            await _alertManager.ClearDatabase();
            var alerts = await _alertManager.GetAllAlertsAsync();
            Assert.That(alerts.Count, Is.EqualTo(0), "Database cleared of all alerts.");
        }

        [Test]
        public async Task GetAlertsWithinPastTimeFrameTest()
        {
            await _alertManager.ClearDatabase();
            var alert = new Alert("ComponentTest", "Critical", "TimeFrame Test", "Action Test");
            await _alertManager.StoreAlertAsync(alert);
            var count = await _alertManager.GetAlertsWithinPastTimeFrame(120);

            Assert.GreaterOrEqual(count, 1, "There is at least one alert in the time frame.");
        }
    }
}
