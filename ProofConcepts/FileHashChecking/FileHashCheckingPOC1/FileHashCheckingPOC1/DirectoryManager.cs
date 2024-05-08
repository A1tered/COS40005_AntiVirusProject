using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FileHashChecking
{
    public class DirectoryManager
    {
        private int _stepUpLimit;
        public DirectoryManager()
        {
            _stepUpLimit = 4;
        }

        private string MatchDirectory(string[] directorySet, string name)
        {
            foreach (string directory in directorySet)
            {
                Console.WriteLine($"Debug Directory Cycler: {directory}");
                if (directory.Contains(name))
                {
                    Console.WriteLine($"Found {directory}");
                    return directory;
                }
            }
            return "";
        }

        public string getDatabaseDirectory(string databaseName) // Find database folder and then database, return directory (assumption folder name is Databases.
        {
            Console.WriteLine("\r\nDatabase File Searcher:");
            string databaseDirectory;
            string[] directoryContents;
            string[] itemContents;
            int limitTracker = 0; // Tracks how many directories up you are searching.
            StringBuilder directorySearchBuilder = new StringBuilder();
            while (limitTracker < _stepUpLimit)
            {
                directorySearchBuilder.Append("..\\");
                directoryContents = Directory.GetDirectories(directorySearchBuilder.ToString());
                string fetchDirectory = MatchDirectory(directoryContents, "Database");
                if (fetchDirectory != "")
                {
                    itemContents = Directory.GetFiles(fetchDirectory);
                    Console.WriteLine(itemContents[0]);
                    databaseDirectory = MatchDirectory(itemContents, databaseName);
                    if (databaseDirectory != "")
                    {
                        return databaseDirectory;
                    }
                }
                limitTracker++;
            }
            return "";
        }
    }
}