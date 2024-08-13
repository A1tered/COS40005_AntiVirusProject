/**************************************************************************
* File:        Program.cs
* Author:      Christopher Thompson & Joel Parks
* Description: The main program file for the file hash scanner.
* Last Modified: 13/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.Diagnostics;
// Add other necessary using directives here

namespace FindTheHash
{
    public class Program
    {
        DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");
        // True => Enable Asynchronous Search, False => Synchronous Search. 
        bool enableAsync = true;
        // Triggers whether scan is done, or configuration code BELOW
        bool run = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Simple Anti-Virus: File Hash Checker");
            Console.WriteLine("INCOMPLETE: This scanner is for use for development purposes only.");
            Console.WriteLine("Your experience with this console app is not indicative of the final product");
            Console.WriteLine("Bugs, crashes, and other instability or unexpected errors may occur.");
            Console.WriteLine("Do you agree to the above terms of use? Please enter Y to agree.");
            while (Console.ReadLine() != "Y".ToLower())
            {
                Console.WriteLine("You must agree to the terms of use to continue.\n");
            }
            Console.Clear();
            MainMenu();
        }

        public static void MainMenu()
        {
            Console.WriteLine("Simple Anti-Virus: File Hash Checker");
            Console.WriteLine("Please choose an option\n");
            Console.WriteLine("1. Get File Hash");
            Console.WriteLine("2. Scan");
            Console.WriteLine("3. Quit");
            Console.WriteLine("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Hasher _hasher = new Hasher();
                    Console.Clear();
                    FileStream path = Hasher.OpenFile();
                    if (path != null)
                    {
                        // Get hash
                        string hash = _hasher.HashFile(path);
                        string file = Path.GetFileName(path.Name);
                        Console.Clear();
                        Console.WriteLine($"The hash of '{file}' is: {hash}\n");
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Error: Invalid or no file selected.\r\n");
                    }
                    MainMenu();
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Scan");
                    Console.WriteLine("Please choose the type of scan you wish to perform");
                    Console.WriteLine("1. Quick scan");
                    Console.WriteLine("2. Full scan");
                    Console.WriteLine("")
                    Console.WriteLine("3. Quit");
                    Console.WriteLine("\r\nSelect an option: ");

                    break;
                case "3":
                    Console.WriteLine("Program exiting.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Error: No option selected.");
                    MainMenu();
                    break;
            }
    }
}
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

