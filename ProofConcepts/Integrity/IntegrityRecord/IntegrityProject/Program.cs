// See https://aka.ms/new-console-template for more information
// Proof of concept for Integrity Checking

using FindTheHash;
using IntegrityMarkRecordGetAccessRecords;
using System.Security.Principal;
Console.ForegroundColor = ConsoleColor.Blue;
DirectoryManager directoryManager = new();
string databaseDirectory = directoryManager.getDatabaseDirectory("integrityDatabase");
Console.ResetColor();
IntegrityManager integrityManager = new(databaseDirectory);

Console.WriteLine(WindowsIdentity.GetCurrent().Name);
bool initialIntegrityRun = true;
bool configAdd = false; // Whether to scan or just add entries.
if (!configAdd)
{

    // continually loop over
    while (true)
    {
        Thread.Sleep(1000);
        List<String> results = integrityManager.IntegrityCheck(initialIntegrityRun);
        initialIntegrityRun = false;
        if (results.Count > 0)
        {
            Console.Write("\n");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (string info in results)
            {
                Console.WriteLine(info);
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(".");
        }
        Console.ResetColor();
    }
}
else // If configadd, run the code here. (If integrity already exists, it updates entry).
{
    string[] directorySet = Directory.GetFiles(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\Cos40005 Technology Project A\TestingGrounds\IntegrityCheckedFiles");
    Console.WriteLine(directorySet.Count());
    foreach (string insideDirectory in directorySet)
    {
        integrityManager.AddIntegrity(insideDirectory);
    }
}
