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

namespace FileHashScanning
{
    public class Program
    {
        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");

        static async Task Main(string[] args)
        {
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
            Stopwatch stopwatch = new();
            stopwatch.Start();

            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange([$"C:\\TestDirectory"]);
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
                    Console.ForegroundColor = ConsoleColor.White;

                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
                    await splitprocessInstance.fillUpSearch(directorySearch);
                    await splitprocessInstance.SearchDirectory();
                }
            });
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"It took {stopwatch.Elapsed} to complete the scan.");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            await MainMenu();
        }
    }
}

