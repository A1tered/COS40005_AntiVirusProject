using DatabaseFoundations;
using DatabaseFoundations.IntegrityRelated;
using IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.IntegrityComparison
{
    public class IntegrityDataPooler
    {
        private int _setRepresentation;
        private int _setAmount;
        private IntegrityDatabaseIntermediary _databaseIntermediary;

        public IntegrityDataPooler(IntegrityDatabaseIntermediary database, int set, int setAmount)
        {
            _databaseIntermediary = database;
            _setRepresentation = set;
            _setAmount = setAmount;
        }

        public List<IntegrityViolation> CheckIntegrity()
        {
            List<IntegrityViolation> violationSet = new();
            Dictionary<string, string> infoSet = _databaseIntermediary.GetSetEntries(_setRepresentation, _setAmount);
            string tempHash = "";
            foreach (KeyValuePair<string, string> dirHash in infoSet)
            {
                tempHash = FileInfoRequester.HashFile(dirHash.Key);
                if (tempHash != dirHash.Value)
                {
                    IntegrityViolation violation = new IntegrityViolation();
                    // Database info
                    Tuple<string, string, long, long, long> resultTuple = _databaseIntermediary.GetDirectoryInfo(dirHash.Key);
                    violation.Path = dirHash.Key;
                    violation.OriginalHash = dirHash.Value;
                    violation.OriginalSize = resultTuple.Item5;
                    if (tempHash == "")
                    {
                        violation.Missing = true;
                    }
                    if (violation.Missing != true)
                    {
                        // Real time info
                        violation.Hash = tempHash;
                        violation.TimeOfViolation = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                        // Ensure windows specific command, only runs on windows.
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            violation.RecentUser = WindowsIdentity.GetCurrent().Name;
                        }
                        violation.FileSizeBytes = new FileInfo(dirHash.Key).Length;
                    }
                    violationSet.Add(violation);
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
