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
        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");
        // True => Enable Asynchronous Search, False => Synchronous Search. 
        bool enableAsync = true;

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
                    ChooseScan();
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

        private static void ChooseScan()
        {
            Console.WriteLine("Please choose the type of scan you wish to perform");
            Console.WriteLine("1. Quick scan");
            Console.WriteLine("2. Full scan");
            Console.WriteLine("3. Custom scan");
            Console.WriteLine("4. Back");
            Console.WriteLine("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    Scan("quick");
                    break;
                case "2":
                    Console.Clear();
                    Scan("full");
                    break;
                case "3":
                    Console.Clear();
                    Scan("custom");
                    break;
                case "4":
                    Console.Clear();
                    MainMenu();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Error: No option selected.");
                    ChooseScan();
                    break;
            }
        }

        private static void Scan(string scanType)
        {

            Console.WriteLine($"Database Directory Found: {databaseDirectory}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            List<string> directories = new List<string>();
            
            if (scanType == "quick")
            {
                directories.AddRange([$"C:\\Program Files", "C:\\Program Files (x86)", "C:\\ProgramData", "C:\\Users\\Default\\AppData", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData"), "C:\\Windows"]);
            }
            else if (scanType == "full")
            {
                directories.Add("C:\\");
            }
            else if (scanType == "custom")
            {
                Console.WriteLine("Not implemented");
            }
                foreach (string directorySearch in directories)
                {
                    // Print the current database directory and 
                    Console.WriteLine($"Starting search in directory: {directorySearch}");
                    Console.ForegroundColor = ConsoleColor.White;

                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
                    splitprocessInstance.fillUpSearch(directorySearch);
                    // Simply creates the initial directories to unpack.
                    Stopwatch stopwatch = new();
                    stopwatch.Start();
                    splitprocessInstance.SearchDirectory(true);
                    stopwatch.Stop();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"It took {stopwatch.Elapsed} to search the directory provided");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            Console.ForegroundColor= ConsoleColor.White;
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            MainMenu();
        }          
    }
}

