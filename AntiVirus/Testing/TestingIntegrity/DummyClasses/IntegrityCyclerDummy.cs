/**************************************************************************
 * File:        IntegrityCycler.cs
 * Author:      Christopher Thompson, etc.
 * Description: Manages the start of the Integrity Scanning process, deals with returned Violations when it initiates multiple 
 * Integrity data poolers to scan certain sets of database queries.
 * Last Modified: 26/08/2024
 **************************************************************************/

using SimpleAntivirus.IntegrityModule.Alerts;
using SimpleAntivirus.IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAntivirus.IntegrityModule.Db;
using System.Diagnostics;
using SimpleAntivirus.IntegrityModule.Interface;

namespace TestingIntegrity.DummyClasses
{
    public class IntegrityCyclerDummy : IIntegrityCycler
    {
        // How many data sets does 1 task undertake? (Higher = Less Speed/Less Intensive) (Lower = High Speed / More Intensive)
        private int _amountPerSet;
        private IIntegrityDatabaseIntermediary _database;
        private IViolationHandler _violationHandler;
        private CancellationTokenSource _cancelToken;
        private Type _poolerType;
        public IntegrityCyclerDummy(IIntegrityDatabaseIntermediary database, IViolationHandler violationHandler)
        {

        }

        public event EventHandler<ProgressArgs> ProgressUpdate;


        public void SetPoolerType(Type type)
        {

        }

        public async Task CancelScan()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Initiate scan of entire IntegrityDatabase and compare with real time system documents.
        /// </summary>
        /// <remarks>One of the most important functions.</remarks>
        public async Task<List<IntegrityViolation>> InitiateScan()
        {
            await Task.CompletedTask;
            return new List<IntegrityViolation>();
        }

        /// <summary>
        /// Similar to InitiateScan, except it only scans given directory in database.
        /// </summary>
        /// <param name="path">Windows File Path</param>
        public async Task InitiateDirectoryScan(string directoryPath)
        {
            await Task.CompletedTask;
        }

        public int AmountSet
        {
            get
            {
                return 2;
            }
            set
            {

            }
        }
    }
}
