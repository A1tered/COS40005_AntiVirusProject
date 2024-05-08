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
                if (directory.Contains(name))
                {
                    return directory;
                }
            }
            return "";
        }

        public string getDatabaseDirectory(string databaseName) // Find database folder and then database, return directory (assumption folder name is Databases.
        {
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