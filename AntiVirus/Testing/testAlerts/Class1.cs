using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Services;



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
            // Initialize SetupService correctly
            SetupService.GetInstance(); // Ensure this call initializes SetupService properly

            // Create a new AlertManager and EventBus instance
            _alertManager = new AlertManager();
            _eventBus = new EventBus(_alertManager);
        }

        [Test]
        public void InitializationTest()
        {
            Assert.IsNotNull(_eventBus, "EventBus should initialize correctly.");
            Assert.IsNotNull(_alertManager, "AlertManager should be initialized.");
        }

        [Test]
        public async Task PublishAsyncTest()
        {
            // Clear the database to ensure a clean test environment
            await _alertManager.ClearDatabase();

            // Publish an alert using EventBus
            await _eventBus.PublishAsync("EventComponent", "Warning", "EventBus test message", "Action needed");

            // Retrieve all alerts from the database to verify
            var alerts = await _alertManager.GetAllAlertsAsync();

            Assert.IsNotNull(alerts, "Alerts list should not be null.");
            Assert.IsTrue(alerts.Exists(a => a.Component == "EventComponent" && a.Message == "EventBus test message"),
                          "Alert should be published via EventBus.");
        }
    }


}
