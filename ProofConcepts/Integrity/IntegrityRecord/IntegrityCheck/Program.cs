// See https://aka.ms/new-console-template for more information
// Proof of concept for Integrity Checking

using FindTheHash;
using IntegrityMarkRecordGetAccessRecords;
Console.ForegroundColor = ConsoleColor.Blue;
DirectoryManager directoryManager = new();
string databaseDirectory = directoryManager.getDatabaseDirectory("integrityDatabase");
Console.ResetColor();

IntegrityManager integrityManager = new(databaseDirectory);
List<String> results = integrityManager.IntegrityCheck();
if (results.Count > 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    foreach (string info in results)
    {
        Console.WriteLine(info);
    }
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("No integrity violaations found");
}
Console.ResetColor();
//integrityManager.AddIntegrity("C:\\Users\\yumcy\\OneDrive\\Desktop\\UniversitySubjects\\Cos40005 Technology Project A\\TestingGrounds\\IntegrityCheckedFiles\\picture.jpg");