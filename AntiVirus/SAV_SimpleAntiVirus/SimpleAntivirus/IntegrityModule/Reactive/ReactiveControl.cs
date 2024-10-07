/**************************************************************************
 * File:        IntegrityConfigurator.cs
 * Author:      Christopher Thompson, etc.
 * Description: Deals with scans that are reactive, such that if a file change occurs that it scans that certain file for performance, and the reduction
 * of time taken to respond to issues.
 * Last Modified: 8/10/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using System.IO;
using SimpleAntivirus.IntegrityModule.Interface;

namespace SimpleAntivirus.IntegrityModule.Reactive
{
    public class ReactiveControl : IReactiveControl
    {
        private List<FileSystemWatcher> _fileWatcherList;
        private IIntegrityDatabaseIntermediary _intermediaryDB;
        private IIntegrityCycler _integrityCycler;
        private bool _reactiveInitialized;
        private List<string> _directoryTracker;
        // Prevent overlap (may result in detection failures, but better than alert failures)
        private bool _eventCallInProgress = false;
        public ReactiveControl(IIntegrityDatabaseIntermediary intermediary, IIntegrityCycler cycler)
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
                            fileWatcherTemp.Deleted += IndividualScanEventHandler;
                            fileWatcherTemp.Renamed += IndividualScanEventHandler;
                            _fileWatcherList.Add(fileWatcherTemp);
                            _directoryTracker.Add(getDirectoryPath);
                        }
                    }
                }
            }
        }

        private async void IndividualScanEventHandler(object sender, FileSystemEventArgs eventArguments)
        {
            // Dirty fix: Await here, so there is enough time for file explorer to make changes without being evaluated halfway
            // through resulting in lost information. (Obviously for large operations this would still have issues, but still greatly
            // improves the information that can get through.
            await Task.Delay(1000);
            if (!_eventCallInProgress)
            {
                _eventCallInProgress = true;
                string getDirectoryPath = Path.GetDirectoryName(eventArguments.FullPath);
                if (getDirectoryPath != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Item changed {eventArguments.FullPath}");
                    await _integrityCycler.InitiateDirectoryScan(getDirectoryPath);
                }
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
            // Only remove for directory, if, all traces of the directory do not exist in database.
            string getDirectoryPath = Path.GetDirectoryName(path);
            if (getDirectoryPath != null)
            {
                Dictionary<string, string> directoryHashDict = _intermediaryDB.GetSetEntriesDirectory(getDirectoryPath);
                System.Diagnostics.Debug.WriteLine($"Debug Count Check: {directoryHashDict.Count}");
                if (directoryHashDict.Count == 0) {
                    // Remove all file watchers that equal given path.
                    List<FileSystemWatcher> disposedObjects = new();
                    _directoryTracker.Remove(getDirectoryPath);
                    foreach (FileSystemWatcher fileWatcher in _fileWatcherList)
                    {
                        if (fileWatcher.Path == getDirectoryPath)
                        {
                            disposedObjects.Add(fileWatcher);
                            System.Diagnostics.Debug.WriteLine($"Attempted Event Disconnection: {fileWatcher.Path}");
                            fileWatcher.Dispose();
                        }
                    }
                    disposedObjects.ForEach(x => _fileWatcherList.Remove(x));
                }
            }
        }

        public void RemoveAll()
        {
            System.Diagnostics.Debug.WriteLine($"Attempted All Event Disconnection");
            _fileWatcherList.ForEach(x => x.Dispose());
            _fileWatcherList.Clear();
            _directoryTracker.Clear();
        }
    }
}
