/**************************************************************************
 * File:        IntegrityDataPooler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Scans a set of database directories and compares them with system documents.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataRelated;
using SimpleAntivirus.IntegrityModule.DataTypes;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using SimpleAntivirus.IntegrityModule.Interface;

namespace SimpleAntivirus.IntegrityModule.IntegrityComparison
{
    public class IntegrityDataPooler : IIntegrityDataPooler
    {
        private int _setRepresentation;
        private int _setAmount;
        private string _selectedDirectory;
        private IIntegrityDatabaseIntermediary _databaseIntermediary;

        public IntegrityDataPooler(IIntegrityDatabaseIntermediary database, int set, int setAmount)
        {
            _databaseIntermediary = database;
            _setRepresentation = set;
            _setAmount = setAmount;
        }

        /// <summary>
        /// Set up data pooler only to scan a specific file. (Generally ideal for reactive control use)
        /// </summary>
        /// <param name="database"></param>
        /// <param name="path"></param>
        public IntegrityDataPooler(IIntegrityDatabaseIntermediary database, string path)
        {
            _databaseIntermediary = database;
            _selectedDirectory = path;
        }

        /// <summary>
        /// Creates integrity violation object from Tuple created from FileInfoRequester RetrieveFileInfo.
        /// </summary>
        /// <param name="newHash">Hash of real time file.</param>
        /// <param name="resultTuple">Tuple returned from RetrieveFileInfo.</param>
        /// <returns>IntegrityViolation data structure.</returns>
        private IntegrityViolation CreateViolation(string newHash, Tuple<string, string, long, long, long> resultTuple)
        {
            IntegrityViolation violationNew = new();
            violationNew.Path = resultTuple.Item1;
            violationNew.OriginalHash = resultTuple.Item2;
            violationNew.TimeOfSignature = resultTuple.Item4;
            if (newHash == "")
            {
                violationNew.Missing = true;
            }
            if (violationNew.Missing != true)
            {
                // Real time info
                violationNew.Hash = newHash;
                violationNew.TimeOfViolation = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                // Ensure windows specific command, only runs on windows.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    violationNew.RecentUser = WindowsIdentity.GetCurrent().Name;
                }
                violationNew.FileSizeBytesChange = FileInfoRequester.SizeValueToLabel(resultTuple.Item5 - new FileInfo(resultTuple.Item1).Length);
            }
            return violationNew;
        }

        /// <summary>
        /// Retrieving a set of the database, compare with system files.
        /// </summary>
        /// <returns>Violations found via mismatching Hashes.</returns>
        public async Task<List<IntegrityViolation>> CheckIntegrity(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            List<IntegrityViolation> violationSet = new();
            if (_selectedDirectory == null)
            {
                Dictionary<string, string> infoSet = _databaseIntermediary.GetSetEntries(_setRepresentation, _setAmount);
                // We want to async calculate all hashes before cycling across.
                List<string> stringList = await FileInfoRequester.HashSet(infoSet.Keys.ToList());
                string tempHash = "";
                int index = 0;
                foreach (KeyValuePair<string, string> dirHash in infoSet)
                {
                    ct.ThrowIfCancellationRequested();
                    tempHash = stringList[index];
                    index++;
                    if (tempHash != dirHash.Value)
                    {
                        // Database info
                        Tuple<string, string, long, long, long> resultTuple = _databaseIntermediary.GetDirectoryInfo(dirHash.Key);
      
                        violationSet.Add(CreateViolation(tempHash, resultTuple));
                    }
                }
            }
            return violationSet;
        }

        /// <summary>
        /// Singular comparison between the database and a windows file.
        /// </summary>
        /// <returns>Violation or Null</returns>
        public async Task<List<IntegrityViolation>> CheckIntegrityDirectory()
        {
            List<IntegrityViolation> violationSet = new();
            Dictionary<string, string> returnInfo = _databaseIntermediary.GetSetEntriesDirectory(_selectedDirectory);
            List<string> stringList = await FileInfoRequester.HashSet(returnInfo.Keys.ToList());
            int index = 0;
            string tempHash = "";
            foreach (KeyValuePair<string, string> dirHash in returnInfo)
            {
                tempHash = stringList[index];
                index++;
                if (tempHash != dirHash.Value)
                {
                    // Database info
                    Tuple<string, string, long, long, long> infoTuple = _databaseIntermediary.GetDirectoryInfo(dirHash.Key);

                    violationSet.Add(CreateViolation(tempHash, infoTuple));
                }
            }
            return violationSet;
        }

        public int Set
        {
            get
            {
                return _setRepresentation;
            }
        }
    }
}
