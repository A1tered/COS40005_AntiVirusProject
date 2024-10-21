using System.Text;
using System.IO;

namespace SimpleAntivirus.FileHashScanning
{
    /// <summary>
    /// DirectoryManager deals with the annoying concept of directory management in C#.
    /// 
    /// The solutions implemented here are unlikely to be industry standard, and are quick fixes that may bring trouble in the future.
    /// </summary>
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

        public string getDatabaseDirectory(string databaseName) // Find database folder and then database, return directory (assumption folder name is Database)
        {
            string databaseDirectory;
            string[] directoryContents;
            string[] itemContents;
            int limitTracker = 0; // Tracks how many directories up you are searching.
            StringBuilder directorySearchBuilder = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory);
            while (limitTracker < _stepUpLimit)
            {
                directoryContents = Directory.GetDirectories(directorySearchBuilder.ToString());
                string fetchDirectory = MatchDirectory(directoryContents, "Databases");
                itemContents = Directory.GetFiles(fetchDirectory);
                databaseDirectory = MatchDirectory(itemContents, databaseName);
                if (databaseDirectory != "")
                {
                        
                    return databaseDirectory;
                }
                //limitTracker++;
            }
            return "";
        }
    }
}
