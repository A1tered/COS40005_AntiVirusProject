using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FindTheHash
{
    public class SplitProcess
    {
        private List<string> _directoryViolations;
        private Stack<string> _directoryRemnants;
        private List<Hunter> _hunterUnits;
        private List<Task> _taskUnits;
        private string _databaseDirectory;
        private int _directoriesSearched;
        private int _filesScanned;

        /// <summary>
        /// The start of the directory unpacking process.
        /// 
        /// Spawns hunters that find directories and return their results, cycles this multiple times until
        /// no more directories are to be unpacked. 
        /// 
        /// </summary>
        /// <param name="databaseDirectory"></param>
        public SplitProcess(string databaseDirectory, int filesScanned)
        {
            _directoryViolations = new();
            _directoryRemnants = new();
            _hunterUnits = new();
            _taskUnits = new();

            _databaseDirectory = databaseDirectory;

            // Statistics
            _directoriesSearched = 0;
            _filesScanned = filesScanned;
        }

        public async Task<int> SearchDirectory()
        {
            // OPTIONS THAT DIRECTLY AFFECT PERFORMANCE!!!
            // How many asynchronous directory readers can run in a cycle (More > system use is heavier)
            // default 500
            int hunterCreationLimit = 500;
            // (ms millisecond time) How long to wait for asynchronous tasks, will keep tasks in next cycle if going over.
            // default 1000
            int taskWaitTime = 1000;
            // temp vars
            int removedTasks;
            int totalTasks;
            Console.WriteLine("SplitProcess has started splitting directories to hunters.");
            while (_directoryRemnants.Count > 0 || _taskUnits.Count > 0)
            {
                // Max at hunter units - task units.
                for (int i = 0; i < Math.Min(_directoryRemnants.Count, hunterCreationLimit - _taskUnits.Count()); i++)
                {
                    _hunterUnits.Add(new Hunter(_directoryRemnants.Pop(), _databaseDirectory, _filesScanned));
                }
                // Remove items from directory remnants (based on how many hunters)
                Console.WriteLine($"Hunter Units in Use: {_hunterUnits.Count}");
                foreach (Hunter hunter in _hunterUnits)
                {
                    _taskUnits.Add(Task.Run(() => hunter.SearchDirectory()));
                }
                Console.WriteLine($"Hunter units destroyed, directoryRemnants: {_directoryRemnants.Count}");
                _hunterUnits.Clear();
                totalTasks = _taskUnits.Count();
                // wait all thing here, they should return more directories
                Task.WaitAll(_taskUnits.ToArray(), taskWaitTime);
                foreach (Task<(string[], string[], int)> task in _taskUnits)
                {
                    if (task.IsCompleted)
                    {
                        _directoriesSearched++;
                        await Unpack(task.Result.Item1, task.Result.Item2, task.Result.Item3);
                    }
                }
                // Remove all completed tasks
                removedTasks = _taskUnits.RemoveAll(task => task.IsCompleted == true);
                Console.WriteLine($"Removed {removedTasks} tasks, {totalTasks - removedTasks} tasks are ongoing... ");
            }
                Console.WriteLine($"Search has finalized, violations detected: {_directoryViolations.Count}");
            return _filesScanned;
        }

        private async Task Unpack(string[] Violations, string[] DirectoryRemnants, int FilesScanned)
        {
            // Hunters have new directories to search and any violations they've found via tuple.
            await Task.Run(() =>
            {
                Array.ForEach<string>(Violations, _directoryViolations.Add);
                Array.ForEach<string>(DirectoryRemnants, _directoryRemnants.Push);
                _filesScanned += FilesScanned;
            });
        }

        // Initial function, to find the initial directories. This is not called other than in the initial process.
        public async Task fillUpSearch(string directory)
        {
            (string[] violations, string[] directoryRemnants, int filesScanned) = await new Hunter(directory, _databaseDirectory, _filesScanned).SearchDirectory();
            await Unpack(violations, directoryRemnants, filesScanned);
        }

        public int DirectoriesSearched
        {
            get
            {
                return _directoriesSearched;
            }
        }


    }
}