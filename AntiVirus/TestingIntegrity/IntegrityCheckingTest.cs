using DatabaseFoundations;
using IntegrityModule.ControlClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingIntegrity
{
    public class IntegrityCheckingTest
    {
        private IntegrityManagement _integrityManagement;
        private IntegrityDatabaseIntermediary _integDatabase;
        private string _fileProvided;
        [SetUp]
        public void Setup()
        {
            _integDatabase = new("IntegrityDatabase", true);
            _integrityManagement = new(_integDatabase);
            _fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\TestingIntegrity\\hashExample.txt";
        }

        [TearDown]
        public void TearUp()
        {
            _integDatabase = null;
        }

        [Test]
        public void DeleteAllGetAmount()
        {
            _integrityManagement.ClearDatabase();
            _integrityManagement.AddBaseline(_fileProvided);
            Assert.That(_integDatabase.QueryAmount("IntegrityCheck"), Is.GreaterThanOrEqualTo(1));
            _integDatabase.DeleteAll();
            Assert.That(_integDatabase.QueryAmount("IntegrityCheck"), Is.EqualTo(0));
        }
    }
}
