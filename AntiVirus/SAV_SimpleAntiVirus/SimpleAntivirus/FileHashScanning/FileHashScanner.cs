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
// Add other necessary using directives here

namespace SimpleAntivirus.FileHashScanning
{
    public class FileHashScanner
    {
        private long _totalSize;
        private long _currentSize;
        private double _progress;

        static DirectoryManager directoryManager = new DirectoryManager();
        // Get directory to database.
        static string databaseDirectory => directoryManager.getDatabaseDirectory("SigHashDB.db");

        static ScanningViewModel _viewModel = new();
        ScanningPage _scanningPage = new ScanningPage(_viewModel);

        public async Task Scan(string scanType)
        {
            _scanningPage.percentComplete.Text = $"10% complete";
            await Task.Run(async () =>
            {
                List<string> directories = new List<string>();

                if (scanType == "quick")
                {
                    directories.AddRange([$"C:\\TestDirectory", "C:\\Users\\CardmanOfficial\\Documents"]);
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

                _totalSize = await CalculateTotalSize(directories);

                foreach (string directorySearch in directories)
                {
                    SplitProcess splitprocessInstance = new SplitProcess(databaseDirectory, this);
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
                    string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    
                }
                catch (IOException)
                {
                    
                }
            }
            return await Task.FromResult(totalSize);
        }

        public void UpdateSize(long size)
        {
            _currentSize += size;
            _progress = (_currentSize / (double)_totalSize) * 100;
        }

        public void UpdateProgress()
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    _scanningPage.progressBar.Value = _progress;
            //    _scanningPage.percentComplete.Text = $"{_progress}% complete";
            //});
        }
    }
}