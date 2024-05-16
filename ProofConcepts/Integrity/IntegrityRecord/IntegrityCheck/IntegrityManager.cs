using FindTheHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityMarkRecordGetAccessRecords
{
    public class IntegrityManager
    {
        private DatabaseConnector _databaseConnector;
        public IntegrityManager(string databaseDirectory)
        {
            _databaseConnector = new(databaseDirectory);
        }


        public string UnixTimeFormat(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).ToString("dd:MM:yyyy");
        }

        public List<string> IntegrityCheck()
        {
            List<String> directorySet = _databaseConnector.QueryDirectories();
            List<String> directoryViolations = new();
            List<String> informationSet = new();
            string openHashResult;
            string fileExtraInfo;
            // Debug Block
            foreach (string directory in directorySet)
            {
                fileExtraInfo = "";
                Tuple<string, long ,long> result = _databaseConnector.QueryDirectoryData(directory);
                openHashResult = new FileInspector().OpenHashFile(directory);
                if (result.Item1 != openHashResult && result.Item1 != "")
                {
                    if (openHashResult == "N")
                    {
                        fileExtraInfo = "File was deleted.";
                    }
                    directoryViolations.Add(directory);
                    informationSet.Add($@"Violation Found: 
                    Directory: {directory}
                    LastApprovedModificationTime: {UnixTimeFormat(result.Item2)}
                    SignatureTime: {UnixTimeFormat(result.Item3)}
                    {fileExtraInfo}
                    ");
                }
            }
            //
            return informationSet; // Expected to return a list of violations and their relevant directories.
        }

        public void AddIntegrity(string directory)
        {
            FileInspector tempInspector = new();
            string hash = tempInspector.OpenHashFile(directory);
            Tuple<long, long> tupleReturn = tempInspector.GetIntegrityAttributes(directory);
            Console.WriteLine($"File Directory: {directory},  File properties, Hash: {hash}, modificationTime: {tupleReturn.Item1}, signatureTime: {tupleReturn.Item2}");
            if (_databaseConnector.AddIntegrityEntry(directory, hash, tupleReturn.Item1, tupleReturn.Item2))
            {
                Console.WriteLine("Addition/Update success.");
            }
            else
            {
                Console.WriteLine("Addition/Update failure.");
            }
        }
    }
}
