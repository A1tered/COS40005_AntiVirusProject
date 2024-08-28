/**************************************************************************
 * File:        [FileName].cs
 * Author:      [Name]
 * Description: [Brief description of what the file does]
 * Last Modified: [Date]
 * Libraries:   [Location Libraries / Dependencies]
 **************************************************************************/


using DatabaseFoundations;
using DatabaseFoundations.IntegrityRelated;
using IntegrityModule.ControlClasses;
using IntegrityModule.DataTypes;
using IntegrityModule.Debug;
using IntegrityModule.IntegrityComparison;
using IntegrityModule.Reactive;
using Microsoft.Data.Sqlite;

namespace IntegrityModule;

class Program
{
    public static async Task Main(string[] args)
    {
        //IntegrityDatabaseIntermediary databaseIntermediary = new("IntegrityDatabase", false);
        //IntegrityManagement integrityModule = new IntegrityManagement(databaseIntermediary);
        ////integrityModule.ClearDatabase();
        ////await integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles", true);
        //await integrityModule.Scan(true);
        ////integrityModule.RemoveBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");


        //integrityModule.ClearDatabase();
        //await integrityModule.AddBaseline(@"C:\\Users\\yumcy\\OneDrive\\Desktop\\UniversitySubjects\\COS40006 Computing Technology Project B\\TestingGround\\HundredIntegrityFiles");

        // Test Start
        //integrityModule.ClearDatabase();
        ////bool TestCheck = await databaseIntermediary.AddEntry(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles", 100);
        ////Console.WriteLine(TestCheck);
        //ReactiveControl reactiveControl = new(databaseIntermediary, new IntegrityCycler(databaseIntermediary, new Alerts.ViolationHandler()));
        ////Console.WriteLine(itemReturn);
        //reactiveControl.Initialize();
        //SqliteCommand commandTest = new();
        //commandTest.CommandText = "SELECT * FROM IntegrityTrack";
        //string output = DebugAssist.StringListToStringDisplay(databaseIntermediary.QueryReaderAsText(commandTest));
        ////Console.WriteLine(output);
        //Console.ReadLine(); // Remove when merging


    }
}