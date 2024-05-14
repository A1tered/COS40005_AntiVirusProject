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
    /// It will also find directories and pass them back up to the "SplitProcess" class.
    /// </summary>
    public class Hunter
    {
        // Directory that the hunter is responsible for.
        private string _directoryRepresentation;
        private List<string> _directoryTracker;
        private DatabaseConnector _databaseConnection;
        private Hasher _hasher;
        private bool _asyncOperation;
        public Hunter(string directoryRepresentation, string databaseDirectory, bool asyncOperation = false)
        {
            _directoryRepresentation = directoryRepresentation;
            _directoryTracker = new();
            _databaseConnection = new DatabaseConnector(databaseDirectory);
            _hasher = new Hasher();
            _asyncOperation = asyncOperation;
        }

        private void Violation(string fileDirectory)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Violation found: {fileDirectory}");
            Console.ForegroundColor = ConsoleColor.White;

        }
        public Tuple<string[], string[]> SearchDirectory()
        {
            try
            {
                int[] asyncIndexHolder;
                string[] fileCycle = Directory.GetFiles(_directoryRepresentation);
                string[] directoryRemnants = Directory.GetDirectories(_directoryRepresentation);
                List<string> violationsList = new List<string>();
                if (_asyncOperation)
                {
                    asyncIndexHolder = CompareCycleBatch(fileCycle);
                    foreach (int index in asyncIndexHolder)
                    {
                        violationsList.Add(fileCycle[index]);
                    }
                }
                else
                {
                    foreach (string fileDirEach in fileCycle)
                    {
                        if (CompareCycle(fileDirEach))
                        {
                            violationsList.Add(fileDirEach);
                            Violation(fileDirEach);
                        }
                    }
                }
                return new Tuple<string[], string[]>(violationsList.ToArray(), directoryRemnants);
            }
            catch (Exception exception)
            {
                if (exception is IOException || exception is AccessViolationException || exception is UnauthorizedAccessException)
                {
                    return new Tuple<string[], string[]>(Array.Empty<string>(), Array.Empty<string>());
                }
                throw;
            }
            finally
            {
                _databaseConnection.CleanUp();
            }
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
