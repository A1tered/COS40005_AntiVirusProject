using DatabaseFoundations.IntegrityRelated;

namespace TestingIntegrity 
{
    public class FileInfoRequesterTest
    {
        private string fileProvided;

        [SetUp]
        public void Setup()
        {
            fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\TestingIntegrity\\hashExample.txt";
        }

        [Test]
        public void HashFile()
        {

            //99709748FD9E3F995FB24E129974FE1A68811217
            Assert.That(FileInfoRequester.HashFile(fileProvided), Is.EqualTo("99709748FD9E3F995FB24E129974FE1A68811217"));
        }

        [Test]
        public void RetrieveFileInfo()
        {
            Assert.That(FileInfoRequester.RetrieveFileInfo(fileProvided), Is.Not.Null);
        }
    }
}