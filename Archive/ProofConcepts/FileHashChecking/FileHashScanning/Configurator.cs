using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHashScanning
{
    public class Configurator
    {
        DatabaseConnector _databaseConnector;
        public Configurator(string database)
        {
            _databaseConnector = new(database, true);
        }

        public void AddHashToDatabase(string directory)
        {
            if (_databaseConnector.AddHash(new Hasher().OpenHashFile(directory)))
            {
                Console.WriteLine("Added hash to database");
            }
            else
            {
                Console.WriteLine("Hash not added, likely already added.");
            }
        }
    }
}
