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
using IntegrityModule.Debug;
using Microsoft.Data.Sqlite;

namespace IntegrityModule;

class Program
{
    public static void Main(string[] args)
    {
        IntegrityDatabaseIntermediary databaseIntermediary = new("IntegrityDatabase", false);
        IntegrityManagement integrityModule = new IntegrityManagement(databaseIntermediary);
        integrityModule.ClearDatabase();
        integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\SmallerIntegrityCheckedFiles", true);
        //integrityModule.Scan(true);
        SqliteCommand customSqliteCommand = new();
        customSqliteCommand.CommandText = "SELECT * FROM IntegrityTrack";
        List<List<string>> returnItem = (databaseIntermediary.QueryReaderAsText(customSqliteCommand));
        Console.WriteLine(DebugAssist.StringListToStringDisplay(returnItem));
        ////integrityModule.RemoveBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
        //Console.ReadLine(); // Remove when merging
        
        
    }
}