/**************************************************************************
* File:        Program.cs
* Author:      Christopher Thompson & Joel Parks
* Description: The main program file for the file hash scanner.
* Last Modified: 13/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
// Add other necessary using directives here

namespace FindTheHash
{
    public class Program
    {
        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");

        static async Task Main(string[] args)
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
            await MainMenu();
        }

        public static async Task MainMenu()
        {
            Console.WriteLine("Simple Anti-Virus: File Hash Checker");
            Console.WriteLine("Please choose an option\n");
            Console.WriteLine("1. Quick Scan");
            Console.WriteLine("2. Full Scan");
            Console.WriteLine("3. Custom Scan");
            Console.WriteLine("4. Quit");
            Console.WriteLine("\r\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    await Scan("quick");
                    break;
                case "2":
                    Console.Clear();
                    await Scan("full");;
                    break;
                case "3":
                    Console.Clear();
                    await Scan("custom");
                    break;
                case "4":
                    Console.WriteLine("Program exiting.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Error: No option selected.");
                    await MainMenu();
                    break;
            }
        }

        private static async Task Scan(string scanType)
        {
            await Task.Run(async () =>
            {
                Console.WriteLine($"Database Directory Found: {databaseDirectory}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange([$"C:\\Program Files\\TestDirectory"]);
                }
                else if (scanType == "full")
                {
                    string[] drives = Environment.GetLogicalDrives();
                    foreach (string drive in drives)
                    {
                        directories.Add(drive);
                    }
                }
                else if (scanType == "custom")
                {
                    Console.WriteLine("Not implemented");
                }
                foreach (string directorySearch in directories)
                {
                    // State the directory that the search is beginning in 
                    Console.WriteLine($"Starting search in directory: {directorySearch}");
                    Console.ForegroundColor = ConsoleColor.White;

                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
                    await splitprocessInstance.fillUpSearch(directorySearch);
                    Stopwatch stopwatch = new();
                    stopwatch.Start();
                    await splitprocessInstance.SearchDirectory();
                    stopwatch.Stop();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"It took {stopwatch.Elapsed} to search the directory provided. Total items scanned: <Placeholder>");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            });
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            await MainMenu();
        }          
    }
}

