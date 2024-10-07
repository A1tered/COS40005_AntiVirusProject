/**************************************************************************
 * File:        IntegrityDataPooler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Scans a set of database directories and compares them with system documents.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;
using SimpleAntivirus.IntegrityModule.Interface;
using SimpleAntivirus.IntegrityModule.IntegrityComparison;

namespace TestingIntegrity.DummyClasses
{
    public class IntegrityDataPoolerDummy : IIntegrityDataPooler
    {


        public IntegrityDataPoolerDummy(IIntegrityDatabaseIntermediary database, int set, int setAmount)
        {

        }

        public IntegrityDataPoolerDummy(IIntegrityDatabaseIntermediary database, string path)
        {

        }

        /// <summary>
        /// Creates integrity violation object from Tuple created from FileInfoRequester RetrieveFileInfo.
        /// </summary>
        /// <param name="newHash">Hash of real time file.</param>
        /// <param name="resultTuple">Tuple returned from RetrieveFileInfo.</param>
        /// <returns>IntegrityViolation data structure.</returns>
        private IntegrityViolation CreateViolation(string newHash, Tuple<string, string, long, long, long> resultTuple)
        {
            return new IntegrityViolation();
        }

        /// <summary>
        /// Retrieving a set of the database, compare with system files.
        /// </summary>
        /// <returns>Violations found via mismatching Hashes.</returns>
        public async Task<List<IntegrityViolation>> CheckIntegrity(CancellationToken ct)
        {
            await Task.CompletedTask;
            List<IntegrityViolation> violationSet = new();
            violationSet.Add(new IntegrityViolation());
            return violationSet;
        }

        /// <summary>
        /// Singular comparison between the database and a windows file.
        /// </summary>
        /// <returns>Violation or Null</returns>
        public async Task<List<IntegrityViolation>> CheckIntegrityDirectory()
        {
            List<IntegrityViolation> violationSet = new();
            await Task.CompletedTask;
            return violationSet;
        }

        public int Set
        {
            get
            {
                return 0;
            }
        }
    }
}
