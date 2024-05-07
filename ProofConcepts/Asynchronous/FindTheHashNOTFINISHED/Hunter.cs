using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FindTheHash
{
    public class Hunter
    {
        private string _directoryRepresentation;
        private List<string> _directoryTracker;
        private DatabaseConnector _databaseConnection;
        private Hasher _hasher;
        public Hunter(string directoryRepresentation, string databaseDirectory)
        {
            _directoryRepresentation = directoryRepresentation;
            _directoryTracker = new();
            _databaseConnection = new DatabaseConnector(databaseDirectory);
            _hasher = new Hasher();
        }

        public Tuple<string[], string[]> SearchDirectory()
        {
            try
            {
                string[] fileCycle = Directory.GetFiles(_directoryRepresentation);
                string[] directoryRemnants = Directory.GetDirectories(_directoryRepresentation);
                List<string> violationsList = new List<string>();
                foreach (string fileDirEach in fileCycle)
                {
                    if (CompareCycle(fileDirEach))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Violation found: {fileDirEach}");
                        Console.ForegroundColor = ConsoleColor.White;
                        violationsList.Add(fileDirEach);
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

    }
}
