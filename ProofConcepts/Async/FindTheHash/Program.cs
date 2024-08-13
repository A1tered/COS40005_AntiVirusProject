// See https://aka.ms/new-console-template for more information


using FindTheHash;
using System.Diagnostics;

DirectoryManager directoryManager = new DirectoryManager();
// Get directory to database.
string databaseDirectory = directoryManager.getDatabaseDirectory("SigHashDB.db");
// Directory to search, should work with any folder.
string directorySearch = "C:\\";
// True => Enable Asynchronous Search, False => Synchronous Search. 
bool enableAsync = true;
// Triggers whether scan is done, or configuration code BELOW
bool run = true;
if (run)
{
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
}
else
{
    Console.WriteLine("Running configuration options...");
    // For adding entries into SQLite database, only needs to be ran once for entries u want to add, and u can turn run back to false.
    Configurator configObject = new Configurator(databaseDirectory);
    configObject.AddHashToDatabase(@"C:\Users\yumcy\AppData\Local\Android\Sdk\system-images\android-34\google_apis\x86_64\NOTICE.txt");
}

