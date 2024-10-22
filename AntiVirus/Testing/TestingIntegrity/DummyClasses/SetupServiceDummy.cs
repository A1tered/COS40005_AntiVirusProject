using Microsoft.Data.Sqlite;
using SimpleAntivirus.GUI.Services;
using SimpleAntivirus.GUI.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Appearance;

namespace TestingIntegrity.DummyClasses
{
    public class SetupServiceDummy : ISetupService
    {
        public SetupServiceDummy()
        {

        }

        public static ISetupService GetInstance(bool testingMode)
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

        public void Y2k38Problem()
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

        public ApplicationTheme ApplicationTheme
        {
            get
            {
                return ApplicationTheme.Dark;
            }

        }



    }
}
