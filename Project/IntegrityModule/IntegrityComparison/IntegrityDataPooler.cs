using DatabaseFoundations;
using DatabaseFoundations.IntegrityRelated;
using IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.IntegrityComparison
{
    public class IntegrityDataPooler
    {
        private int _setRepresentation;
        private int _setAmount;
        private IntegrityDatabaseIntermediary _databaseIntermediary;

        public IntegrityDataPooler(int set, int setAmount)
        {
            _setRepresentation = set;
            _setAmount = setAmount;
        }

        public CheckIntegrity()
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
                    violation.Path = dirHash.Key;
                    violation.OriginalHash = dirHash.Value;
                    // Real time info
                    violation.Hash = tempHash;
                }
            }
        }
    }
}
