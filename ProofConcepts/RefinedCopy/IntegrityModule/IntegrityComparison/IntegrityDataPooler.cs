/**************************************************************************
 * File:        IntegrityDataPooler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Scans a set of database directories and compares them with system documents.
 * Last Modified: 26/08/2024
 **************************************************************************/

using DatabaseFoundations;
using DatabaseFoundations.IntegrityRelated;
using IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.IntegrityComparison
{
    public class IntegrityDataPooler
    {
        private int _setRepresentation;
        private int _setAmount;
        private string _selectedPath;
        private IntegrityDatabaseIntermediary _databaseIntermediary;

        public IntegrityDataPooler(IntegrityDatabaseIntermediary database, int set, int setAmount)
        {
            _databaseIntermediary = database;
            _setRepresentation = set;
            _setAmount = setAmount;
        }

        public IntegrityDataPooler(IntegrityDatabaseIntermediary database, string path)
        {
            _databaseIntermediary = database;
            _selectedPath = path;
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
        public async Task<List<IntegrityViolation>> CheckIntegrity()
        {
            List<IntegrityViolation> violationSet = new();
            if (_selectedPath == null)
            {
                Dictionary<string, string> infoSet = _databaseIntermediary.GetSetEntries(_setRepresentation, _setAmount);
                // We want to async calculate all hashes before cycling across.
                List<string> stringList = await FileInfoRequester.HashSet(infoSet.Keys.ToList());
                string tempHash = "";
                int index = 0;
                foreach (KeyValuePair<string, string> dirHash in infoSet)
                {
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
        public async Task<IntegrityViolation> CheckIntegrityFile()
        {
            string hash = await FileInfoRequester.HashFile(_selectedPath);
            Tuple<string, string, long, long, long> resultTuple = _databaseIntermediary.GetDirectoryInfo(_selectedPath);
            if (hash != resultTuple.Item2)
            {
                return CreateViolation(hash, resultTuple);
            }
            return null;
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
