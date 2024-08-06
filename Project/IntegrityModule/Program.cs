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
       IntegrityManagement integrityModule = new IntegrityManagement(new IntegrityDatabaseIntermediary("IntegrityDatabase", false));
        //integrityModule.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\SmallerIntegrityCheckedFiles");
        integrityModule.Scan();
    }
}