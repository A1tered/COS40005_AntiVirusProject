

using SimpleAntivirus.IntegrityModule.DataRelated;

namespace TestingIntegrity 
{
    public class FileInfoRequesterTest
    {
        private string fileProvided;

        [SetUp]
        public void Setup()
        {
            fileProvided = "C:\\Users\\yumcy\\OneDrive\\Desktop\\Github Repositories\\Technology Project A\\COS40005_AntiVirusProject\\AntiVirus\\Testing\\TestingIntegrity\\hashExample.txt";
        }

        [Test]
        public async Task HashFile()
        {

            //99709748FD9E3F995FB24E129974FE1A68811217
            Assert.That(await FileInfoRequester.HashFile(fileProvided), Is.EqualTo("99709748FD9E3F995FB24E129974FE1A68811217"));
        }

        [Test]
        public void SizeLabelTest()
        {

            //99709748FD9E3F995FB24E129974FE1A68811217
            Assert.That(FileInfoRequester.SizeValueToLabel(6442450944), Is.EqualTo("6 GB"));
        }

        [Test]
        public void RetrieveFileInfo()
        {
            Assert.That(FileInfoRequester.RetrieveFileInfo(fileProvided), Is.EqualTo(new Tuple<long, long>(1726814927, 17)));
        }
    }
}