/**************************************************************************
* File:        FileHashScanner
* Author:      Christopher Thompson & Joel Parks
* Description: The main program file for the file hash scanner.
* Last Modified: 13/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using SimpleAntivirus.FileHashScanning;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
// Add other necessary using directives here

namespace SimpleAntivirus.FileHashScanning
{
    public class FileHashScanner
    {
        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");

        public async Task Scan(string scanType)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange([$"C:\\Users\\CardmanOfficial\\Documents"]);
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
                    // Console.WriteLine("Not implemented");
                }
                foreach (string directorySearch in directories)
                {

                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory);
                    await splitprocessInstance.fillUpSearch(directorySearch);
                    await splitprocessInstance.SearchDirectory();
                }
            });
            stopwatch.Stop();
            Console.WriteLine($"It took {stopwatch.Elapsed} to complete the scan.");
            // Navigate back to ScannerPage
        }
    }
}

