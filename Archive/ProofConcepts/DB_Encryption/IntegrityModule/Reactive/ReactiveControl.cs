﻿using DatabaseFoundations;
using DatabaseFoundations.IntegrityRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.Reactive
{
    public class ReactiveControl
    {
        private List<FileSystemWatcher> _fileWatcherList;
        private IntegrityDatabaseIntermediary _intermediaryDB;
        public ReactiveControl(IntegrityDatabaseIntermediary intermediary)
        {
            _fileWatcherList = new();
            _intermediaryDB = intermediary;
        }

        public bool Initialize()
        {
            List<string> directoriesMonitor = new();
            Console.WriteLine("Reactive Control Initialization");
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

        private void SetUpFileWatcher(string path)
        {
            Console.WriteLine($"Attempted Event Connection: {path}");
            FileSystemWatcher fileWatcherTemp = new(Path.GetDirectoryName(path), Path.GetFileName(path));
            fileWatcherTemp.EnableRaisingEvents = true;
            fileWatcherTemp.Changed += IndividualScanEventHandler;
            _fileWatcherList.Add(fileWatcherTemp);
        }

        private void IndividualScanEventHandler(object sender, FileSystemEventArgs eventArguments)
        {
            Console.WriteLine($"Item changed {eventArguments.FullPath}");
        }

        public void Add(string path)
        {
            List<string> pathsToAdd = FileInfoRequester.PathCollector(path);
        }
    }
}
