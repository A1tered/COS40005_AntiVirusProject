using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FindTheHash
{
    /// <summary>
    /// Hunter Class
    /// Responsibilities: Given a directory, is responsible for hashing items and comparing to database.
    /// </summary>
    /// 
    public class Hunter
    {
        private string _directoryToScan;
        private DatabaseConnector _databaseConnection;
        public Hasher _hasher;
        private string _databaseDirectory;
        private int _filesScanned;

        public Hunter(string directoryToScan, string databaseDirectory, int filesScanned)
        {
            _directoryToScan = directoryToScan;
            _databaseConnection = new DatabaseConnector(databaseDirectory);
            _hasher = new Hasher();
            _filesScanned = filesScanned;
        }

        public async Task<(string[] Violations, string[] DirectoryRemnants, int FilesScanned)> SearchDirectory()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(_directoryToScan);
                    string[] directoryRemnants = Directory.GetDirectories(_directoryToScan);
                    List<string> violationsList = new List<string>();
                    foreach (string file in files)
                    {
                        if (CompareCycle(file))
                        {
                            _filesScanned++;
                            Console.WriteLine($"Files scanned: {_filesScanned}");
                            violationsList.Add(file);
                            Violation(file);
                        }
                    }
                    return (violationsList.ToArray(), directoryRemnants, _filesScanned);
                }
                catch (Exception exception)
                {
                    if (exception is IOException || exception is AccessViolationException || exception is UnauthorizedAccessException)
                    {
                        return (Array.Empty<string>(), Array.Empty<string>(), 0);
                    }
                    throw;
                }
                finally
                {
                    _databaseConnection.CleanUp();
                }
            });
        }

        private void Violation(string fileDirectory)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Violation found: {fileDirectory}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public bool CompareCycle(string fileDirectory)
        {
            string hashGet = _hasher.OpenHashFile(fileDirectory);
            if (hashGet != "Non-readable")
            {
                return _databaseConnection.QueryHash(hashGet);
            }
            return false;
        }

        public int[] CompareCycleBatch(string[] fileDirectory)
        {
            string[] hashGetArray = _hasher.OpenHashFileBatch(fileDirectory);
            int[] indexDetections = _databaseConnection.QueryHashBatch(hashGetArray);
            return indexDetections;
        }
    }
}
