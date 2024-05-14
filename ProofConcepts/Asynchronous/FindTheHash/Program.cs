// See https://aka.ms/new-console-template for more information


using FindTheHash;
using System.Diagnostics;
// Directory to search, should work with any folder.
string directorySearch = "C:\\Users\\yumcy\\AppData\\Local\\Discord";
// True => Enable Asynchronous Search, False => Synchronous Search. 
bool enableAsync = true;

DirectoryManager directoryManager = new DirectoryManager();
// Get directory to database.
string databaseDirectory = directoryManager.getDatabaseDirectory("hashDatabase");
Console.WriteLine($"Database Directory Found: {databaseDirectory}");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"Starting search in directory: {directorySearch}");
Console.ForegroundColor = ConsoleColor.White;
SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
// Simply creates the initial directories to unpack.
splitprocessInstance.fillUpSearch(directorySearch);
Stopwatch stopwatch = new();
stopwatch.Start();
splitprocessInstance.SearchDirectory(enableAsync); 
stopwatch.Stop();
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine($"It took {stopwatch.Elapsed} to search the directory provided");
Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine($"Program searched {splitprocessInstance.DirectoriesSearched} directories.");
Console.ResetColor();