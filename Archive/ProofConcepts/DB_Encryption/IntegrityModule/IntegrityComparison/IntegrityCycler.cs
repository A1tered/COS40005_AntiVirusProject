using DatabaseFoundations;
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
        public IntegrityCycler(IntegrityDatabaseIntermediary database)
        {
            _database = database;
            _amountPerSet = 100;
        }

        public bool InitiateScan()
        {
            List<IntegrityDataPooler> dataPoolerList = new();
            List<Task<List<IntegrityViolation>>> taskList = new();
            List<IntegrityViolation> summaryViolation = new();
            long amountEntry = _database.QueryAmount("IntegrityTrack");
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
            // Note to self to add asynchronous support, and to immediately emit alerts rather than holding onto them.
            Task.WaitAll(taskList.ToArray());
            foreach (Task<List<IntegrityViolation>> taskItem in taskList)
            {
                taskItem.Result.ForEach(summaryViolation.Add);
            }
            Console.WriteLine($"Violations Found: {summaryViolation.Count()}");
            return true;
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
