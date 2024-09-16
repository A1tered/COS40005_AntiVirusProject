/**************************************************************************
 * File:        IntegrityConfigurator.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with scans that are reactive, such that if a file change occurs that it scans that certain file for performance, and the reduction
 * of time taken to respond to issues.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule.Db;
using System.IO;

namespace SimpleAntivirus.IntegrityModule.Reactive
{
    public class ReactiveControl
    {
        private List<FileSystemWatcher> _fileWatcherList;
        private IntegrityDatabaseIntermediary _intermediaryDB;
        private IntegrityCycler _integrityCycler;
        private bool _reactiveInitialized;
        private List<string> _directoryTracker;
        // Prevent overlap (may result in detection failures, but better than alert failures)
        private bool _eventCallInProgress = false;
        public ReactiveControl(IntegrityDatabaseIntermediary intermediary, IntegrityCycler cycler)
        {
            _reactiveInitialized = false;
            _fileWatcherList = new();
            _directoryTracker = new();
            _intermediaryDB = intermediary;
            _integrityCycler = cycler;
        }

        public bool Initialize()
        {
            // Only initialise if havent before
            if (_reactiveInitialized == false)
            {
                _reactiveInitialized = true;
                System.Diagnostics.Debug.WriteLine("\n");
                System.Diagnostics.Debug.WriteLine("Reactive Control Initialization");
                System.Diagnostics.Debug.WriteLine("\n");
                long amountEntry = _intermediaryDB.QueryAmount();
                decimal divison = (decimal)amountEntry / 100;
                int sets = Convert.ToInt32(Math.Ceiling(divison));
                Dictionary<string, string> DirHashDirectory = new();
                for (int iterator = 0; iterator < sets; iterator++)
                {
                    DirHashDirectory = _intermediaryDB.GetSetEntries(iterator, 100);
                    foreach (KeyValuePair<string, string> dirHash in DirHashDirectory)
                    {
                        SetUpFileWatcher(dirHash.Key);
                    }
                }
                return true;
            }
            return false;
        }

        private void SetUpFileWatcher(string path)
        {
            if (_reactiveInitialized)
            {
                if (Path.Exists(path))
                {
                    string getDirectoryPath;
                    getDirectoryPath = Path.GetDirectoryName(path);
                    if (getDirectoryPath != null)
                    {
                        // Make sure the directory hasnt already been connected to a filewatcher.
                        if (!_directoryTracker.Exists(x => x == getDirectoryPath))
                        {
                            System.Diagnostics.Debug.WriteLine($"Attempted Event Connection: {getDirectoryPath}");
                            FileSystemWatcher fileWatcherTemp = new(getDirectoryPath);
                            fileWatcherTemp.EnableRaisingEvents = true;
                            fileWatcherTemp.Changed += IndividualScanEventHandler;
                            _fileWatcherList.Add(fileWatcherTemp);
                            _directoryTracker.Add(getDirectoryPath);
                        }
                    }
                }
            }
        }

        private async void IndividualScanEventHandler(object sender, FileSystemEventArgs eventArguments)
        {
            if (!_eventCallInProgress)
            {
                _eventCallInProgress = true;
                System.Diagnostics.Debug.WriteLine($"Item changed {eventArguments.FullPath}");
                await _integrityCycler.InitiateDirectoryScan(eventArguments.FullPath);
                _eventCallInProgress = false;
            }
        }

        public async Task Add(string path)
        {
            List<string> pathsToAdd = FileInfoRequester.PathCollector(path);
            await Task.Run(() =>
            {
                foreach (string pathItem in pathsToAdd)
                {
                    SetUpFileWatcher(pathItem);
                }
            });
        }

        public void Remove(string path)
        {
            foreach (FileSystemWatcher fileWatcher in _fileWatcherList)
            {
                // Remove all file watchers that equal given path.
                _fileWatcherList.RemoveAll(x => x.Path == path);
            }
        }

        public void RemoveAll()
        {
            _fileWatcherList.Clear();
        }
    }
}
