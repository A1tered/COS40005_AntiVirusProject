using NUnit.Framework;
using SimpleAntivirus;
using SimpleAntivirus.Alerts;
using System;

namespace SimpleAntivirus.Tests
{
    [TestFixture]
    public class AlertTests
    {
        [Test]
        public void Alert_Creation_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            string component = "File Scanner";
            string severity = "High";
            string message = "Malware detected";
            string suggestedAction = "Quarantine the file";

            // Act
            var alert = new Alert(component, severity, message, suggestedAction);

            // Assert
            Assert.AreEqual(component, alert.Component);
            Assert.AreEqual(severity, alert.Severity);
            Assert.AreEqual(message, alert.Message);
            Assert.AreEqual(suggestedAction, alert.SuggestedAction);
            Assert.That(alert.Timestamp, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
        }
    }
}
