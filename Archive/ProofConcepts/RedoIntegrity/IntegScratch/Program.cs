// See https://aka.ms/new-console-template for more information
using IntegrityCheckingFromScratch;
using System.Diagnostics;



IntegrityManagement integrityManagement = new();
/*Console.WriteLine(new IntegrityDatabaseIntermediary("IntegrityDatabase").RequestDebugDisplay(@"SELECT * FROM IntegrityTrack LIMIT 100 OFFSET 0"));
Console.WriteLine(new IntegrityDatabaseIntermediary("IntegrityDatabase").RequestDebugDisplay(@"SELECT * FROM IntegrityTrack LIMIT 200 OFFSET 100"));*/
// Measure Scanning
Stopwatch stopwatch = new();
stopwatch.Start();
integrityManagement.Scan();
stopwatch.Stop();
Console.WriteLine($"Scanning database took {stopwatch.Elapsed}");

/***
CREATE TABLE IntegrityTrack(directory text primary key, hash text, modificationTime int, signatureCreation int,
originalSize int);
***/
/*
Console.WriteLine(integrityManagement.ClearDatabase());*/
//integrityManagement.AddBaseline(@"C:\Users\yumcy\OneDrive\Desktop\ProgrammingFiles\sqliteTools\TestSizeDatabase");
// Database test

Console.WriteLine(new IntegrityDatabaseIntermediary("IntegrityDatabase").RequestDebugDisplay());
/*Console.WriteLine("Populating database...");
string[] directoryBatch = Directory.GetFiles(@"C:\Users\yumcy\OneDrive\Desktop\UniversitySubjects\COS40006 Computing Technology Project B\TestingGround\IntegrityCheckedFiles");
int progress = 0;
int maxProgress = directoryBatch.Count();
foreach (string path in directoryBatch)
{
    Console.Write($"\rDatabase {progress}/{maxProgress}");
    progress++;
    integrityManagement.AddBaseline(path);
}*/

/*List<List<string>> tableFabricator = new();
tableFabricator.Add(new List<string>() { "Field1", "Field2", "Field3" });
tableFabricator.Add(new List<string>() { "value", "val22222ue2", "value3" });
tableFabricator.Add(new List<string>() { "value4", "value5", "value6" });
Console.WriteLine(DebugTableDisplayHandler.StringListToStringDisplay(tableFabricator));*/