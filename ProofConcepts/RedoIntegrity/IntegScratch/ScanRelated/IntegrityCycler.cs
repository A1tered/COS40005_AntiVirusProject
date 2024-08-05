using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch.ScanRelated
{
    public class IntegrityCycler
    {
        private IntegrityDatabaseIntermediary _databaseHolder;
        public IntegrityCycler(IntegrityDatabaseIntermediary databaseHandler)
        {
            _databaseHolder = databaseHandler;
        }

        public bool InitiateScan(int amountPerSet = 1000)
        {
            //_databaseHolder.AssessAmountRows() / amountPerSet

            // \/ believed issue to be overlap of some kind...
            int amountOfPoolersRequired = _databaseHolder.AssessAmountRows() / amountPerSet;
            Console.WriteLine($"Row searches required: {_databaseHolder.AssessAmountRows()}");
            amountOfPoolersRequired++;
            List<IntegrityDataPooler> poolers = new();
            List < Task<string> > taskManager = new();
            for (int amountPoolerTracker = 0; amountPoolerTracker < amountOfPoolersRequired; amountPoolerTracker++)
            {
                poolers.Add(new IntegrityDataPooler(_databaseHolder, amountPoolerTracker, amountPerSet));
            }
            foreach (IntegrityDataPooler dataPoolGet in poolers)
            {
                taskManager.Add(Task.Run(() => dataPoolGet.CheckIntegrity()));
            }
            Task.WaitAll(taskManager.ToArray());
            Console.WriteLine("Summary:");
            foreach (Task<string> taskOutput in taskManager)
            {
                Console.Write(taskOutput.Result);
            }
            return true;
        }
    }
}
