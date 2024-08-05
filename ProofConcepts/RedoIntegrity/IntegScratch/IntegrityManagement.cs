using IntegrityCheckingFromScratch.ScanRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityCheckingFromScratch
{
    public class IntegrityManagement
    {
        private IntegrityConfigurator _integrityConfigurator;
        private IntegrityCycler _integrityCycler;
        public IntegrityManagement()
        {
            IntegrityDatabaseIntermediary databaseHandler = new("IntegrityDatabase");
            _integrityConfigurator = new(databaseHandler);
            _integrityCycler = new(databaseHandler);
        }

        public bool Scan()
        {
            return _integrityCycler.InitiateScan(); 
        }

        public bool AddBaseline(string directory)
        {
            return _integrityConfigurator.AddIntegrityDirectory(directory);
        }

        public bool RemoveBaseline(string directory)
        {
            return _integrityConfigurator.RemoveIntegrityDirectory(directory);
        }

        public bool ClearDatabase()
        {
            return _integrityConfigurator.ClearIntegrityDatabase();
        }
    }
}
