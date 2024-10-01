using DatabaseFoundations;
using IntegrityModule.Alerts;
using IntegrityModule.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.IntegrityComparison
{
    public class IntegrityCycler
    {
        // How many data sets does 1 thread undertake? (Higher = Less Speed/Less Intensive) (Lower = High Speed / More Intensive)
        private int _amountPerSet;
        private IntegrityDatabaseIntermediary _database;
        private ViolationHandler _violationHandler;
        public IntegrityCycler(IntegrityDatabaseIntermediary database, ViolationHandler violationHandler)
        {
            _database = database;
            _violationHandler = violationHandler;
            _amountPerSet = 100;
        }

        /// <summary>
        /// Initiate scan of entire IntegrityDatabase and compare with real time system documents.
        /// </summary>
        /// <remarks>One of the most important functions.</remarks>
        public void InitiateScan()
        {
            List<IntegrityDataPooler> dataPoolerList = new();
            List<Task<List<IntegrityViolation>>> taskList = new();
            List<IntegrityViolation> summaryViolation = new();
            long amountEntry = _database.QueryAmount("IntegrityTrack");
            if (amountEntry == 0)
            {
                Console.WriteLine("No entries to scan");
                return;
            }
            decimal divison = (decimal)amountEntry / _amountPerSet;
            int sets = Convert.ToInt32(Math.Ceiling(divison));
            // For each set, give it to a pooler
            for (int cycle = 0; cycle < sets; cycle++)
            {
                dataPoolerList.Add(new IntegrityDataPooler(_database, cycle, _amountPerSet));
            }
            // Now tell those poolers to get a move on.
            foreach (IntegrityDataPooler poolerObject in dataPoolerList)
            {
                Console.WriteLine($"{poolerObject.Set} / {sets} - Pooler Set Started");
                taskList.Add(Task.Run(() => poolerObject.CheckIntegrity()));
            }
            while (taskList.Exists(x => x.IsCompleted == false))
            {
                Task.WaitAny(taskList.ToArray());
                foreach (Task<List<IntegrityViolation>> taskItem in taskList)
                {
                    if (taskItem.IsCompleted)
                    {
                        taskItem.Result.ForEach(summaryViolation.Add);
                        // For each violation, send to violation handler
                        taskItem.Result.ForEach(_violationHandler.ViolationAlert);
                    }
                }
                taskList.RemoveAll(x => x.IsCompleted);
            }
            Console.WriteLine($"Violations Found: {summaryViolation.Count()}");
        }

        /// <summary>
        /// Similar to InitiateScan, except it only scans 1 file.
        /// </summary>
        /// <param name="path">Windows File Path</param>
        public void InitiateSingleScan(string path)
        {
            IntegrityDataPooler singlePooler = new(_database, path);
            IntegrityViolation violation = singlePooler.CheckIntegrityFile();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Reactive Alert: {violation.OriginalHash} -> {violation.Hash}, Size change: {violation.OriginalSize} -> {violation.FileSizeBytes}");
            Console.ResetColor();
        }

        public int AmountSet
        {
            get
            {
                return _amountPerSet;
            }
            set
            {
                _amountPerSet = value;
            }
        }
    }
}
