using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch.ScanRelated
{
    public class IntegrityDataPooler
    {
        private IntegrityDatabaseIntermediary _integrityDatabaseHandler;
        private int _set;
        private int _setAmount;
        public IntegrityDataPooler(IntegrityDatabaseIntermediary integrityHandler, int set, int amountPerSet)
        {
            _integrityDatabaseHandler = integrityHandler;
            _set = set;
            _setAmount = amountPerSet;
        }

        public string CheckIntegrity()
        {
            
            Dictionary<string, string> returnedResults = _integrityDatabaseHandler.QueryDirectoryHashPairs(_set, _setAmount);
            string localHash = "";
            StringBuilder resultLoad = new();
            foreach (KeyValuePair<string, string> dictHash in returnedResults)
            {
                // If current hash does not equal hash stored
                localHash = Hasher.HashFile(dictHash.Key);
                if (Path.Exists(dictHash.Key))
                {
                    if (localHash != dictHash.Value)
                    {
                        // Violation
                        resultLoad.Append($"Hash Mismatch at: {dictHash.Key}");
                        List<string> fileDatabaseInfo = _integrityDatabaseHandler.QueryDirectory(dictHash.Key);
                        List<long> modTimeSizeFile = Hasher.FileInfoUnpack(dictHash.Key);
                        resultLoad.Append($"    Hash:\n {dictHash.Value} -> {localHash}");
                        resultLoad.Append($"    Size:\n {DisplayHandler.ByteToSize(long.Parse(fileDatabaseInfo[4]))} -> {DisplayHandler.ByteToSize(modTimeSizeFile[1])}");
                        resultLoad.Append("\n");
                    }
                }
                else
                {
                    // If the path no longer exists then provide warning.
                    resultLoad.Append($"Warning, file missing at: {dictHash.Key}");
                }
            }
            Console.WriteLine($"Data pooler scanned - {_setAmount} in set {_set}");
            return resultLoad.ToString();
        }
    }
}
