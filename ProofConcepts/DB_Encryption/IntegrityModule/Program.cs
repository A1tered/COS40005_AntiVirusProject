/**************************************************************************
 * File:        [FileName].cs
 * Author:      [Name]
 * Description: [Brief description of what the file does]
 * Last Modified: [Date]
 * Libraries:   [Location Libraries / Dependencies]
 **************************************************************************/


using DatabaseFoundations;
using IntegrityModule.ControlClasses;

namespace IntegrityModule;

class Program
{
    public static void Main(string[] args)
    {
        string testKey = "testKey";
        IntegrityDatabaseIntermediary inter = new IntegrityDatabaseIntermediary("integrity_database", false, testKey);
       IntegrityManagement integrityModule = new IntegrityManagement(inter);
        //integrityModule.ClearDatabase();
        //integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\SmallerIntegrityCheckedFiles", true);
        //integrityModule.Scan(true);
        //integrityModule.RemoveBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
        List<List<string>> outGet = inter.QueryReaderAsText(new Microsoft.Data.Sqlite.SqliteCommand("SELECT * FROM IntegrityTrack"));
        foreach (List<string> lineSet in outGet)
        {
            foreach (string item in lineSet)
            {
                Console.Write($"{item},");
            }
            Console.Write("\n");
        }
        integrityModule.Close();
        Console.ReadLine(); // Remove when merging
    }
}