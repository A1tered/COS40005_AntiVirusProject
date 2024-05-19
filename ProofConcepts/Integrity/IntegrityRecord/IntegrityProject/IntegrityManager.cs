using FindTheHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityMarkRecordGetAccessRecords
{
    public class IntegrityManager
    {
        private DatabaseConnector _databaseConnector;
        private List<string> _directoryViolationsKnown;
        private List<string> _violationsSentOut;
        private List<long> _correlatingSizeDifferences;
        public IntegrityManager(string databaseDirectory)
        {
            _databaseConnector = new(databaseDirectory);
            _directoryViolationsKnown = new();
            _violationsSentOut = new();
            _correlatingSizeDifferences = new();
        }


        public string UnixTimeFormat(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("dd:MM:yyyy");
        }

        public List<string> IntegrityCheck(bool bootCheck = false)
        {
            List<String> directorySet = _databaseConnector.QueryDirectories();
            List<String> directoryViolations = new();
            List<String> informationSet = new();
            FileInspector tempInspector = new();
            string openHashResult;
            string fileExtraInfo;
            string userCorrelation;
            long sizeDifference;
            bool sizeDifferenceKnownIgnore;
            int tempIndex;
            foreach (string directory in directorySet)
            {
                sizeDifferenceKnownIgnore = false;
                userCorrelation = "N/A";
                fileExtraInfo = "";
                Tuple<string, long ,long, long> result = _databaseConnector.QueryDirectoryData(directory);
                openHashResult = tempInspector.OpenHashFile(directory);
                sizeDifference = tempInspector.GetIntegrityAttributes(directory).Item3 - result.Item4;
                tempIndex = _directoryViolationsKnown.FindIndex(e => e == directory);
                if (tempIndex >= 0)
                {
                    sizeDifferenceKnownIgnore = _correlatingSizeDifferences[tempIndex] != sizeDifference;
                    _correlatingSizeDifferences[tempIndex] = sizeDifference;
                }
                if (result.Item1 != openHashResult && result.Item1 != "")
                {
                    // Adds to a list of directories already known to exist, ran at boot. 
                    if (!_directoryViolationsKnown.Contains(directory))
                    {
                        if (bootCheck)
                        {
                            _directoryViolationsKnown.Add(directory);
                        }
                        else
                        {
                            // get user, (warning, makes this program windows only)
                            try
                            {
                                userCorrelation = WindowsIdentity.GetCurrent().Name;
                            }
                            catch (Exception error)
                            {
                                userCorrelation = "N/A not supported";
                            }
                        }
                    }
                    if (openHashResult == "N")
                    {
                        fileExtraInfo = "File was deleted.";
                    }
                    if (!_violationsSentOut.Contains(directory) || sizeDifferenceKnownIgnore)
                    {
                        if (!_violationsSentOut.Contains(directory))
                        {
                            _violationsSentOut.Add(directory);
                            _correlatingSizeDifferences.Add(sizeDifference);
                        }
                        directoryViolations.Add(directory);
                        informationSet.Add($@"Violation Found: 
                    Directory: {directory}
                    LastApprovedModificationTime: {UnixTimeFormat(result.Item2)}
                    SignatureTime: {UnixTimeFormat(result.Item3)}
                    Size_Difference: {sizeDifference} bytes
                    {fileExtraInfo}
                    User Correlation: {userCorrelation}");
                    }
                }
            }
            //
            return informationSet; // Expected to return a list of violations and their relevant directories.
        }

        public void AddIntegrity(string directory)
        {
            if (File.Exists(directory))
            {
                FileInspector tempInspector = new();
                string hash = tempInspector.OpenHashFile(directory);
                Tuple<long, long, long> tupleReturn = tempInspector.GetIntegrityAttributes(directory);
                Console.WriteLine($"File Directory: {directory},  File properties, Hash: {hash}, modificationTime: {tupleReturn.Item1}, signatureTime: {tupleReturn.Item2}, sizeBytes : {tupleReturn.Item3}");
                if (_databaseConnector.AddIntegrityEntry(directory, hash, tupleReturn.Item1, tupleReturn.Item2, tupleReturn.Item3))
                {
                    Console.WriteLine("Addition/Update success.");
                }
                else
                {
                    Console.WriteLine("Addition/Update failure.");
                }
            }
            else
            {
                Console.WriteLine("THIS FILE DIRECTORY DOES NOT EXIST");
            }
        }
    }
}
