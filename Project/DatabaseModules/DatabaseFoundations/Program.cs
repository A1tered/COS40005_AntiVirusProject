// See https://aka.ms/new-console-template for more information
using DatabaseFoundations;

IntegrityDatabaseIntermediary integrityDatabaseIntermediary = new("IntegrityDatabase", false);
integrityDatabaseIntermediary.AddEntry(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
Dictionary<string, string> dict = integrityDatabaseIntermediary.GetSetEntries(1, 100);
Console.WriteLine("Pair debug:");
foreach (KeyValuePair<string, string> pair in dict)
{
    Console.WriteLine(pair);
}