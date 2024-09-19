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
        private ProgressTracker _progressTracker;
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
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");

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
                        foreach(string dir in customScanDirs)
                        {
                            Debug.WriteLine($"Currently added dir: {dir}");
                            directories.Add(dir);
                        }
                    }
                    else
                    {
                        MessageBox.Show("List of files and folders to scan cannot be empty.", "Simple Antivirus", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task<long> CalculateTotalSize(List<string> directories)
        {
            long totalSize = 0;

            foreach (string directory in directories)
            {
                try
                {
                    // The EnumerationOptions ensure that any files or directories that cannot be accessed 
                    EnumerationOptions options = new EnumerationOptions
                    {
                        IgnoreInaccessible = true, // Ignores folders/files that cannot be accessed
                        RecurseSubdirectories = true, // Recursively access subdirectories
                        AttributesToSkip = FileAttributes.ReparsePoint // Skip symbolic links/junctions
                    };

                    foreach (string file in Directory.EnumerateFiles(directory, "*", options))
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            totalSize += fileInfo.Length;
                        }
                        catch (UnauthorizedAccessException uaEx)
                        {
                            Debug.WriteLine($"Access denied to file: {file}. Error: {uaEx.Message}");
                        }
                        catch (IOException ioEx)
                        {
                            Debug.WriteLine($"I/O error with file: {file}. Error: {ioEx.Message}");
                        }
                    }
                }
                catch (UnauthorizedAccessException uaEx)
                {
                    Debug.WriteLine($"Access denied to directory: {directory}. Error: {uaEx.Message}");
                }
                catch (IOException ioEx)
                {
                    Debug.WriteLine($"I/O error with directory: {directory}. Error: {ioEx.Message}");
                }
            }

            Debug.WriteLine($"Total Size Calculated: {totalSize} bytes");

            _progressTracker = new ProgressTracker(totalSize);

            return await Task.FromResult(totalSize);
        }

        public void UpdateSize(long size)
        {
            // Update current progress on tracker
            _progressTracker?.UpdateTracker(size);
        }

        public void UpdateProgress()
        {

        }
        //Application.Current.Dispatcher.Invoke(() =>
        //{
        //    _scanningPage.progressBar.Value = _progress;
        //    _scanningPage.percentComplete.Text = $"{_progress}% complete";
        //});
    }
}