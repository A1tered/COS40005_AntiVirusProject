// See https://aka.ms/new-console-template for more information


using FindTheHash;
using System.Diagnostics;
string directorySearch = "C:\\Users\\yumcy\\AppData\\Local\\Discord";
DirectoryManager directoryManager = new DirectoryManager();
string databaseDirectory = directoryManager.getDatabaseDirectory("hashDatabase");
Console.WriteLine($"Database Directory Found: {databaseDirectory}");

SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
splitprocessInstance.fillUpSearch(directorySearch);
Stopwatch stopwatch = new();
stopwatch.Start();
splitprocessInstance.SearchDirectory();
stopwatch.Stop();
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"It took {stopwatch.Elapsed} to search the directory provided");
Console.ResetColor();

//DatabaseConnector databaseConnect = new(databaseDirectory);
//Console.WriteLine(databaseConnect.QueryHash("4E06DB09B8C3769968C3D0B51D7CF7470FDBA1AAF32DECF49DBD923708F86AE7"));
//4E06DB09B8C3769968C3D0B51D7CF7470FDBA1AAF32DECF49DBD923708F86AE7
//string hashFile = new Hasher().OpenHashFile("C:\\Users\\yumcy\\AppData\\Local\\Discord\\Update.exe"); // Discord file
//Console.WriteLine(hashFile);