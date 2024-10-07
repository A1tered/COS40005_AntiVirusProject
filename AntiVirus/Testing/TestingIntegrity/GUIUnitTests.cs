/**************************************************************************
 * File:        GUIUnitTests.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with functions that are closest to GUI.
 * Last Modified: 8/10/2024
 **************************************************************************/

using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;
namespace TestingIntegrity
{
    public class GUIUnitTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        /// <summary>
        /// Tests that SetUpService recognises that the test structure does not meet the project structure.
        /// </summary>
        [Test]
        public async Task SetupServiceTest()
        {
            ISetupService setupService = SetupService.GetInstance(null, true);
            bool getResult = await setupService.Run();
            Assert.That(setupService.ProgramCooked, Is.True);
        }
    }
}
