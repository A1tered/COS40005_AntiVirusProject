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
        public ReactiveControl(IntegrityDatabaseIntermediary intermediary, IntegrityCycler cycler)
        {
            _reactiveInitialized = false;
            _fileWatcherList = new();
            _intermediaryDB = intermediary;
            _integrityCycler = cycler;
        }

        public bool Initialize()
        {
            // Only initialise if havent before
            if (_reactiveInitialized == false)
            {
                _reactiveInitialized = true;
                System.Diagnostics.Debug.WriteLine("Reactive Control Initialization");
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
                System.Diagnostics.Debug.WriteLine($"Attempted Event Connection: {path}");
                if (Path.Exists(path))
                {
                    FileSystemWatcher fileWatcherTemp = new(Path.GetDirectoryName(path), Path.GetFileName(path));
                    fileWatcherTemp.EnableRaisingEvents = true;
                    fileWatcherTemp.Changed += IndividualScanEventHandler;
                    _fileWatcherList.Add(fileWatcherTemp);
                }
            }
        }

        private async void IndividualScanEventHandler(object sender, FileSystemEventArgs eventArguments)
        {
            //System.Diagnostics.Debug.WriteLine($"Item changed {eventArguments.FullPath}");
            await _integrityCycler.InitiateSingleScan(eventArguments.FullPath);
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
