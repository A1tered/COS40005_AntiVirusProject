/**************************************************************************
* File:        FileHashScanner
* Author:      Christopher Thompson & Joel Parks
* Description: The main program file for the file hash scanner.
* Last Modified: 13/08/2024
* Libraries:   [Location Libraries / Dependencies]
**************************************************************************/

using SimpleAntivirus.FileHashScanning;
using SimpleAntivirus.GUI.ViewModels.Pages;
using SimpleAntivirus.GUI.Views.Pages;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Controls;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.FileQuarantine;


/// <summary>
/// The File Hash Scanner is used to run scans comparing hashes of files in the given scanning directories and searching the hash database for matches.
/// If a match is found, it is marked as a threat and the file is quarantined and an alert is raised.
/// </summary>
namespace SimpleAntivirus.FileHashScanning
{
    public class FileHashScanner
    {
        public AlertManager AlertManager;
        public EventBus EventBus;
        public CancellationToken Token;
        public QuarantineManager QuarantineManager;

        public FileHashScanner(AlertManager alertManager, EventBus eventBus, CancellationToken token, QuarantineManager quarantineManager)
        {
            EventBus = eventBus;
            AlertManager = alertManager;
            Token = token;
            QuarantineManager = quarantineManager;
        }

        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("sighash.db");

        public async Task Scan(string scanType, List<string> customScanDirs)
        {
            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange
                    ([
                     $"C:\\Program Files",
                     "C:\\Program Files (x86)",
                     "C:\\ProgramData",
                     "C:\\Users\\Default\\AppData",
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData"),
                     "C:\\Windows",
                     Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                     Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                     Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Startup")
                    ]);
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
                    if (customScanDirs != null && customScanDirs.Count > 0)
                    {
                        foreach (string dir in customScanDirs)
                        {
                            Debug.WriteLine($"Currently added dir: {dir}");
                            directories.Add(dir);
                        }
                    }
                }

                foreach (string directorySearch in directories)
                {
                    Token.ThrowIfCancellationRequested();
                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory, this, Token);
                    await splitprocessInstance.fillUpSearch(directorySearch);
                    await splitprocessInstance.SearchDirectory(this);
                }
            });
        }
    }
}