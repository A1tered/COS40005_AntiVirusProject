using DatabaseFoundations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IntegrityModule.ControlClasses
{
    public class IntegrityConfigurator
    {
        private IntegrityDatabaseIntermediary _database;
        public IntegrityConfigurator(IntegrityDatabaseIntermediary integrityDatabase)
        {
            _database = integrityDatabase;
        }

        public bool AddIntegrityDirectory(string path)
        {
            return _database.AddEntry(path);
        }

        public bool RemoveIntegrityDirectory(string path)
        {
            return _database.RemoveEntry(path);
        }

        public bool RemoveAll()
        {
            return _database.DeleteAll();
        }
    }
}
