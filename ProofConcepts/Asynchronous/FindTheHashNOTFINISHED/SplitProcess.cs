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
        private List<Hunter> hunterUnits;
        private string _databaseDirectory;
        public SplitProcess(string databaseDirectory)
        {
            _directoryViolations = new();
            _directoryRemnants = new();
            hunterUnits = new();
            _databaseDirectory = databaseDirectory;
        }

        public void SearchDirectory() // Attempt to utilise events for alert maybe, this attempts to demonstrate Asynchronous Abilities.
        {
            int hunterCreationLimit = 200;
            Console.WriteLine("SplitProcess has started splitting directories to hunters.");
            while (_directoryRemnants.Count > 0)
            {
                for (int i = 0; i < Math.Min(_directoryRemnants.Count,200); i++)
                {
                    hunterUnits.Add(new Hunter(_directoryRemnants.Pop(), _databaseDirectory));
                }
                // Remove items from directory remnants (based on how many hunters)
                Console.WriteLine($"Hunter Units in Use: {hunterUnits.Count}");
                foreach (Hunter hunter in hunterUnits)
                {
                    Tuple<string[], string[]> tupleReturn = hunter.SearchDirectory(); // Stack ensures the directories further down are unpacked first.;
                    UnpackTuple(tupleReturn);
                }
                Console.WriteLine($"Hunter units destroyed, directoryRemnants: {_directoryRemnants.Count}");
                hunterUnits.Clear();
                // wait all thing here, they should return more directories

                // Add more directory remnants here.
            }
            Console.WriteLine($"Search has finalized, violations detected: {_directoryViolations.Count}");
        }

        private void UnpackTuple(Tuple<string[], string[]> tuple)
        {
            Array.ForEach<string>(tuple.Item1, _directoryViolations.Add);
            Array.ForEach<string>(tuple.Item2, _directoryRemnants.Push);
        }
        public void fillUpSearch(string directory)
        {
            Tuple <string[], string[]> tupleItem = new Hunter(directory, _databaseDirectory).SearchDirectory();
            UnpackTuple(tupleItem);
        }
    }
}
