using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleAntivirus.Alerts;
using SimpleAntivirus.FileQuarantine;
using System.Diagnostics;

namespace SimpleAntivirus.FileHashScanning
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
        private readonly CancellationToken _token;

        public Hunter(string directoryToScan, string databaseDirectory, CancellationToken token)
        {
            _directoryToScan = directoryToScan;
            _databaseConnection = new DatabaseConnector(databaseDirectory);
            _token = token;
            _hasher = new Hasher();
        }

        public async Task<Tuple<string[], string[]>> SearchDirectory(FileHashScanner scanner)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Debug.WriteLine($"Hunter currently scanning directory {_directoryToScan}");

                    string[] files = Directory.GetFiles(_directoryToScan);
                    string[] directoryRemnants = Directory.GetDirectories(_directoryToScan);
                    List<string> violationsList = new List<string>();

                    foreach (string file in files)
                    {
                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }
                        Debug.WriteLine($"Current file: {file}");
                        FileInfo fileInfo = new FileInfo(file);
                        scanner.UpdateSize(fileInfo.Length);
                        scanner.UpdateProgress();

                        if (CompareCycle(file))
                        {
                            violationsList.Add(file);
                            Violation(scanner,file);
                        }
                    }
                    return Tuple.Create(violationsList.ToArray(), directoryRemnants);
                }
                catch (Exception exception)
                {
                    if (exception is IOException || exception is AccessViolationException || exception is UnauthorizedAccessException)
                    {
                        Debug.WriteLine($"IO Exception. Cannot open directory {_directoryToScan}");
                        return Tuple.Create(Array.Empty<string>(), Array.Empty<string>());
                    }
                    throw;
                }
                finally
                {
                    _databaseConnection.CleanUp();
                }
            }, scanner.Token);
        }

        private async void Violation(FileHashScanner scanner, string fileDirectory)
        {
            await scanner.EventBus.PublishAsync("File Hash Scanning", "Severe", $"Threat found! File: {fileDirectory} has been found and SAV has quarantined the threat.", "No action is required. You may unquarantine or delete if you choose.");
            await scanner.QuarantineManager.QuarantineFileAsync(fileDirectory);
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
