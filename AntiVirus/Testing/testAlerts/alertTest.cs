using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.GUI.Services;



namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class AlertTests
    {
        private Alert _alert;

        [SetUp]
        public void Setup()
        {
            // Initialize Alert instance
            _alert = new Alert("TestComponent", "High", "Test message", "Suggested action");
        }

        [Test]
        public void AlertInitializationTest()
        {
            Assert.AreEqual("TestComponent", _alert.Component);
            Assert.AreEqual("High", _alert.Severity);
            Assert.AreEqual("Test message", _alert.Message);
            Assert.AreEqual("Suggested action", _alert.SuggestedAction);
            Assert.IsInstanceOf<DateTime>(_alert.Timestamp);
        }

        [Test]
        public void DisplayAlertWithoutAggregationTest()
        {
            // Test that the DisplayAlert method works correctly without aggregation
            _alert.DisplayAlert();
            Assert.Pass("Alert displayed successfully without aggregation.");
        }

        [Test]
        public void DisplayAlertWithAggregationTest()
        {
            // Test that the DisplayAlert method works correctly with aggregation
            _alert.DisplayAlert(true, 5);
            Assert.Pass("Alert displayed successfully with aggregation.");
        }
    }
}
