using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services.Interface;

namespace TestingIntegrity.DummyClasses
{
    public class SetupServiceDummy : ISetupService
    {
        public SetupServiceDummy()
        {

        }

        public static ISetupService GetInstance(IServiceProvider serviceSet, bool testingMode)
        {
            return new SetupServiceDummy();
        }

        public static ISetupService GetExistingInstance()
        {
            return new SetupServiceDummy();
        }

        public void AddToConfig(string key, int value)
        {
            
        }

        public void TestingMode()
        {

        }

        public int GetFromConfig(string key)
        {
            return 0;
        }

        public async Task UpdateConfig()
        {
            await Task.CompletedTask;
        }

        public async Task<bool> Run()
        {
            await Task.FromResult<bool>(true);
            return true;
        }

        public async Task<bool> FirstRun()
        {
            await Task.FromResult<bool>(true);
            return true;    
        }

        public bool GeneralRun()
        {
            return true;
        }

        public static void TransferContents(SqliteConnection sqliteConnection, string initialisationFolder, string fillerdatabasename, string table)
        {

        }

        public string DbKey()
        {
            return "";
        }

        public bool FirstTimeRunning
        {
            get
            {
                return true;
            }
        }

        public bool ProgramCooked
        {
            get
            {
                return false;
            }
        }

    }
}
