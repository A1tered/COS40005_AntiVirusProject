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

namespace IntegrityModule;

class Program
{
    public static void Main(string[] args)
    {
        IntegrityManagement integrityModule = new IntegrityManagement(new IntegrityDatabaseIntermediary("IntegrityDatabase", false));
        integrityModule.ClearDatabase();
        integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\SmallerIntegrityCheckedFiles", true);
        integrityModule.Scan(true);
        //integrityModule.RemoveBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
        Console.ReadLine(); // Remove when merging
    }
}