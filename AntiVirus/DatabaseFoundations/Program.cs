/**************************************************************************
 * File:        [FileName].cs
 * Author:      [Name]
 * Description: [Brief description of what the file does]
 * Last Modified: [Date]
 * Libraries:   [Location Libraries / Dependencies]
 **************************************************************************/

using DatabaseFoundations;

class Program
{
    static void Main(string[] args)
    {
        IntegrityDatabaseIntermediary integrityDatabaseIntermediary = new("IntegrityDatabase", true);
/*        integrityDatabaseIntermediary.DeleteAll();
        //integrityDatabaseIntermediary.AddEntry(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\Archive");
        Dictionary<string, string> dict = integrityDatabaseIntermediary.GetSetEntries(1, 100);
        Console.WriteLine("Pair debug:");
        foreach (KeyValuePair<string, string> pair in dict)
        {
            Console.WriteLine(pair);
        }*/
    }
}
