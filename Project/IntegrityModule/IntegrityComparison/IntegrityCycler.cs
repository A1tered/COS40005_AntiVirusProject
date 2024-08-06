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
                poolerObject.CheckIntegrity().ForEach(summaryViolation.Add);
            }
            // Note to self to add asynchronous support, and to immediately emit alerts rather than holding onto them.
            Console.WriteLine($"Violations Found: {summaryViolation.Count()}");
            return true;
        }
    }
}
