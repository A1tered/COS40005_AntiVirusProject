using DatabaseFoundations;
using IntegrityModule.IntegrityComparison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.ControlClasses
{
    public class IntegrityManagement
    {
        private IntegrityConfigurator _integrityConfigurator;
        private IntegrityCycler _integrityCycler;
        public IntegrityManagement(IntegrityDatabaseIntermediary integrityIntermediary)
        {
            _integrityConfigurator = new IntegrityConfigurator(integrityIntermediary);
            _integrityCycler = new IntegrityCycler(integrityIntermediary);
        }

        // Alert Handler to be placed here at later date.

        public bool Scan()
        {
           return _integrityCycler.InitiateScan();
        }

        public bool AddBaseline(string path)
        {
           return _integrityConfigurator.AddIntegrityDirectory(path);
        }

        public bool RemoveBaseline(string path)
        {
            return _integrityConfigurator.RemoveIntegrityDirectory(path);
        }

        public bool ClearDatabase()
        {
            return _integrityConfigurator.RemoveAll();
        }
    }
}
