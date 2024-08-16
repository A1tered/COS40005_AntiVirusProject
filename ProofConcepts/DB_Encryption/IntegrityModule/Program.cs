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
       IntegrityManagement integrityModule = new IntegrityManagement(new IntegrityDatabaseIntermediary("IntegrityDatabase", false, testKey));
        integrityModule.ClearDatabase();
        integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles", true);
        integrityModule.Scan(true);
        //integrityModule.RemoveBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
        Console.ReadLine(); // Remove when merging
    }
}