/**************************************************************************
 * File:        IIntegrityCycler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Interface for: Manages the start of the Integrity Scanning process, deals with returned Violations when it initiates multiple 
 * Integrity data poolers to scan certain sets of database queries.
 * Last Modified: 29/09/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.DataTypes;

namespace SimpleAntivirus.IntegrityModule.Interface
{
    public interface IIntegrityCycler
    {

        public event EventHandler<ProgressArgs> ProgressUpdate;

        public Task CancelScan();

        /// <summary>
        /// Initiate scan of entire IntegrityDatabase and compare with real time system documents.
        /// </summary>
        /// <remarks>One of the most important functions.</remarks>
        public Task<List<IntegrityViolation>> InitiateScan();

        /// <summary>
        /// Similar to InitiateScan, except it only scans given directory in database.
        /// </summary>
        /// <param name="path">Windows File Path</param>
        public Task InitiateDirectoryScan(string directoryPath);

        public int AmountSet { get; set; }
    }
}
